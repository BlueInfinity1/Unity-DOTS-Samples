using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using System.Diagnostics;

[BurstCompile]
public partial struct BuildingBlockSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Create and schedule the block update job
        var blockUpdateJob = new BlockUpdateJob
        {
            DeltaTime = deltaTime
        };

        state.Dependency = blockUpdateJob.ScheduleParallel(state.Dependency);
    }
    
    public partial struct BlockUpdateJob : IJobEntity
    {
        public float DeltaTime;

        public void Execute(ref BuildingBlockData block, ref LocalTransform transform)
        {
            // Update velocity with acceleration
            block.Velocity += block.Acceleration * DeltaTime;

            // Apply velocity to the position
            transform.Position += block.Velocity * DeltaTime;

            // Check if the block has reached or gone below its target position
            if (transform.Position.y <= block.TargetPosition.y)
            {
                if (math.abs(block.Velocity.y) < 5f)
                {
                    // Snap to target position
                    transform.Position.y = block.TargetPosition.y;

                    // Stop movement
                    block.Velocity = float3.zero;
                }
                else // Bounce the block back slightly
                {
                    block.Velocity *= -0.1f;
                }

            }
        }
    }
}
