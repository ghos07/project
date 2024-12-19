using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameManager : MonoBehaviour
{
    public static List<MinigameManager> minigames = new List<MinigameManager>();
    public static int minigamesCompleted = 0;
    public static int minigamesFailed = 0;
    public static int minigamesTotal = 0;
    public static int baseMinigamesRequired = 3;
    public static int minigamesRequired = 3;
    public static int minigamesDone => minigamesCompleted + minigamesFailed;

    public bool completed = false;
    public bool active = false;
    public bool minigameEnabled = true;
    public MinigameNames minigameName = MinigameNames.JackBox;
    public List<MonoBehaviour> minigameFunctionalities = new List<MonoBehaviour>();

    public UnityEvent OnActivate = new UnityEvent();
    public UnityEvent OnComplete = new UnityEvent();
    public UnityEvent OnFail = new UnityEvent();
    public UnityEvent OnDeactivate =  new UnityEvent();
    public UnityEvent OnReset = new UnityEvent();

    public bool EscapeToExit = true;
    public bool resetOnExit = true;

    public bool damageOnFail = true;
    public int damage = 1;

    public void Start()
    {
        SetActive(active);
    }

    public void Awake()
    {
        minigames.Add(this);
    }

    public void SetEnabled(bool enabled)
    {
        minigameEnabled = enabled;
        if (active && !enabled)
        {
            SetActive(false);
        }
    }

    public void SetActive(bool active)
    {
        if (!minigameEnabled)
        {
            return;
        }

        this.active = active;
        foreach (var functionality in minigameFunctionalities)
        {
            functionality.enabled = active;
        }
        if (active)
        {
            OnActivate.Invoke();
            AmbientLightsController.Instance.SetIntensity(0.0f);
        }
        else
        {
            OnDeactivate.Invoke();
            AmbientLightsController.Instance.SetIntensity(1.0f);
        }
    }

    public void CompleteMinigame()
    {
        minigamesCompleted++;
        completed = true;
        SetActive(false);
        SetEnabled(false);

        OnComplete.Invoke();
    }

    public void FailMinigame()
    {
        if (damageOnFail)
        {
            JackGameManager.lives -= damage;
        }

        minigamesFailed++;
        SetActive(false);
        SetEnabled(false);

        OnFail.Invoke();
    }

    public static void ResetMinigames(List<MinigameManager> exclude = null)
    {
        minigamesCompleted = 0;
        minigamesFailed = 0;
        foreach (var minigame in minigames)
        {
            if (exclude != null)
            {
                if (exclude.Contains(minigame))
                {
                    continue;
                }
            }
            minigame.SetActive(false);
            foreach (var functionality in minigame.minigameFunctionalities)
            {
                functionality.enabled = false;
            }
        }
        minigamesTotal = minigames.Count;

        foreach (var minigame in minigames)
        {
            if (exclude != null)
            {
                if (exclude.Contains(minigame))
                {
                    continue;
                }
            }
            minigame.OnReset.Invoke();
        }
    }

    public void ResetMinigame()
    {
        SetActive(false);
        
        OnReset.Invoke();
    }

    public static bool CheckMinigames()
    {
        return minigamesCompleted >= minigamesRequired;
    }

    public static MinigameManager GetMinigame(MinigameNames minigameName)
    {
        foreach (var minigame in minigames)
        {
            if (minigame.minigameName == minigameName)
            {
                return minigame;
            }
        }
        return null;
    }

    public static void SetActiveMinigame(MinigameNames minigameName, bool active)
    {
        MinigameManager minigame = GetMinigame(minigameName);
        if (minigame != null)
        {
            minigame.SetActive(active);
        }
    }

    public void Update()
    {
        if (EscapeToExit && Input.GetKeyDown(KeyCode.Escape) && active)
        {
            if (resetOnExit)
            {
                ResetMinigames();
            }
            else
            {
                SetActive(false);
            }
        }
    }
}
