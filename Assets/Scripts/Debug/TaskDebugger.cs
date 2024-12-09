using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDebugger : MonoBehaviour
{

    [SerializeField] private bool printDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TestSequential()
    {
        Debug.Log("Testing Sequential Tasks");

        Debug.Log("Creating Sequential TaskManager");
        TaskManager sequentialTaskManager = TaskManager.NewTaskManager("Sequential Task Manager");
        sequentialTaskManager.isSequential = true;
        Debug.Log("Creating Task: Say 'Sequential 1' for 2 seconds");
        Task sequentialTask1 = new Task((Task tm) =>
        {
            if (tm.context.TimeElapsed >= 2)
            {
                tm.MarkComplete();
                return;
            }
            Debug.Log("Sequential 1");
        }, "Sequential 1");
        Debug.Log("Creating Task: Say 'Sequential 2' for 2 seconds");
        Task sequentialTask2 = new Task((Task tm) =>
        {
            if (tm.context.TimeElapsed >= 2)
            {
                tm.MarkComplete();
                return;
            }
            Debug.Log("Sequential 2");
            Debug.Log("Time Elapsed: " + tm.context.TimeElapsed);
        }, "Sequential 2");
        Debug.Log("Adding Tasks to Sequential TaskManager");
        sequentialTaskManager.AddTask(sequentialTask1);
        sequentialTaskManager.AddTask(sequentialTask2);
        Debug.Log("Added tasks " + sequentialTask1.name + " and " + sequentialTask2.name + " to " + sequentialTaskManager.taskManagerName);
    }

    public void TestBasic()
    {
        Debug.Log("Grabbing Global TaskManager");
        TaskManager globalTaskManager = TaskManager.GlobalInstance;
        Debug.Log("Global TaskManager: " + globalTaskManager.taskManagerName + ", attached to " + globalTaskManager.gameObject.name + ", " + globalTaskManager.gameObject);

        Debug.Log("Creating Task: Say Hello for 5 frames");
        Task sayHelloTask = new Task((Task tm) =>
        {
            if (tm.context.framesElapsed >= 5)
            {
                tm.MarkComplete();
                return;
            }
            Debug.Log("Hello!");
            Debug.Log("Frames Elapsed: " + tm.context.framesElapsed);
        }, "Hello For 5 Frames");
        Debug.Log("Adding Task to Global TaskManager");
        globalTaskManager.AddTask(sayHelloTask);
        Debug.Log("Added task " + sayHelloTask.name + " to " + globalTaskManager.taskManagerName);
    }

    public void TestAsync()
    {
        Debug.Log("Testing Async Tasks");

        Debug.Log("Creating Async TaskManager");
        TaskManager asyncTaskManager = TaskManager.NewTaskManager("Async Task Manager");
        asyncTaskManager.runAsynchronously = true;
        Debug.Log("Creating Tasks. Async2 should finish before Async1");
        Debug.Log("Creating Task: Say 'Async 1' for 2 seconds");
        Task asyncTask1 = new Task((Task tm) =>
        {
            if (tm.context.TimeElapsed >= 2)
            {
                tm.MarkComplete();
                return;
            }
            // Stuff to make Async Task 1 take longer
            for (int i = 0; i < 1000000000; i++)
            {
                // Do nothing
            }
            Debug.Log("Async 1");
        }, "Async 1");
        Debug.Log("Creating Task: Say 'Async 2' for 2 seconds");
        Task asyncTask2 = new Task((Task tm) =>
        {
            if (tm.context.TimeElapsed >= 2)
            {
                tm.MarkComplete();
                return;
            }
            Debug.Log("Async 2");
        }, "Async 2");
        Debug.Log("Adding Tasks to Async TaskManager");
        asyncTaskManager.AddTask(asyncTask1);
        asyncTaskManager.AddTask(asyncTask2);
        Debug.Log("Added tasks " + asyncTask1.name + " and " + asyncTask2.name + " to " + asyncTaskManager.taskManagerName);
    }

    // Update is called once per frame
    void Update()
    {

        if (!printDebug)
        {
            return;
        }
        
        // Grab all TaskManagers in the scene
        TaskManager[] taskManagers = FindObjectsOfType<TaskManager>();

        string output = "";

        // Loop through each TaskManager
        foreach (TaskManager taskManager in taskManagers)
        {
            // Loop through each task in the TaskManager
            foreach (Task task in taskManager.ActiveTasks)
            {
                // Print the task's name with some debug info
                string line = ("Task: " + task.name + " - Complete: " + task.complete + " - Time Elapsed: " + task.context.TimeElapsed + " - Frames Elapsed: " + task.context.framesElapsed);
                output += line + "\n";
            }
        }

        if (output != "")
        {
            Debug.Log(output);
        }

    }
}
