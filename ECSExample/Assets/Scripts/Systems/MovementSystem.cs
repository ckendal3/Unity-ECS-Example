using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// This System controls the position movement of the entities (cubes)
/// </summary>
public class MovementSystem : JobComponentSystem
{
    [BurstCompile]
    public struct MovementJob : IJobForEach<Translation, Rotation, Speed>
    {
        [ReadOnly] public float DeltaTime;

        // Execute is where the actual logic (system) takes place such as manipulation of data
        public void Execute(ref Translation position, [ReadOnly] ref Rotation rotation, [ReadOnly] ref Speed speed)
        {
            // Moves the cube along its forward direction
            float3 newPos = position.Value + (DeltaTime * speed.Value) * math.forward(rotation.Value);

            position.Value = newPos;
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var moveJob = new MovementJob
        {
            DeltaTime = Time.DeltaTime
        }.Schedule(this, inputDeps);

        return moveJob;
    }
}