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
[UpdateAfter(typeof(LifeTimeSystem))] // Only does this job only after the lifetime jobs have completed
public class DestroySystem : JobComponentSystem
{
    // "EndSimulationBarrier automatically executes any commands in this buffer when the System runs at the end of a frame" - Unity ECS Sample Repo doc
    EndSimulationEntityCommandBufferSystem entityCommandBuffer;

    [BurstCompile]
    public struct DestroyJob : IJobProcessComponentDataWithEntity<LifeTime>
    {
        // Creates a queue of commands to execute after the job is over (these commands are done on the main thread)
        [ReadOnly] public EntityCommandBuffer CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref LifeTime lifeTime)
        {
            // if the lifetimes are less than 0 - don't try an equality on floats (bad: lifetime.Value == 0 || lifetime.Value <= 0)
            if (lifeTime.Value < 0)
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
        // "Ensures that the EntityCommandBufferSystem waits for the job to complete before playing back the command buffer." - Official Unity Manual
        entityCommandBuffer.AddJobHandleForProducer(job);

        return job;
    }

    protected override void OnCreateManager()
    {
        // Prevents us from creating this every frame
        entityCommandBuffer = World.GetOrCreateManager<EndSimulationEntityCommandBufferSystem>();
    }

}