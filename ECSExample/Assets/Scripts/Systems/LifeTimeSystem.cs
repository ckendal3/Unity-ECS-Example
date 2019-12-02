using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// This System controls the lifetime 'timer' of the entities (cubes)
/// </summary>
[UpdateAfter(typeof(MovementSystem))] // Only does this job only after the movement jobs have completed
public class LifeTimeSystem : JobComponentSystem
{
    [BurstCompile]
    public struct UpdateTimeJob : IJobForEach<LifeTime>
    {
        [ReadOnly] public float DeltaTime;

        public void Execute(ref LifeTime lifeTime)
        {
            lifeTime.Value = lifeTime.Value - DeltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {    
        var timeJob = new UpdateTimeJob
        {
            DeltaTime = Time.DeltaTime
        }.Schedule(this, inputDeps);

        return timeJob;
    }

}
