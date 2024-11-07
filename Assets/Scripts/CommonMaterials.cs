using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class CommonMaterials : EditorWindow
{
    private string enumName = "CommonMaterial";
    private string enumDescription = "Materials that can be retrieved from CommonMaterials";
    private string[] enumValues = new string[] { "New Material" };
    private Vector2 scrollPos;

    private static Material[] materials = new Material[] { null };

    Regex invalidEnumNamePattern = new Regex(@"[^a-zA-Z0-9_]");
    HashSet<string> reservedKeywords = new HashSet<string>
    {
    "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
    "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
    "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach",
    "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
    "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
    "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
    "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
    "ushort", "using", "virtual", "void", "volatile", "while"
    };

    [MenuItem("Tools/Common Materials")]
    public static void ShowWindow()
    {
        GetWindow<CommonMaterials>("Common Materials").LoadValues("CommonMaterial");
    }

    private void OnEnable()
    {
        LoadValues(enumName);
    }

    public static Material GetMaterial(CommonMaterial commonMaterial)
    {
        return Resources.Load<CommonMaterialsHolder>("CommonMaterialsHolder").materials[(int)commonMaterial];
    }

    private void OnGUI()
    {
        // Lowkey text that says to press load materials if there are errors
        EditorGUILayout.HelpBox("Usually pressing Load Materials or reopening this window will fix errors.\nAlso note that values will occasionally be loaded forcefully, such as on script compilation, so save often.", MessageType.Info);

        // Try to load CommonMaterial enum if it exists and button is pressed
        if (GUILayout.Button("Load Materials"))
        {
            LoadValues(enumName);
        }

        if (enumValues.Length != materials.Length)
        {
            Array.Resize(ref enumValues, materials.Length);
        }

        EditorGUILayout.LabelField("Materials:");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < enumValues.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            enumValues[i] = EditorGUILayout.TextField(enumValues[i]);
            materials[i] = (Material)EditorGUILayout.ObjectField(materials[i], typeof(Material), false);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveValueAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Material"))
        {
            Array.Resize(ref enumValues, enumValues.Length + 1);
            Array.Resize(ref materials, materials.Length + 1);
        }

        bool hasEmptyValue = HasEmptyValue();

        bool hasInvalidCharacter = false;

        foreach (var value in enumValues)
        {
            if (String.IsNullOrEmpty(value))
            {
                continue;
            }

            if (invalidEnumNamePattern.IsMatch(value))
            {
                hasInvalidCharacter = true;
                break;
            }
        }

        bool containsReservedKeyword = false;

        foreach (var value in enumValues)
        {
            if (String.IsNullOrEmpty(value))
            {
                continue;
            }

            if (reservedKeywords.Contains(value))
            {
                containsReservedKeyword = true;
                break;
            }
        }

        bool hasDuplicateValues = false;

        for (int i = 0; i < enumValues.Length; i++)
        {
            for (int j = i + 1; j < enumValues.Length; j++)
            {
                if (enumValues[i] == enumValues[j] && !String.IsNullOrEmpty(enumValues[i]))
                {
                    hasDuplicateValues = true;
                    break;
                }
            }
        }

        EditorGUI.BeginDisabledGroup(hasEmptyValue || string.IsNullOrEmpty(enumName) || hasInvalidCharacter || hasDuplicateValues);
        if (GUILayout.Button("Save"))
        {
            {
                GenerateEnumScript(enumName, enumDescription, enumValues);
                SaveMaterials();
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        if (hasEmptyValue)
        {
            EditorGUILayout.HelpBox("Cannot save: One or more values are empty.", MessageType.Error);
        }
        if (hasInvalidCharacter)
        {
            EditorGUILayout.HelpBox("Cannot save: Names cannot contain special characters", MessageType.Error);
        }
        if (hasDuplicateValues)
        {
            EditorGUILayout.HelpBox("Cannot save: Duplicate values found.", MessageType.Error);
        }
        if (containsReservedKeyword)
        {
            // List values that are reserved keywords
            string reservedValues = "";
            foreach (var value in enumValues)
            {
                if (reservedKeywords.Contains(value))
                {
                    reservedValues += value + ", ";
                }
            }
            reservedValues = reservedValues.TrimEnd(',', ' ');

            EditorGUILayout.HelpBox($"Cannot save: The following values are reserved keywords: {reservedValues}", MessageType.Error);
        }
    }

    private void SaveMaterials()
    {
        CommonMaterialsHolder holder = Resources.Load<CommonMaterialsHolder>("CommonMaterialsHolder");
        if (holder == null)
        {
            holder = ScriptableObject.CreateInstance<CommonMaterialsHolder>();
            AssetDatabase.CreateAsset(holder, "Assets/Resources/CommonMaterialsHolder.asset");
        }

        holder.materials = materials;
        EditorUtility.SetDirty(holder);
        AssetDatabase.SaveAssets();
    }

    private void LoadValues(string enumName)
    {
        string folderPath = "Assets/Enums";
        string filePath = Path.Combine(folderPath, $"{enumName}.cs");

        if (File.Exists(filePath))
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    List<string> valuesList = new List<string>();
                    string line;
                    bool isInEnumSection = false;

                    enumDescription = "";

                    while ((line = reader.ReadLine()) != null)
                    {
                        // Look for the start of the enum definition
                        if (!isInEnumSection && line.Contains("public enum " + enumName))
                        {
                            isInEnumSection = true;
                            continue;
                        }

                        // Read enum values until the end of the enum definition
                        if (isInEnumSection)
                        {
                            if (line.Contains("{"))
                            {
                                continue;
                            }

                            // Check for the end of the enum definition
                            if (line.Contains("}"))
                            {
                                break;
                            }

                            // Extract enum values (remove comments and trim)
                            string value = line.Trim();
                            if (!string.IsNullOrEmpty(value) && !value.StartsWith("//"))
                            {
                                // Remove any assigned values (e.g., Value = 1)
                                int equalIndex = value.IndexOf('=');
                                if (equalIndex != -1)
                                {
                                    value = value.Substring(0, equalIndex).Trim();
                                }

                                // Remove trailing commas
                                if (value.EndsWith(","))
                                {
                                    value = value.Substring(0, value.Length - 1).Trim();
                                }

                                // Add to values list
                                if (!string.IsNullOrEmpty(value))
                                {
                                    valuesList.Add(value);
                                }
                            }
                        }

                        // Read enum description if available
                        if (line.StartsWith("///"))
                        {
                            if (line.Contains("summary>"))
                            {
                                continue;
                            }

                            enumDescription += line.TrimStart('/') + "\n";
                        }
                    }

                    enumDescription = enumDescription.TrimEnd('\n');

                    // Assign loaded values to enumValues array
                    enumValues = valuesList.ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading enum values from file '{filePath}': {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Enum file '{filePath}' not found.");
        }

        CommonMaterialsHolder holder;

        // Check if the scriptable object exists
        if (!File.Exists("Assets/Resources/CommonMaterialsHolder.asset"))
        {
            Debug.LogWarning($"CommonMaterialsHolder.asset not found. Creating new one.");

            // Check if the Resources folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                Debug.LogWarning("Resources folder not found. Creating new one.");
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            holder = ScriptableObject.CreateInstance<CommonMaterialsHolder>();
            AssetDatabase.CreateAsset(holder, "Assets/Resources/CommonMaterialsHolder.asset");
            AssetDatabase.SaveAssets();
            return;
        }

        holder = Resources.Load<CommonMaterialsHolder>("CommonMaterialsHolder");
        materials = holder.materials;
    }

    private void RemoveValueAt(int index)
    {
        if (index < 0 || index >= enumValues.Length)
        {
            return;
        }

        for (int i = index; i < enumValues.Length - 1; i++)
        {
            enumValues[i] = enumValues[i + 1];
            materials[i] = materials[i + 1];
        }
        Array.Resize(ref enumValues, enumValues.Length - 1);
        Array.Resize(ref materials, materials.Length - 1);
    }

    private bool HasEmptyValue()
    {
        foreach (var value in enumValues)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
        }
        foreach (var material in materials)
        {
            if (material == null)
            {
                return true;
            }
        }
        return false;
    }

    private void GenerateEnumScript(string enumName, string enumDescription, string[] enumValues)
    {
        string folderPath = "Assets/Enums";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Enums");
        }

        string path = Path.Combine(folderPath, $"{enumName}.cs");
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("///<summary>");

            foreach (string line in enumDescription.Split('\n'))
            {
                if (!string.IsNullOrEmpty(line))
                    writer.WriteLine("/// " + line.Trim());
            }

            writer.WriteLine("///</summary>");
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");

            foreach (string value in enumValues)
            {
                writer.WriteLine("    " + value + ",");
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }
}
