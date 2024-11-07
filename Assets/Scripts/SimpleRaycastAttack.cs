using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Only to be used for testing purposes. Simply raycasts forward and attacks anything it hits.
/// </summary>
public class SimpleRaycastAttack : MonoBehaviour
{
    public float attackRange = 10f;
    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(Vector3 position, Vector3 rotation)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, rotation, out hit, 10))
        {
            HealthComponent healthComponent = HealthComponent.GetHealthComponent(hit.collider.gameObject);
            if (healthComponent != null)
            {
                healthComponent.ApplyDamage(new(damage, this.gameObject));
            }
        }
    }
}
