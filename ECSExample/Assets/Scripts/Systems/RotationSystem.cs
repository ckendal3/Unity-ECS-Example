using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// This System controls the rotation of the entities (cubes)
/// </summary>
[UpdateAfter(typeof(MovementSystem))]
public class RotationSystem : JobComponentSystem
{
    [BurstCompile]
    [RequireComponentTag(typeof(RotationOnlyTag))] // Entity must have a "RotationOnlyTag" to rotate
    public struct RotationJob : IJobForEach<Rotation, Speed>
    {
        // Time it took to render the last frame
        public float DeltaTime;

        // Execute is where the actual logic (system) takes place such as manipulation of data
        public void Execute(ref Rotation rotation, [ReadOnly] ref Speed speed)
        {
            rotation.Value = math.mul((rotation.Value), quaternion.AxisAngle(math.up(), speed.Value * DeltaTime));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new RotationJob
        {
            DeltaTime = Time.DeltaTime
        }.Schedule(this, inputDeps);

        return job;
    }
}
