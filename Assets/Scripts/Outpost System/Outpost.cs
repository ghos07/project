using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    private int population;
    private int maxPermanentPopulation;
    private int maxTemporaryPopulation;
    private int maxPopulation => maxPermanentPopulation + maxTemporaryPopulation;
    public List<GameObject> inhabitants;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
