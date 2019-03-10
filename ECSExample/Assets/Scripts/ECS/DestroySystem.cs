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
[UpdateAfter(typeof(LifeTimeSystem))] // Only does this job only after the movement jobs have completed
public class DestroySystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem entityCommandBuffer;

    [BurstCompile]
    public struct DestroyJob : IJobProcessComponentDataWithEntity<LifeTime>
    {
        // Creates a queue of commands to execute after the job is over (these commands are done on the main thread)
        [ReadOnly] public EntityCommandBuffer CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref LifeTime lifeTime)
        {
            // if the lifetimes are less than 0
            if (lifeTime.Value <= 0)
            {
                // Queue up command to destroy the entity (cube)
                CommandBuffer.DestroyEntity(entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroyJob
        {
            CommandBuffer = entityCommandBuffer.CreateCommandBuffer() // Create the command buffer that queues commands
        }.ScheduleSingle(this, inputDeps);

        // Must be added now as of 0.0.21
        entityCommandBuffer.AddJobHandleForProducer(job);

        return job;
    }

    protected override void OnCreateManager()
    {
        // Prevents us from creating this every frame
        entityCommandBuffer = World.GetOrCreateManager<EndSimulationEntityCommandBufferSystem>();
    }

}