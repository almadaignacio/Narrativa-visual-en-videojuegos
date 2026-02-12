using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FlockingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<FlockBounds>(out var bounds))
        {
            return;
        }

        var boidPositions = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var boidVelocities = SystemAPI.GetComponentLookup<BoidState>(true);

        var boidQuery = state.GetEntityQuery(
             ComponentType.ReadOnly<Boid>(),
             ComponentType.ReadOnly<BoidConfig>(),
             ComponentType.ReadWrite<LocalTransform>(),
             ComponentType.ReadWrite<BoidState>()
         );

        if (boidQuery.IsEmpty)
        {
            return;
        }

        var boidEntities = boidQuery.ToEntityArray(Allocator.TempJob);

        var targetPosition = new float3(0, 0, 0);

        state.Dependency = new FlockingJob
        {
            BoidPositions = boidPositions,
            BoidVelocities = boidVelocities,
            AllBoids = boidEntities,
            DeltaTime = SystemAPI.Time.DeltaTime,
            TargetPosition = targetPosition,
            BoundsCenter = bounds.Center,
            BoundsSize = bounds.Size,
            BoundsAvoidWeight = 4f

        }.ScheduleParallel(boidQuery, state.Dependency);
    }
}

[BurstCompile]
public partial struct FlockingJob : IJobEntity
{
    [NativeDisableContainerSafetyRestriction]
    [ReadOnly] public ComponentLookup<LocalTransform> BoidPositions;

    [NativeDisableContainerSafetyRestriction]
    [ReadOnly] public ComponentLookup<BoidState> BoidVelocities;

    [ReadOnly] public NativeArray<Entity> AllBoids;

    public float DeltaTime;
    public float3 TargetPosition;

    public float3 BoundsCenter;
    public float3 BoundsSize;
    public float BoundsAvoidWeight;

    void Execute(Entity entity, ref LocalTransform transform, ref BoidState state, in BoidConfig config)
    {
        float3 separationSum = float3.zero;
        float3 positionSum = float3.zero;
        float3 alignmentSum = float3.zero;
        int neighborsCount = 0;

        foreach (var otherEntity in AllBoids)
        {
            if (entity == otherEntity) continue;

            var otherPosition = BoidPositions[otherEntity].Position;
            var distance = math.distance(transform.Position, otherPosition);

            if (distance > 0 && distance < config.VisionRadius)
            {
                separationSum += (transform.Position - otherPosition) / distance;

                positionSum += otherPosition;
                alignmentSum += BoidVelocities[otherEntity].Velocity;
                neighborsCount++;
            }
        }

        float3 steer = float3.zero;

        if (neighborsCount > 0)
        {
            var cohesionForce = (positionSum / neighborsCount - transform.Position);
            var alignmentForce = (alignmentSum / neighborsCount);

            steer += (separationSum / neighborsCount) * config.SeparationWeight;
            steer += math.normalize(cohesionForce) * config.CohesionWeight;
            steer += math.normalize(alignmentForce) * config.AlignmentWeight;
        }

        var targetForce = TargetPosition - transform.Position;
        steer += math.normalize(targetForce) * config.TargetWeight;

        float3 boundsSteer = float3.zero;
        float3 halfSize = BoundsSize / 2f;

        if (transform.Position.x < BoundsCenter.x - halfSize.x) boundsSteer.x = 1f;
        if (transform.Position.x > BoundsCenter.x + halfSize.x) boundsSteer.x = -1f;
        if (transform.Position.y < BoundsCenter.y - halfSize.y) boundsSteer.y = 1f;
        if (transform.Position.y > BoundsCenter.y + halfSize.y) boundsSteer.y = -1f;
        if (transform.Position.z < BoundsCenter.z - halfSize.z) boundsSteer.z = 1f;
        if (transform.Position.z > BoundsCenter.z + halfSize.z) boundsSteer.z = -1f;

        steer += boundsSteer * BoundsAvoidWeight;

        state.Velocity += steer * DeltaTime;
        if (math.lengthsq(state.Velocity) > config.MaxSpeed * config.MaxSpeed)
        {
            state.Velocity = math.normalize(state.Velocity) * config.MaxSpeed;
        }

        transform.Position += state.Velocity * DeltaTime;
        transform.Rotation = quaternion.LookRotationSafe(state.Velocity, math.up());
    }
}