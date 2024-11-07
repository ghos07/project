using System.Collections.Generic;
using UnityEngine;
using System;

public class ModifierManager : MonoBehaviour
{
    private Dictionary<Modifier, List<float>> modifiers = new();
    private List<(Action, float)> queuedModifierActions = new();
    public event Action OnAddModifiers;

    public static ModifierManager GetModifierManager(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<ModifierManager>(out ModifierManager modifierManager))
        {
            return modifierManager;
        }
        Debug.Log("No ModifierManager found on " + gameObject.name + ", adding one now.");
        return gameObject.AddComponent<ModifierManager>();
    }

    private void Update()
    {
        Dictionary<Modifier, List<float>> publicModifiers = new(modifiers);
        ClearModifiers();

        List<(Action, float)> queuedActions = new(queuedModifierActions);
        queuedModifierActions.Clear();
        foreach ((Action action, float persistence) in queuedActions)
        {
            action.Invoke();
            if (persistence - Time.deltaTime > 0)
            {
                QueueModifierAction(action, persistence - Time.deltaTime);
            }
        }

        OnAddModifiers?.Invoke();
    }

    private float CombineModifiers(List<float> modifiers)
    {
        float totalModifier = 1;
        foreach (float modifier in modifiers)
        {
            if (modifier > 1)
            {
                totalModifier += modifier - 1;
            }
        }
        foreach (float modifier in modifiers)
        {
            if (modifier < 1)
            {
                totalModifier *= modifier;
            }
        }
        return totalModifier;
    }

    public void AddModifier(Modifier modifier)
    {
        if (!modifiers.ContainsKey(modifier))
        {
            modifiers.Add(modifier, new List<float>());
        }
    }

    public void AddModifier(Modifier modifier, float defaultValue)
    {
        if (!modifiers.ContainsKey(modifier))
        {
            modifiers.Add(modifier, new List<float>());
        }
        modifiers[modifier].Add(defaultValue);
    }

    public bool RemoveModifier(Modifier modifier)
    {
        return modifiers.Remove(modifier);
    }

    public void AddValue(Modifier modifier, float value)
    {
        if (modifiers.ContainsKey(modifier))
        {
            modifiers[modifier].Add(value);
        }
        else
        {
            Debug.Log("Modifier " + modifier + " does not exist, adding now.");
            AddModifier(modifier, value);
        }
    }

    public void RemoveValue(Modifier modifier, float value)
    {
        if (modifiers.ContainsKey(modifier))
        {
            modifiers[modifier].Remove(value);
        }
        else
        {
            Debug.Log("Modifier " + modifier + " does not exist, adding now.");
            AddModifier(modifier, value);
        }
    }

    public float GetValue(Modifier modifier)
    {
        if (modifiers.ContainsKey(modifier))
        {
            return CombineModifiers(modifiers[modifier]);
        }
        else
        {
            Debug.Log("Modifier " + modifier + " does not exist, adding now.");
            AddModifier(modifier);
            return 1;
        }
    }

    public List<KeyValuePair<Modifier, float>> GetModifiers()
    {
        List<KeyValuePair<Modifier, float>> modifierList = new();
        foreach (KeyValuePair<Modifier, List<float>> modifier in modifiers)
        {
            modifierList.Add(new KeyValuePair<Modifier, float>(modifier.Key, CombineModifiers(modifier.Value)));
        }
        return modifierList;
    }

    public List<KeyValuePair<Modifier, List<float>>> GetValues()
    {
        return new(modifiers);
    }

    public void ClearModifiers()
    {
        foreach (var modifier in modifiers)
        {
            modifier.Value.Clear();
        }
    }

    public void QueueModifierAction(System.Action action)
    {
        queuedModifierActions.Add((action, 0));
    }

    public void QueueModifierAction(System.Action action, float persistence) // TODO: Switch from frames to seconds
    {
        queuedModifierActions.Add((action, persistence));
    }
}
