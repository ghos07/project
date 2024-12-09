using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Tasks can be used to queue actions to be run over a period of multiple frames.
/// T
/// </summary>
public class Task
{
    public readonly int priority;
    public readonly System.Action<Task> action;
    public readonly string name;
    public readonly bool mustFinish;
    public readonly bool multitask;
    public bool complete;
    public int frameDuration;
    public float secondDuration;

    public bool runWithPhysics = false;

    public TaskContext context;
    
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
        return System.HashCode.Combine(priority, action, name, mustFinish, multitask, context);
    }

    public void MarkComplete()
    {
        complete = true;
    }

    public Task(System.Action<Task> action) : this(action, "", 0, false, true)
    {
    }

    public Task(System.Action<Task> action, string name) : this(action, name, 0, false, true)
    {
    }

    public Task(System.Action<Task> action, string name, int priority) : this(action, name, priority, false, true)
    {
    }

    public Task(System.Action<Task> action, string name, int priority, bool mustFinish, bool multitask)
    {
        this.priority = priority;
        this.action = action;
        this.name = name;
        this.mustFinish = mustFinish;
        this.multitask = multitask;
        complete = false;
        context = TaskContext.Create();

        frameDuration = -1;
        secondDuration = -1;
    }

    public Task Clone()
    {
        Task task = new Task(action, name, priority, mustFinish, multitask);
        task.complete = complete;
        task.context = context.Clone();
        task.frameDuration = frameDuration;
        task.secondDuration = secondDuration;
        return task;
    }
}

public struct TaskContext
{
    public readonly int createdFrame;
    public readonly float createdTime;
    public readonly int startFramePhysics;
    public int lastFramePhysics;
    public float startTime;
    public int startFrame;
    /// <summary>
    /// The number of frames that have elapsed since the task was created.
    /// Updated in the TaskManager.Update() method, so keep in mind that this value may not be accurate if time has passed since the task action was called.
    /// </summary>
    public int framesElapsed;
    public readonly float TimeElapsed => TimeUnityless.Time - startTime;
    public int PhysicsFramesElapsed => TimeUnityless.PhysicsFrames - startFramePhysics;

    public TaskContext(float startTime, int startFrame, int startFramePhysics)
    {
        createdFrame = Time.frameCount;
        createdTime = TimeUnityless.Time;
        this.startTime = -1;
        this.startFrame = startFrame;
        framesElapsed = 0;
        this.startFramePhysics = startFramePhysics;
        lastFramePhysics = -1;
    }

    private TaskContext(int createdFrame, float createdTime, int startFrame, float startTime, int startFramePhysics)
    {
        this.createdFrame = createdFrame;
        this.createdTime = createdTime;
        framesElapsed = 0;
        this.startFrame = startFrame;
        this.startTime = startTime;
        this.startFramePhysics = startFramePhysics;
        lastFramePhysics = -1;
    }

    // TODO: Review need for constructors with startFrame and startTime
    public static TaskContext Create()
    {
        return new TaskContext(Time.time, Time.frameCount, TimeUnityless.PhysicsFrames);
    }

    public TaskContext Clone()
    {
        return new TaskContext(createdFrame, createdTime, startFrame, startTime, TimeUnityless.PhysicsFrames);
    }
}

public class TaskManager : MonoBehaviour
{
    public string taskManagerName = "Task Manager";

    public bool isGlobal = false;
    public bool runAsynchronously = true;
    public bool isSequential = false;
    public bool isPaused = false;

    private List<Task> queuedtasks = new();
    private List<Task> activeTasks = new();

    private static TaskManager globalInstance;
    public static TaskManager GlobalInstance
    {
        get
        {
            // Check if there is a stored global task manager instance already created and active in the scene
            if (globalInstance == null || !globalInstance.isGlobal || !globalInstance.gameObject.activeInHierarchy)
            {
                // If there is no stored global task manager instance, check if there is a global task manager in the scene
                TaskManager[] taskManagers = FindObjectsOfType<TaskManager>();
                foreach (TaskManager taskManager in taskManagers)
                {
                    if (taskManager.isGlobal)
                    {
                        globalInstance = taskManager;
                        return globalInstance;
                    }
                }

                // If there is no global task manager in the scene, create one
                GameObject taskManagerObject = new GameObject("Task Manager");
                globalInstance = taskManagerObject.AddComponent<TaskManager>();
                globalInstance.isGlobal = true;
            }
            
            return globalInstance;
        }
    }

    public List<Task> QueuedTasks => queuedtasks;
    public List<Task> ActiveTasks => activeTasks;

    public static TaskManager NewTaskManager()
    {
        return NewTaskManager("Task Manager");
    }

    public static TaskManager NewTaskManager(string name)
    {
        GameObject taskManagerObject = new GameObject(name);
        TaskManager taskManager = taskManagerObject.AddComponent<TaskManager>();
        return taskManager;
    }

    /// <summary>
    /// Adds a task to the task manager.
    /// </summary>
    /// <param name="action"> The action to be added. </param>
    /// <param name="runWithPhysics"> Whether or not the task should run in sync with fixed delta time. </param>
    /// <param name="allowDuplicates"> Whether or not to allow duplicate tasks. </param>
    public void AddTask(System.Action<Task> action, bool runWithPhysics = false, bool allowDuplicates = true)
    {
        Task task = new(action)
        {
            runWithPhysics = runWithPhysics
        };
        AddTask(task, allowDuplicates);
    }

    public void DoNextFrame(System.Action action, bool runWithPhysics = false)
    {
        DoForFrames((Task task) => { action(); }, 1, runWithPhysics);
    }

    public void DoNextFrame(System.Action<Task> action, bool runWithPhysics = false)
    {
        DoForFrames(action, 1, runWithPhysics);
    }

    public void DoForFrames(System.Action<Task> action, int frames, bool runWithPhysics = false)
    {
        Task task = new Task(action);
        task.frameDuration = frames;
        AddTask(task, runWithPhysics);
    }

    /// <summary>
    /// Adds a task to the task manager.
    /// </summary>
    /// <param name="task"> The task to be added. </param>
    /// <param name="allowDuplicates"> Whether or not to allow duplicate tasks. </param>
    public void AddTask(Task task, bool allowDuplicates = true)
    {
        if (allowDuplicates)
        {
            queuedtasks.Add(task);
        }
        else
        {
            if (queuedtasks.Contains(task) || activeTasks.Contains(task))
            {
                return;
            }
            queuedtasks.Add(task);
        }
    }

    /// <summary>
    /// Adds a task to the task manager.
    /// </summary>
    /// <param name="action"> The action to be added. </param>
    /// <param name="priority"> The priority of the task. </param>
    /// <param name="name"> The name of the task. </param>
    /// <param name="mustFinish"> Whether or not the task must finish before the next task can start. </param>
    /// <param name="multitask"> Whether or not the task can be run multiple times simultaneously. </param>
    /// <param name="allowDuplicates"> Whether or not to allow duplicate tasks. </param>
    /// <param name="context"> The data object to be passed to the task. </param>
    /// <returns> The task that was added. </returns>
    /// <remarks> The task will be added to the task manager. </remarks>
    /// <remarks> If allowDuplicates is false, the task will not be added if it is already in the task manager. </remarks>
    /// <remarks> If the task is already in the task manager, it will be added again if allowDuplicates is true. </remarks>
    public Task AddTask(System.Action<Task> action, TaskContext context, string name, int priority, bool mustFinish, bool multitask, bool allowDuplicates, bool runWithPhysics)
    {
        Task task = new Task(action, name, priority, mustFinish, multitask);
        task.context = context;
        task.runWithPhysics = runWithPhysics;
        AddTask(task, allowDuplicates);
        return task;
    }

    private async System.Threading.Tasks.Task RunTaskAsync(int activeIndex)
    {
        Task task = activeTasks[activeIndex];

        if (Time.frameCount == task.context.createdFrame)
        {
            return;
        }

        else if (task.context.startTime == -1)
        {
            task.context.framesElapsed = 0;
            task.context.startFrame = Time.frameCount;
            task.context.startTime = TimeUnityless.Time;
        }

        bool runTask = true;

        if (task.frameDuration != -1 && task.context.framesElapsed >= task.frameDuration)
        {
            task.complete = true;
            runTask = false;
        }

        if (task.secondDuration != -1 && task.context.TimeElapsed >= task.secondDuration)
        {
            task.complete = true;
            runTask = false;
        }

        await System.Threading.Tasks.Task.Run(() => { lock (task) { if (runTask) task.action(task); } });

        if (task.complete && activeTasks.Contains(task))
        {
            if (!task.mustFinish)
            {
                activeTasks.RemoveAt(activeIndex);
            }
            else
            {
                if (task.context.framesElapsed >= task.frameDuration)
                {
                    activeTasks.RemoveAt(activeIndex);
                }

                if (task.context.TimeElapsed >= task.secondDuration)
                {
                    activeTasks.RemoveAt(activeIndex);
                }
            }
        }
    }

    private void RunTask(int activeIndex)
    {
        Task task = activeTasks[activeIndex];

        if (Time.frameCount == task.context.createdFrame)
        {
            return;
        }

        else if (task.context.startTime == -1)
        {
            task.context.framesElapsed = 0;
            task.context.startFrame = Time.frameCount;
            task.context.startTime = TimeUnityless.Time;
        }

        // TODO: Review need for runTask
        bool runTask = true;

        if (task.frameDuration != -1 && task.context.framesElapsed >= task.frameDuration)
        {
            task.complete = true;
            runTask = false;
        }

        if (task.secondDuration != -1 && task.context.TimeElapsed >= task.secondDuration)
        {
            task.complete = true;
            runTask = false;
        }

        if (task.runWithPhysics && task.context.lastFramePhysics == TimeUnityless.PhysicsFrames)
        {
            runTask = false;
        }

        if (runTask)
            task.action(task);

        if (task.complete && activeTasks.Contains(task))
        {
            if (!task.mustFinish)
            {
                activeTasks.RemoveAt(activeIndex);
            }
            else
            {
                if (task.context.framesElapsed >= task.frameDuration)
                {
                    activeTasks.RemoveAt(activeIndex);
                }

                if (task.context.TimeElapsed >= task.secondDuration)
                {
                    activeTasks.RemoveAt(activeIndex);
                }
            }
        }

    }

    private async void Update()
    {

        foreach (Task task in activeTasks)
        {
            task.context.framesElapsed = Time.frameCount - task.context.startFrame;
        }

        if (isSequential)
        {
            if (activeTasks.Count == 0)
            {
                Task task = queuedtasks[0];
                queuedtasks.RemoveAt(0);
                activeTasks.Add(task);
            }
        }
        else
        {
            for (int i = 0; i < queuedtasks.Count; i++)
            {
                Task task = queuedtasks[i];
                activeTasks.Add(task);
            }
            queuedtasks.Clear();
        }

        if (activeTasks.Count == 0)
        {
            return;
        }

        for (int i = 0; i < activeTasks.Count; i++)
        {
            if (runAsynchronously)
            {
                await RunTaskAsync(i);
            }
            else
            {
                RunTask(i);
            }
        }
    }
}
