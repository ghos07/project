using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TimerComponent : MonoBehaviour
{
    public float duration = 1.0f;
    public bool repeat = false;
    public bool destroyGameObjectOnComplete = false;

    public UnityEvent OnComplete;

    private bool isComplete = false;
    public float timeLeft = 1.0f;

    private void Start()
    {
        timeLeft = duration;
    }

    private void Update()
    {
        if (isComplete) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            Complete();
        }
    }

    private void Complete()
    {
        if (isComplete) return;

        isComplete = true;

        OnComplete?.Invoke();

        if (repeat)
        {
            timeLeft = duration;
            isComplete = false;
            return;
        }

        if (destroyGameObjectOnComplete)
        {
            Destroy(gameObject);
        }
        Destroy(this);
    }
}

public class Timer
{
    public float duration { get; private set; }
    private float endTime;
    public bool reTrigger { get; set; }

    public Timer(float duration)
    {
        this.duration = duration;
        this.endTime = Time.time + duration;
    }

    public Timer() : this(0) { }

    public void Trigger()
    {
        this.endTime = Time.time + duration;
    }

    public bool IsComplete()
    {
        if (Time.time >= endTime)
        {
            if (reTrigger)
            {
                endTime = Time.time + duration;
            }
            return true;
        }
        else if (reTrigger)
        {
            endTime = Time.time + duration;
        }
        return false;
    }

    public static implicit operator bool(Timer timer)
    {
        return timer.IsComplete();
    }
}
