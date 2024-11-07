using System.Collections;
using System.Collections.Generic;
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
            if (spawnFragmentsWithWorldScale)
            {
                objCopy = Instantiate(obj, transform.position, obj.transform.rotation * transform.rotation);
                objCopy.transform.parent = transform;
            }
            else
            {
                objCopy = Instantiate(obj, obj.transform.position, obj.transform.rotation, transform);
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
                    print(objCopy.name + " is being pushed");
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

                // Calculate the average point by dividing the sum by the number of points
                if (pointCount > 0)
                {
                    averagePoint /= pointCount;
                }

                FragmentExplosionSourceVector3 = averagePoint;
                OnDestroyedInternal(new(averagePoint));

                useVector3FragmentExplosionSource = false;
                return;
                if (collidingRigidbody != null)
                {
                    collidingRigidbody.velocity = Vector3.zero;
                    collidingRigidbody.angularVelocity = Vector3.zero;
                    collidingRigidbody.AddForce(collidingObjectVelocity, ForceMode.VelocityChange);
                    collidingRigidbody.AddTorque(collidingObjectAngularVelocity, ForceMode.VelocityChange);
                }
            }
        }
    }
}
