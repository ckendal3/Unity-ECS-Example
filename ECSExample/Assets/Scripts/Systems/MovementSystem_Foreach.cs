using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// This is an example of the new Foreach that can be used inside of the JobComponentSystem
/// It has functions for .WithoutBurst() and .Run()
/// [DisableAutoCreation] makes it so the system is not auto-created. The user has to create it
/// as this is the same as MovementSystem.cs, we do not want this to update.
/// </summary>

[DisableAutoCreation]
public class MovementSystem_Foreach : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float DeltaTime = Time.DeltaTime;

        JobHandle movementHandle = Entities.ForEach((ref Translation position, in Rotation rotation, in Speed speed) =>
        {
            // Moves the cube along its forward direction
            float3 newPos = position.Value + (DeltaTime * speed.Value) * math.forward(rotation.Value);

            // Set the new position
            position.Value = newPos;
        }).Schedule(inputDeps);

        return movementHandle;
    }
}