using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GameManager
{
    public readonly static float minisculeForceMultiplier = 0.0000001f;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    private static GameManager instance;

    public Dictionary<string, Item> items = new();

    public GameObject player;

    public PlayerController playerController;

    public CameraController cameraController;

    public static T GetComponentEnsured<T>(GameObject gameObject) where T : MonoBehaviour
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            return gameObject.AddComponent<T>();
        }
        return component;
    }
}
