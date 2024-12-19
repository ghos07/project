using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackGameManager : MonoBehaviour
{
    public static JackGameManager Instance;

    public static int score = 0;
    public static int lives = 2;
    public static int level = 1;
    public static float difficulty = 1.0f;

    // Editor button
    [InspectorButton("ProgressLevel")]
    public bool progressLevel;

    public static void ProgressLevel()
    {
        level++;
        difficulty += 1.0f;
        print(level);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CamPPBlend.Instance.locked)
        {
            CamPPBlend.Instance.targetBlendFactor = 0.8f * (1f - lives / 2f);
        }
    }
}
