using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;

    private bool isAlive = true;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            OnHealthChanged?.Invoke();
        }
    }

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            OnHealthChanged?.Invoke();
        }
    }

    private DamageContext lastDamageContext = DamageContext.Empty;

    public float HealthPercentage => health / maxHealth;

    public bool IsAlive => health > 0;

    public delegate DamageContext DamageProcess(DamageContext damageContext);
    private List<DamageProcess> damageProcesses;

    public static readonly string DAMAGE_TAKEN_MODIFIER = "damageTakenModifier";

    public void AddDamageProcessor(DamageProcess processor)
    {
        damageProcesses.Add(processor);
    }

    /// <summary>
    /// Called when the entity's health changes.
    /// </summary>
    public event System.Action OnHealthChanged;

    /// <summary>
    /// Called when the entity is healed.
    /// </summary>
    public event System.Action OnHeal;

    /// <summary>
    /// Called when the entity dies.
    /// </summary>
    public event System.Action<DamageContext> OnDeath;

    /// <summary>
    /// Called when the entity takes damage.
    /// </summary>
    public event System.Action<DamageContext> OnDamageTaken;

    public static HealthComponent GetHealthComponent(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out HealthComponent healthComponent))
        {
            return healthComponent;
        }
        
        if (gameObject.TryGetComponent(out ChildHealthComponent childHealthComponent))
        {
            return childHealthComponent.ParentHealthComponent;
        }

        return null;
    }

    // Start is called before the first frame update
    void Awake()
    {
        damageProcesses ??= new()
            {
                (damage) => { return damage; }
            };
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            health = 0;
        }

        CheckDeath(lastDamageContext);
        lastDamageContext = DamageContext.Empty;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        OnHealthChanged?.Invoke();
    }

    public void ForceHeal(float amount)
    {
        health += amount;
        OnHealthChanged?.Invoke();
    }

    public void ApplyDamage(DamageContext damageContext)
    {
        lastDamageContext = damageContext;

        float oldHealth = health;
        foreach (var process in damageProcesses)
        {
            damageContext = process(damageContext);
        }

        health -= damageContext.Damage;

        OnHealthChanged?.Invoke();
        OnDamageTaken?.Invoke(damageContext);
    }

    public static bool ApplyDamage(GameObject gameObject, DamageContext damageContext)
    {
        HealthComponent healthComponent = GetHealthComponent(gameObject);
        if (healthComponent == null)
        {
            return false;
        }

        healthComponent.ApplyDamage(damageContext);
        return true;
    }

    private void CheckDeath(DamageContext damageContext)
    {
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            OnDeath?.Invoke(damageContext);
        }
        else if (health > 0)
        {
            isAlive = true;
        }
    }
}
