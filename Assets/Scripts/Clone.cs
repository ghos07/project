using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    public void Copy()
    {
        Instantiate(gameObject);
    }
}
