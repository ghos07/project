using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 5.0f;
    public float force = 5.0f;
    public float damage = 50.0f;

    public DamageType damageType = DamageType.Physical;

    private void Start()
    {
        Explode(transform.position, radius, force, new DamageContext(damage, 0, damageType, gameObject));
        Destroy(this);
    }

    public static void Explode(Vector3 position, float radius, float force, DamageContext damage)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, position, radius);
            }

            HealthComponent hc = hit.GetComponent<HealthComponent>();
            if (hc != null)
            {
                hc.ApplyDamage(damage);
            }
        }
    }

    public static void Explode(Vector3 position, float radius, float force, float damage)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (Collider hit in colliders)
        {
            float appliedForce = force;

            Rigidbody rb = hit.GetComponent<Rigidbody>();

            MovementBehaviour mb = hit.GetComponent<MovementBehaviour>();

            if (TagManager.HasTag(hit.gameObject, Tag.NearZeroMass))
            {
                appliedForce = force * GameManager.minisculeForceMultiplier;
            }

            if (mb != null && rb != null)
            {
                ModifierManager.GetModifierManager(hit.gameObject).QueueModifierAction(() => {
                    ModifierManager.GetModifierManager(hit.gameObject).AddValue(Modifier.DragModifier, .05f);
                    rb.AddForce(Vector3.up * appliedForce, ForceMode.VelocityChange);
                    rb.AddExplosionForce(appliedForce, position, radius);
                }, 0);
            }else if (rb != null)
            {
                rb.AddExplosionForce(appliedForce, position, radius);
            }

            HealthComponent hc = hit.GetComponent<HealthComponent>();
            if (hc != null)
            {
                hc.ApplyDamage(new(damage));
            }
        }
    }
}
