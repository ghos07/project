using System;
using UnityEngine;

public class TimeUnityless
{
    /// <summary>
    /// Time in seconds since January 1, 1970.
    /// </summary>
    public static float Time { get { Initialize();  return ((float)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds) / 1000; } }

    /// <summary>
    /// Time in milliseconds since January 1, 1970. Use to avoid float precision issues.
    /// </summary>
    public static long TimeMilliseconds { get { Initialize();  return (long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds); } }

    private static int _physicsFrames = 0;

    /// <summary>
    /// Physics frames elapsed since TimeUnityless was initialized. Do not use as a frame counter, only use for relative time comparisons.
    /// </summary>
    public static int PhysicsFrames {  get { Initialize();  return _physicsFrames; } private set { _physicsFrames = value; } }

    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        _isInitialized = true;

        _physicsFrames = 0;

        //Create a game object in DontDestroyOnLoad that tracks physics time:
        GameObject timeKeeper = new GameObject("TimeKeeper");
        timeKeeper.AddComponent<TaskManager>();
        UnityEngine.Object.DontDestroyOnLoad(timeKeeper);

        timeKeeper.GetComponent<TaskManager>().AddTask(new Task((Task tm) =>
        {
            _physicsFrames++;
        }, "Physics Frame Counter"));
    }

}
