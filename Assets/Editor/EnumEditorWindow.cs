using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class EnumEditorWindow : EditorWindow
{
    private string enumName = "NewEnum";
    private string enumDescription = "";
    private string[] enumValues = new string[] { "Value1", "Value2", "Value3" };
    private string selectedEnumName = "";
    private string[] existingEnumNames;
    private bool createNewEnum = false;
    private Vector2 scrollPos;

    [MenuItem("Tools/Enum Editor")]
    public static void ShowWindow()
    {
        GetWindow<EnumEditorWindow>("Enum Editor");
    }

    
    private void OnEnable()
    {
        LoadExistingEnums();
    }

    private void LoadExistingEnums()
    {
        string folderPath = "Assets/Enums";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Enums");
        }

        string[] enumFiles = Directory.GetFiles(folderPath, "*.cs");
        List<string> enumNamesList = new List<string>();
        foreach (string file in enumFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            enumNamesList.Add(fileName);
        }
        existingEnumNames = enumNamesList.ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enum Editor", EditorStyles.boldLabel);

        createNewEnum = EditorGUILayout.Toggle("Create New Enum", createNewEnum);

        if (!createNewEnum && existingEnumNames != null && existingEnumNames.Length > 0)
        {
            int selectedEnumIndex = EditorGUILayout.Popup("Select Enum", Array.IndexOf(existingEnumNames, selectedEnumName), existingEnumNames);
            if (selectedEnumIndex >= 0 && selectedEnumIndex < existingEnumNames.Length)
            {
                if (selectedEnumName != existingEnumNames[selectedEnumIndex])
                {
                    selectedEnumName = existingEnumNames[selectedEnumIndex];
                    LoadEnumValues(selectedEnumName);
                }
            }
        }
        else
        {
            enumName = EditorGUILayout.TextField("New Enum Name", enumName);
        }

        EditorGUILayout.LabelField("Enum Description:");
        enumDescription = EditorGUILayout.TextArea(enumDescription, GUILayout.Height(60));

        EditorGUILayout.LabelField("Enum Values:");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < enumValues.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            enumValues[i] = EditorGUILayout.TextField($"Value {i + 1}", enumValues[i]);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveValueAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Value"))
        {
            Array.Resize(ref enumValues, enumValues.Length + 1);
        }

        bool hasEmptyValue = HasEmptyValue();
        EditorGUI.BeginDisabledGroup(hasEmptyValue || string.IsNullOrEmpty(enumName));
        if (GUILayout.Button("Generate Enum Script"))
        {
            if (createNewEnum)
            {
                GenerateEnumScript(enumName, enumDescription, enumValues);
            }
            else
            {
                GenerateEnumScript(selectedEnumName, enumDescription, enumValues);
            }
        }
        EditorGUI.EndDisabledGroup();

        if (hasEmptyValue)
        {
            EditorGUILayout.HelpBox("Cannot generate script: One or more enum values are empty.", MessageType.Error);
        }

        // If enumDescription contains < or > characters, display a warning
        if (enumDescription.Contains("<") || enumDescription.Contains(">"))
        {
            EditorGUILayout.HelpBox("The enum description contains '<' or '>' characters, which may cause issues in the generated script.", MessageType.Warning);
        }

        if (!string.IsNullOrEmpty(enumDescription))
            foreach (string line in enumDescription.Split('\n'))
            {
                if (string.IsNullOrEmpty(line))
                {
                    EditorGUILayout.HelpBox("Empty line found in description. This will be removed from the generated script.", MessageType.Warning);
                    break;
                }
            }
    }

    private void LoadEnumValues(string enumName)
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
        }
        Array.Resize(ref enumValues, enumValues.Length - 1);
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