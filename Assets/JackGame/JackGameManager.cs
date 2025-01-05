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

    public static float danger = 0.0f;

    public ModifierManager modifierManager;

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
        ModifierManager modifierManager = ModifierManager.GetModifierManager(gameObject);
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
            CamPPBlend.Instance.targetBlendFactor = 0.8f * (1f - lives / 2f) * (danger + 1);
        }

        danger = modifierManager.GetValue(Modifier.DangerModifier);

        if (lives <= 0)
        {
            // Game over
        }

        if (danger > 10f)
        {
            if (Random.Range(0, 100) < danger / 2f)
            {
                
            }
        }
    }


}
