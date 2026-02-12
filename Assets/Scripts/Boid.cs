using Unity.Entities;
using Unity.Mathematics;

public struct Boid : IComponentData { }

public struct BoidConfig : IComponentData
{
    public float VisionRadius;
    public float SeparationWeight;
    public float CohesionWeight;
    public float AlignmentWeight;
    public float TargetWeight;
    public float MaxSpeed;
}

public struct BoidState : IComponentData
{
    public float3 Velocity;
}