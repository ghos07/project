using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI health;
    public TextMeshProUGUI minigames;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health.text = "Health: " + JackGameManager.lives;
        minigames.text = "Minigames: " + MinigameManager.minigamesDone + "/" + MinigameManager.minigamesRequired;
    }
}
