using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FlockBoundsAuthoring : MonoBehaviour
{
    public float3 size = new float3(50, 50, 50);

    class Baker : Baker<FlockBoundsAuthoring>
    {
        public override void Bake(FlockBoundsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new FlockBounds
            {
                Center = authoring.transform.position,
                Size = authoring.size
            });
        }
    }
}

public struct FlockBounds : IComponentData
{
    public float3 Center;
    public float3 Size;
}