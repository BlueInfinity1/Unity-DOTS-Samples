using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct BuildingGeneratorSystem : ISystem
{
    private Unity.Mathematics.Random random;
    private static bool initialized = false;

   /*  NOTE: The building generation would be ideally done in OnCreate, but since there doesn't seem to be a way to ensure that EntityReferencesAuthoring.Bake has run before OnCreate, we 
    *  run the generation code in the first OnUpdate where we have the EntityReferences available.
    */

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Require the EntityReferences singleton for this system to run
        state.RequireForUpdate<EntityReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Check if the system has already initialized
        if (initialized)
        {
            UnityEngine.Debug.Log("BuildingGeneratorSystem already initialized.");
            state.Enabled = false; // Disable further updates
            return;
        }

        // Ensure the EntityReferences singleton is available
        if (!SystemAPI.TryGetSingleton<EntityReferences>(out var entityReferences))
        {
            UnityEngine.Debug.LogError("EntityReferences singleton not found!");
            return;
        }

        initialized = true; // Mark system as initialized
        UnityEngine.Debug.Log("EntityReferences singleton retrieved successfully.");


        GenerateBuilding(ref state, entityReferences);

        state.Enabled = false; // Disable the system after initialization
    }

    [BurstCompile]    
    public void GenerateBuilding(ref SystemState state, EntityReferences entityReferences)
    {
        // Initialize random number generator
        random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);

        var entityManager = state.EntityManager;

        int blockCount = 0;
        int numBuildings = 300;

        for (int buildingIndex = 0; buildingIndex < numBuildings; buildingIndex++)
        {
            // Generate random position for the building
            float3 buildingPosition = new float3(
                random.NextInt(0, 800),
                0,
                random.NextInt(0, 800)
            );
            
            Entity buildingParent = entityManager.Instantiate(entityReferences.buildingParentEntity);
            
            entityManager.AddComponentData(buildingParent, new LocalTransform
            {
                Position = buildingPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            // Generate a random number of layers (= "floors of blocks")
            int numLayers = random.NextInt(5, 11);
            float layerHeight = 1f; // Height of each layer

            // Generate a random width and depth for the layer (odd numbers >= 5 and <= 21)
            int width = random.NextInt(5, 21) | 1; // Ensure width is an odd number by utilizing bitwise or with 1, which always sets the least significant bit to 1
            int depth = random.NextInt(5, 21) | 1; // Ensure depth is an odd number
            
            float3 blockDefaultAcceleration = new(0, -20, 0);
            
            // Iterate through layers
            for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
            {
                // TODO: Refactor later on to not replicate code between the if branches
                if (layerIndex == 0 || layerIndex == numLayers - 1) // The floor and ceiling blocks are the only blocks in their respective layers, and their x and z scales are bigger than that of a standard block
                {
                    blockCount++;
                    
                    float3 targetPosition = new float3(
                        0,                    // X position (centered)
                        layerIndex * layerHeight, // Y position target based on the index of the layer this block belongs to
                        0                     // Z position (centered)
                    );

                    // Generate the floor (layerIndex == 0) or ceiling (layerIndex == numLayers - 1)
                    Entity specialLayerEntity = entityManager.Instantiate(entityReferences.buildingBlockEntity);

                    // Set the parent to the building parent
                    entityManager.AddComponentData(specialLayerEntity, new Parent { Value = buildingParent });

                    // Set the target fall position and acceleration
                    entityManager.SetComponentData(specialLayerEntity, new BuildingBlockData
                    {
                        TargetPosition = targetPosition,
                        Acceleration = blockDefaultAcceleration
                    });

                    // Set the starting position for the falling simulation
                    entityManager.SetComponentData(specialLayerEntity, new LocalTransform
                    {
                        Position = targetPosition + new float3(0, layerHeight * (10 + layerIndex * 2), 0), // Start higher than the target position
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });

                    // Apply scaling for both floor and ceiling to make these blocks cover the whole building bottom and top. Non-uniform scaling must be applied by using a PostTransformMatrix, since
                    // LocalTransform.Scale is a float, not float3.
                    entityManager.AddComponentData(specialLayerEntity, new PostTransformMatrix
                    {
                        Value = float4x4.Scale(new float3(width, 1f, depth))
                    });                    
                }
                else
                {
                    // Generate regular layers
                    Entity layerEntity = entityManager.Instantiate(entityReferences.buildingLayerEntity);

                    // Set the parent of the layer to the building parent
                    entityManager.AddComponentData(layerEntity, new Parent { Value = buildingParent });

                    // Position the layer based on its index
                    entityManager.SetComponentData(layerEntity, new LocalTransform
                    {
                        Position = new float3(0, layerIndex * layerHeight, 0),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });

                    // Origin of the floor in local space
                    int originX = (width - 1) / 2;
                    int originZ = (depth - 1) / 2;

                    // Iterate through the width and depth to generate blocks along the borders
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < depth; z++)
                        {
                            blockCount++;

                            // Skip the interior (only generate blocks along the borders)
                            if ((x != 0 && x != width - 1) && (z != 0 && z != depth - 1)) continue;

                            // Skip creating blocks for door spaces on x = originX - 1, originX, originX + 1, but only for the first few layers
                            if (math.abs(x - originX) <= 1 && layerIndex < 5) continue;

                            // Instantiate a block entity
                            Entity blockEntity = entityManager.Instantiate(entityReferences.buildingBlockEntity);

                            // Set the parent of the block to the layer
                            entityManager.AddComponentData(blockEntity, new Parent { Value = layerEntity });

                            // Position the block within the layer
                            float3 targetPosition = new float3(
                                x - originX,       // Centered around the origin in X
                                0,                
                                z - originZ        // Centered around the origin in Z
                            );

                            // Set the target position
                            entityManager.SetComponentData(blockEntity, new BuildingBlockData
                            {
                                TargetPosition = targetPosition,
                                Acceleration = blockDefaultAcceleration
                            });

                            // Set the starting position for the falling simulation
                            entityManager.SetComponentData(blockEntity, new LocalTransform
                            {
                                Position = targetPosition + new float3(0, layerHeight * (10 + layerIndex * 2) + random.NextFloat(-0.5f, 0.5f), 0), // Start higher with some random variation in the starting position
                                Rotation = quaternion.identity,
                                Scale = 1f
                            });
                        }
                    }
                }
            }            
        }

        UnityEngine.Debug.Log($"Total generated blocks: {blockCount}");
    }    
}
