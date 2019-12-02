using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

/// <summary>
/// This System destroys the entities (cubes) if their lifetimes are over
/// </summary>
public class DestroySystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem Buffer;

    [BurstCompile]
    public struct LifeDestroyJob : IJobForEachWithEntity<LifeTime>
    {
        public EntityCommandBuffer.Concurrent Buffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref LifeTime lifeTime)
        {
            // if the lifetimes are less than 0 - don't try an equality on floats (bad: lifetime.Value == 0 || lifetime.Value <= 0)
            if (lifeTime.Value < 0)
            {
                Buffer.DestroyEntity(index, entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var lifeDestroy = new LifeDestroyJob
        {
            Buffer = Buffer.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        Buffer.AddJobHandleForProducer(lifeDestroy);

        return lifeDestroy;
    }

    protected override void OnCreate()
    {
        Buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}