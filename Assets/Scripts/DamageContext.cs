using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public readonly struct DamageContext
{
    public static DamageContext Empty => new DamageContext(0, 0, DamageType.True, null, null, Vector3.negativeInfinity, Vector3.negativeInfinity);

    public readonly float Damage;
    public readonly float Penetration;
    public readonly DamageType DamageType;
    public readonly GameObject Root;
    public readonly GameObject Source;
    public readonly Vector3 RootLocation;
    public readonly Vector3 SourceLocation;

    public DamageContext(float damage, float penetration, DamageType damageType, GameObject root, GameObject source, Vector3 rootLocation, Vector3 sourceLocation)
    {
        Damage = damage;
        Penetration = penetration;
        DamageType = damageType;
        Root = root;
        Source = source;
        RootLocation = rootLocation;
        SourceLocation = sourceLocation;
    }

    public DamageContext(float damage) : this(damage, 0, DamageType.True, null, null, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, GameObject root) : this(damage, 0, DamageType.True, root, root, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, GameObject root, GameObject source) : this(damage, 0, DamageType.True, root, root, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, DamageType damageType) : this(damage, 0, damageType, null, null, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, DamageType damageType, GameObject root) : this(damage, 0, damageType, root, root, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, DamageType damageType, GameObject root, GameObject source) : this(damage, 0, damageType, root, source, root.transform.position, source.transform.position) { }

    public DamageContext(float damage, float penetration) : this(damage, penetration, DamageType.True, null, null, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, float penetration, GameObject root) : this(damage, penetration, DamageType.True, root, root, root.transform.position, root.transform.position) { }

    public DamageContext(float damage, float penetration, GameObject root, GameObject source) : this(damage, penetration, DamageType.True, root, source, root.transform.position, source.transform.position) { }

    public DamageContext(float damage, float penetration, DamageType damageType) : this(damage, penetration, damageType, null, null, Vector3.negativeInfinity, Vector3.negativeInfinity) { }

    public DamageContext(float damage, float penetration, DamageType damageType, GameObject root) : this(damage, penetration, damageType, root, root, root.transform.position, root.transform.position) { }

    public DamageContext(float damage, float penetration, DamageType damageType, GameObject root, GameObject source) : this(damage, penetration, damageType, root, source, root.transform.position, source.transform.position) { }

    public DamageContext(Vector3 rootLocation, Vector3 sourceLocation) : this(0, 0, DamageType.True, null, null, rootLocation, sourceLocation) { }

    public DamageContext(Vector3 location) : this(0, 0, DamageType.True, null, null, location, location) { }

    public DamageContext(Vector3 location, GameObject root) : this(0, 0, DamageType.True, root, root, location, location) { }

    public DamageContext(float damage, Vector3 location) : this(damage, 0, DamageType.True, null, null, location, location) { }

    public DamageContext WithDamage(float damage) => new DamageContext(damage, Penetration, DamageType, Root, Source, RootLocation, SourceLocation);
    public DamageContext WithPenetration(float penetration) => new DamageContext(Damage, penetration, DamageType, Root, Source, RootLocation, SourceLocation);
    public DamageContext WithDamageType(DamageType damageType) => new DamageContext(Damage, Penetration, damageType, Root, Source, RootLocation, SourceLocation);
    public DamageContext WithRoot(GameObject root) => new DamageContext(Damage, Penetration, DamageType, root, Source, root.transform.position, SourceLocation);
    public DamageContext WithSource(GameObject source) => new DamageContext(Damage, Penetration, DamageType, Root, source, RootLocation, source.transform.position);
}
