using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]

public class Destructible : MonoBehaviour
{
    private static List<Destructible> destructionQueue = new();
    private static bool destructionOccurredThisFrame = false;
    private static int frameOfLastDestruction = -1;

    public GameObject[] disappearOnDestroy;
    public GameObject[] createOnDestroy;

    public bool spawnFragmentsWithWorldScale = false;
    public bool noParent = false;
    public float fragmentScaleMultiplier = 1.0f;

    public bool destroySelf = false;
    public bool DestroyCreated = false;
    public float destroyCreatedTimer = 5.0f;

    public bool destroyFromForce = true;
    public float forceThreshold = 5.0f;

    public bool inheritVelocity = true;

    public bool pushFragmentsOnDestroy = true;
    public bool explodeFragmentsOnDestroy = true;

    public bool minisculeFragmentForces = false;

    public float pushForce = 5.0f;
    private float realPushForce => minisculeFragmentForces ? pushForce * GameManager.minisculeForceMultiplier : pushForce;
    public float fragmentExplosionForce = 5.0f;
    private float realFragmentExplosionForce => minisculeFragmentForces ? fragmentExplosionForce * GameManager.minisculeForceMultiplier : fragmentExplosionForce;
    public float fragmentExplosionRadius = 5.0f;

    public bool explodeOnDestroy = false;
    public float explosionDamage = 50.0f;
    public float explosionForce = 5.0f;
    public float explosionRadius = 5.0f;
    public Transform explosionSource;

    public bool applyDestructionPerFrameLimit = false;

    private bool useVector3FragmentExplosionSource = false;

    private Vector3[] savedVelocities = {Vector3.zero, Vector3.zero, Vector3.zero};

    [SerializeField] private Transform fragmentExplosionSource;
    private Vector3 fragmentExplosionSourceVector3storage = Vector3.zero;
    
    private bool destroyedThisFrame = false;

    private bool destroyNextFrame = false;
    private DamageContext destroyNextFrameContext;

    private List<System.Action<GameObject>> fragmentProcedures = new();

    private static TaskManager globalDestructibleTaskManager;
    public static TaskManager GlobalDestructibleTaskManager
    {
        get
        {
            // Check if there is a stored global task manager instance already created and active in the scene
            if (globalDestructibleTaskManager == null || !globalDestructibleTaskManager.isGlobal || !globalDestructibleTaskManager.gameObject.activeInHierarchy)
            {
                // If there is no stored global task manager instance, check if there is a global task manager in the scene
                TaskManager[] taskManagers = FindObjectsOfType<TaskManager>();
                foreach (TaskManager taskManager in taskManagers)
                {
                    if (taskManager.isGlobal)
                    {
                        globalDestructibleTaskManager = taskManager;
                        return globalDestructibleTaskManager;
                    }
                }

                // If there is no global task manager in the scene, create one
                GameObject taskManagerObject = new GameObject("Task Manager");
                globalDestructibleTaskManager = taskManagerObject.AddComponent<TaskManager>();
                globalDestructibleTaskManager.isGlobal = true;
                GlobalDestructibleTaskManager.GetComponent<TaskManager>().runAsynchronously = false;
            }

            return globalDestructibleTaskManager;
        }
    }

    public void AddFragmentProcedure(System.Action<GameObject> procedure)
    {
        fragmentProcedures.Add(procedure);
    }

    private void ApplyProcedures(GameObject fragment)
    {
        foreach (System.Action<GameObject> procedure in fragmentProcedures)
        {
            procedure(fragment);
        }
    }

    public Vector3 FragmentExplosionSourceVector3
    {
        get
        {
            if (!useVector3FragmentExplosionSource)
            {
                return fragmentExplosionSource == null? transform.position : fragmentExplosionSource.position;
            }
            else
            {
                return fragmentExplosionSourceVector3storage;
            }
        }
        set
        {
            fragmentExplosionSourceVector3storage = value;
        }
    }

    public event System.Action OnDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        if (fragmentExplosionSource != null)
        {
            FragmentExplosionSourceVector3 = fragmentExplosionSource.position;
        }
        HealthComponent.GetHealthComponent(gameObject).OnDeath += (DamageContext dc) => OnDestroyedInternal(dc);

        if (!TryGetComponent(out Rigidbody rb))
        {
            inheritVelocity = false;
        }

        if (explosionSource == null)
        {
            explosionSource = transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        destroyedThisFrame = false;

        if (destroyNextFrame)
        {
            if (destructionQueue[0] == this)
            {
                if (frameOfLastDestruction == Time.frameCount)
                {
                    return;
                }
                destructionOccurredThisFrame = false;
                OnDestroyedInternal(destroyNextFrameContext);
                destructionQueue.RemoveAt(0);
                destroyNextFrame = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (inheritVelocity)
        {
            savedVelocities[1] = savedVelocities[0];
            savedVelocities[0] = GetComponent<Rigidbody>().velocity;
        }
    }

    private void OnDestroyedInternal(DamageContext context)
    {
        if (isActiveAndEnabled == false)
        {
            return;
        }

        if (applyDestructionPerFrameLimit && destructionOccurredThisFrame)
        {
            if (destructionQueue.Contains(this))
            {
                return;
            }
            destructionQueue.Add(this);
            destroyNextFrame = true;
            destroyNextFrameContext = context;
            return;
        }

        destructionOccurredThisFrame = true;
        frameOfLastDestruction = Time.frameCount;

        if (context.SourceLocation.Equals(Vector3.negativeInfinity) || context.SourceLocation.Equals(Vector3.positiveInfinity))
        {
            context = new DamageContext(context.Damage, context.Penetration, context.DamageType, gameObject, gameObject, transform.position, transform.position);
        }

        foreach (GameObject obj in disappearOnDestroy)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in createOnDestroy)
        {
            GameObject objCopy;

            Transform parent = noParent ? null : transform;

            if (spawnFragmentsWithWorldScale)
            {
                objCopy = Instantiate(obj, transform.position, obj.transform.rotation * transform.rotation);
                objCopy.transform.parent = parent;
            }
            else
            {
                objCopy = Instantiate(obj, obj.transform.position, obj.transform.rotation, parent);
            }
            objCopy.transform.SetParent(null);
            objCopy.transform.localScale = objCopy.transform.localScale * fragmentScaleMultiplier;
            objCopy.SetActive(true);
            
            if (DestroyCreated)
            {
                TimerComponent timer = objCopy.AddComponent<TimerComponent>();
                timer.duration = destroyCreatedTimer + Random.Range(0, 1.0f);
                timer.destroyGameObjectOnComplete = true;

                if (objCopy.TryGetComponent(out RigidBodyCluster cluster))
                {
                    foreach (Rigidbody babyrb in cluster.rigidbodies)
                    {
                        timer = babyrb.gameObject.AddComponent<TimerComponent>();
                        timer.duration = destroyCreatedTimer + Random.Range(0, 1.0f);
                        timer.destroyGameObjectOnComplete = true;
                    }
                }
            }

            if (inheritVelocity)
            {
                if (objCopy.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = GetComponent<Rigidbody>().velocity;
                }

                if (objCopy.TryGetComponent(out RigidBodyCluster cluster))
                {
                    foreach (Rigidbody babyrb in cluster.rigidbodies)
                    {
                        babyrb.velocity = GetComponent<Rigidbody>().velocity;
                    }
                }
            }

            if (pushFragmentsOnDestroy)
            {
                if (objCopy.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(realPushForce * (transform.position - context.SourceLocation), ForceMode.Force);
                }

                if (objCopy.TryGetComponent(out RigidBodyCluster cluster))
                {
                    foreach (Rigidbody babyrb in cluster.rigidbodies)
                    {
                        babyrb.AddForce(realPushForce * (transform.position - context.SourceLocation), ForceMode.Force);
                    }
                }
            }

            if (explodeFragmentsOnDestroy)
            {
                if (objCopy.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(realFragmentExplosionForce, FragmentExplosionSourceVector3, fragmentExplosionRadius);
                }

                if (objCopy.TryGetComponent(out RigidBodyCluster cluster))
                {
                    foreach (Rigidbody babyrb in cluster.rigidbodies)
                    {
                        babyrb.AddExplosionForce(realFragmentExplosionForce, FragmentExplosionSourceVector3, fragmentExplosionRadius);
                    }
                }
            }

            if (explodeOnDestroy)
            {
                Explosion.Explode(explosionSource.position, explosionRadius, explosionForce, explosionDamage);
            }
        }

        if (destroySelf)
        {
            Destroy(gameObject);
        }

        OnDestroyed?.Invoke();
    }

    private void ForceDestroy()
    {
        OnDestroyedInternal(DamageContext.Empty);
    }

    public void Repair()
    {
        foreach (GameObject obj in disappearOnDestroy)
        {
            obj.SetActive(true);
        }

        HealthComponent.GetHealthComponent(gameObject).Health = HealthComponent.GetHealthComponent(gameObject).MaxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyFromForce)
        {
            if (destroyedThisFrame)
            {
                return;
            }

            bool shouldDestroy = false;
            shouldDestroy = collision.impulse.magnitude > forceThreshold;

            // Check if the other object is also destructible and if it has a higher force threshold.
            Destructible otherDestructible = collision.gameObject.GetComponent<Destructible>();
            if (otherDestructible != null)
            {
                shouldDestroy = shouldDestroy && otherDestructible.forceThreshold > forceThreshold;
            }



            if (shouldDestroy)
            {
                destroyedThisFrame = true;

                Rigidbody collidingRigidbody = collision.rigidbody;
                Vector3 collidingObjectVelocity = collidingRigidbody != null ? collidingRigidbody.velocity : Vector3.zero;

                Vector3 collidingObjectAngularVelocity = collidingRigidbody != null ? collidingRigidbody.angularVelocity : Vector3.zero;


                if (TryGetComponent(out Rigidbody rb))
                {
                    if (inheritVelocity)
                    {
                        if (savedVelocities[1].magnitude > 0.1f && !(rb.velocity.magnitude > 10f && rb.velocity.magnitude > savedVelocities[1].magnitude))
                            rb.velocity = savedVelocities[1];
                    }
                }
                useVector3FragmentExplosionSource = true;

                Vector3 averagePoint = Vector3.zero;
                int pointCount = collision.contacts.Length;

                foreach (ContactPoint contact in collision.contacts)
                {
                    averagePoint += contact.point;
                }

                Vector3 impulse = collision.impulse;

                // Both participants of a collision see the same impulse, so we need to flip it for one of them.
                if (Vector3.Dot(collision.GetContact(0).normal, impulse) < 0f)
                    impulse *= -1f;

                // Calculate the average point by dividing the sum by the number of points
                if (pointCount > 0)
                {
                    averagePoint /= pointCount;
                }

                FragmentExplosionSourceVector3 = averagePoint;
                OnDestroyedInternal(new(averagePoint));

                useVector3FragmentExplosionSource = false;

                GlobalDestructibleTaskManager.DoNextFrame(() =>
                {
                    if (collidingRigidbody != null)
                    {
                        collidingObjectVelocity = collidingRigidbody.velocity - impulse / collidingRigidbody.mass;

                        collidingRigidbody.velocity = collidingObjectVelocity;
                        collidingRigidbody.angularVelocity = collidingObjectAngularVelocity;

                        collidingRigidbody.AddForceAtPosition(-collision.impulse.normalized * forceThreshold, averagePoint, ForceMode.Impulse);

                    }
                }, true);

            }
        }
    }
}
