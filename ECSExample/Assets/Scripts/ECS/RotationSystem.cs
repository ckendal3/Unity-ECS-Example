using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// This System controls the rotation of the entities (cubes)
/// </summary>
public class RotationSystem : JobComponentSystem
{
    [BurstCompile] // The good stuff
    [RequireComponentTag(typeof(RotationOnlyTag))] // Entity must have a "RotationOnlyTag" to rotate
    public struct RotationJob : IJobProcessComponentData<Rotation, Speed>
    {
        // Time it took to render the last frame
        public float deltaTime;

        // Execute is where the actual logic (system) takes place such as manipulation of data
        public void Execute(ref Rotation rotation, [ReadOnly] ref Speed speed)
        {
            rotation.Value = math.mul((rotation.Value), quaternion.AxisAngle(math.up(), speed.Value * deltaTime));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new RotationJob
        {
            deltaTime = Time.deltaTime // passes the deltaTime into the RotationJob
        }.Schedule(this, inputDeps);

        return job;

    }
}
