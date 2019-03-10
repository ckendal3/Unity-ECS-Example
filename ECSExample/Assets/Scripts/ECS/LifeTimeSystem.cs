using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

/// <summary>
/// This System controls the lifetime 'timer' of the entities (cubes)
/// </summary>
[UpdateAfter(typeof(MovementSystem))] // Only does this job only after the movement jobs have completed
public class LifeTimeSystem : JobComponentSystem
{
    [BurstCompile]
    public struct LifeTimeJob : IJobProcessComponentData<LifeTime>
    {
        public float deltaTime; // Time it took to render the last frame

        public void Execute(ref LifeTime lifeTime)
        {
            // decrease the lifetime on the entity
            lifeTime.Value = lifeTime.Value - deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new LifeTimeJob
        {
            deltaTime = Time.deltaTime
        };

        return job.Schedule(this, inputDeps);
    }

}
