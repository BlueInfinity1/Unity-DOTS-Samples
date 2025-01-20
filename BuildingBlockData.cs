using Unity.Entities;
using Unity.Mathematics;

public struct BuildingBlockData : IComponentData
{    
    public float3 TargetPosition; // The position the spawned building block will attempt to reach before stopping motion
    public float3 Velocity; // The drop velocity in y axis
    public float3 Acceleration; // The drop acceleration in y axis
}
