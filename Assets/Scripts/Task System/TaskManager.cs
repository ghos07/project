using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct Task
{
    public readonly int priority;
    public readonly System.Action action;
    public readonly string name;
    public readonly bool mustFinish;
    public readonly bool multitask;
    public bool complete;
    
    // operator overloading
    public static bool operator ==(Task a, Task b)
    {
        return a.priority == b.priority && a.action == b.action && a.name == b.name && a.mustFinish == b.mustFinish && a.multitask == b.multitask;
    }

    public static bool operator !=(Task a, Task b)
    {
        return a.priority != b.priority || a.action != b.action || a.name != b.name || a.mustFinish != b.mustFinish || a.multitask != b.multitask;
    }

    public override bool Equals(object obj)
    {
        return obj is Task task && this == task;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(priority, action, name, mustFinish, multitask);
    }

    public Task(System.Action action, int priority, string name, bool mustFinish, bool multitask)
    {
        this.priority = priority;
        this.action = action;
        this.name = name;
        this.mustFinish = mustFinish;
        this.multitask = multitask;
        complete = false;
    }

    public Task(System.Action action, int priority, string name, bool mustFinish)
    {
        this.priority = priority;
        this.action = action;
        this.name = name;
        this.mustFinish = mustFinish;
        this.multitask = false;
        complete = false;
    }

    public Task(System.Action action, int priority, string name)
    {
        this.priority = priority;
        this.action = action;
        this.name = name;
        this.mustFinish = false;
        this.multitask = false;
        complete = false;
    }
}

public class TaskManager : MonoBehaviour
{
    private List<Task> tasks = new();
    private List<Task> activeTasks = new();

    public void AddTask(Task task)
    {
        if (tasks.Contains(task) || activeTasks.Contains(task))
        {
            return;
        }
        tasks.Add(task);
    }

    public void AddTask(Task task, bool allowDuplicates)
    {
        if (allowDuplicates)
        {
            tasks.Add(task);
        }
        else
        {
            AddTask(task);
        }
    }

    private void Update()
    {
        if (tasks.Count == 0)
        {
            return;
        }

        foreach (Task task in activeTasks)
        {
            if (task.complete)
            {
                activeTasks.Remove(task);
            }
            task.action();
        }
    }
}
