using Unity.Entities;
using UnityEngine;

public class BoidAuthoring : MonoBehaviour
{
    public float visionRadius = 5f;
    public float separationWeight = 2f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float targetWeight = 0.5f;
    public float maxSpeed = 10f;

    class Baker : Baker<BoidAuthoring>
    {
        public override void Bake(BoidAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Boid());

            AddComponent(entity, new BoidConfig
            {
                VisionRadius = authoring.visionRadius,
                SeparationWeight = authoring.separationWeight,
                CohesionWeight = authoring.cohesionWeight,
                AlignmentWeight = authoring.alignmentWeight,
                TargetWeight = authoring.targetWeight,
                MaxSpeed = authoring.maxSpeed
            });

            AddComponent(entity, new BoidState
            {
                Velocity = UnityEngine.Random.insideUnitSphere * authoring.maxSpeed
            });
        }
    }
}