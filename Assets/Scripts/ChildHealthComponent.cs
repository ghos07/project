using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildHealthComponent : MonoBehaviour
{
    public HealthComponent ParentHealthComponent => parentHealthComponent;
    [SerializeField] private HealthComponent parentHealthComponent;
}
