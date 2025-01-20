using Unity.Entities;
using UnityEngine;

// This class converts assigned GameObject prefabs into ECS entities and stores their references.
// It enables ECS systems to use these entities for building-related instantiation and logic.

public class EntityReferencesAuthoring : MonoBehaviour
{
    public GameObject buildingParentPrefab;
    public GameObject buildingLayerPrefab;
    public GameObject buildingBlockPrefab;

    public class Baker : Baker<EntityReferencesAuthoring>
    {
        public override void Bake(EntityReferencesAuthoring authoring)
        {
            Debug.Log("Running EntityReferencesAuthoring Baker");

            // Ensure the referenced prefabs are assigned
            if (authoring.buildingParentPrefab == null ||
                authoring.buildingLayerPrefab == null ||
                authoring.buildingBlockPrefab == null)
            {
                Debug.LogError("One or more prefab references are missing in EntityReferencesAuthoring!");
                return;
            }

            // Create the entity and add the EntityReferences component
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);            

            Entity parentEntity = GetEntity(authoring.buildingParentPrefab, TransformUsageFlags.Dynamic);
            Entity layerEntity = GetEntity(authoring.buildingLayerPrefab, TransformUsageFlags.Dynamic);
            Entity blockEntity = GetEntity(authoring.buildingBlockPrefab, TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new EntityReferences
            {
                buildingParentEntity = parentEntity,
                buildingLayerEntity = layerEntity,
                buildingBlockEntity = blockEntity
            });

            Debug.Log("EntityReferences component added successfully.");
        }
    }
}

public struct EntityReferences : IComponentData
{
    public Entity buildingParentEntity;
    public Entity buildingLayerEntity;
    public Entity buildingBlockEntity;
}
