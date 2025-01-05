using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardGame : MonoBehaviour
{
    // All keys
    public List<KeyboardKey> keys;

    // Keys that the player has to press
    public List<KeyboardKey> keysToPress;
    public List<KeyboardKey> keysToPreview;
    // The key currently being previewed
    public KeyboardKey currentPreviewKey;

    // Time to show each key for
    public float basePreviewTime = 1.0f;
    public float previewTime => Mathf.Min((basePreviewTime / (JackGameManager.difficulty * round / 3)) / (keyCount / 3), basePreviewTime) * 1.2f;
    public float previewTimeCurrent = 0f;

    public float playerPressTime = 0.8f;
    public float playerPressTimeCurrent = 1f;

    public int roundCount = 3;
    public int round = 1;
    public int keyCount => 2 + (int)((JackGameManager.difficulty - 1) * 3) + round;

    public bool previewing = false;

    public bool playerCanPress => playerPressTimeCurrent >= playerPressTime;

    public LayerMask keyMask;

    public bool lost = false;

    public void StartRound()
    {
        

        if (round > roundCount)
        {
            if (lost)
            {
                GetComponent<MinigameManager>().FailMinigame();
                return;
            }

            GetComponent<MinigameManager>().CompleteMinigame();
            return;
        }

        for (int i = 0; i < keyCount; i++)
        {
            var key = keys[Random.Range(0, keys.Count)];
            while (key.activeOnDifficulty > JackGameManager.difficulty)
            {
                key = keys[Random.Range(0, keys.Count)];
            }
            keysToPress.Add(key);
        }

        currentPreviewKey = keysToPress[0];
        previewing = true;
        previewTimeCurrent = previewTime / 2f;

        keysToPreview = new List<KeyboardKey>(keysToPress);
    }

    public void ResetGame()
    {
        keysToPress.Clear();
        keysToPreview.Clear();
        round = 1;
        currentPreviewKey = null;
        previewing = false;
        playerPressTimeCurrent = 0f;
        previewTimeCurrent = 0f;
        lost = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (previewing)
        {

            Cursor.lockState = CursorLockMode.Locked;
            previewTimeCurrent += Time.deltaTime;
            if (previewTimeCurrent >= previewTime)
            {
                previewTimeCurrent = 0f;
                currentPreviewKey.Press(previewTime / 1.2f);
                keysToPreview.Remove(currentPreviewKey);
                if (keysToPreview.Count > 0)
                {
                    currentPreviewKey = keysToPreview[0];
                }
                else
                {
                    previewing = false;
                    playerPressTimeCurrent = playerPressTime - previewTime;
                }
            }
        }

        if (!previewing)
        {
            Cursor.lockState = CursorLockMode.Confined;
            playerPressTimeCurrent += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                if (playerCanPress)
                {
                    // Get key under mouse cursor
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, keyMask))
                    {
                        if (hit.collider.gameObject.TryGetComponent(out KeyboardKey key))
                        {
                            key.Press(playerPressTime);
                            if (keysToPress[0] != key) {
                                lost = true;
                            }
                            
                            keysToPress.RemoveAt(0);

                            if (keysToPress.Count == 0)
                            {
                                round++;
                                keysToPress.Clear();
                                StartRound();
                            }

                            playerPressTimeCurrent = 0f;
                        }
                    }
                }
            }
        }
    }
}
