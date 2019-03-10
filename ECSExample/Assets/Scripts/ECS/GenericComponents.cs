using Unity.Entities;

/*
 * This class is strictly for generalized components
 */

/// <summary>
/// The lifetime component for determining when to destroy an entity.
/// </summary>
public struct LifeTime : IComponentData
{
    public float Value;
}

/// <summary>
/// The speed component is used for determing how fast an object moves and/or rotates.
/// </summary>
public struct Speed : IComponentData
{
    public float Value;
}

/// <summary>
/// The RotationOnlyTag component is used exclsuively for the tagging (identifying a subset) of related entities.
/// </summary>
public struct RotationOnlyTag : IComponentData { }
