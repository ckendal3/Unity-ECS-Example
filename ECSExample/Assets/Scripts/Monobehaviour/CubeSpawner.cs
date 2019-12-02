using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;
using Unity.Collections;

public class CubeSpawner : MonoBehaviour
{
    #region Cube Spawn Variables
    // Determines how the cubes spawn.
    [SerializeField]
    public SpawnType spawnType = SpawnType.Timer;

    // How many seconds inbetween cube spawns
    [SerializeField]
    public float cubeSpawnRate = 3;

    // Determines how many cubes to spawn.
    [SerializeField]
    public int NumCubesToSpawnAtOnce = 1;

    // Determines if the cubes spawned will rotate
    [SerializeField]
    public bool rotating = false;

    // mesh to be spawned
    [SerializeField]
    Mesh inMesh;

    // material to be used on the mesh, primary color - should be instanced
    [SerializeField]
    Material inMaterial_Primary;

    // material to be used on the mesh, secondary color - should be instanced
    [SerializeField]
    Material inMaterial_Secondary;

    private float timeSinceLastSpawn = 0;

    #endregion

    #region ECS Variables
    EntityManager entityManager;
    EntityArchetype cubeArch;
    #endregion

    #region Random Variables
    bool primaryColor = true;
    Random r;
    float3 rPos;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Get the entity manager (createed in WorldBootstrap.cs)
        // DefaultGameObjectInjectionWorld - only used in gameobjects here
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create our Entity archetype to spawn our cubes from later
        cubeArch = entityManager.CreateArchetype(
                                                ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<Translation>(),
                                                ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<NonUniformScale>(),
                                                ComponentType.ReadWrite<LifeTime>(), ComponentType.ReadWrite<Speed>()
                                                );

    }

    void Update()
    {
        if(inMesh == null || inMaterial_Primary == null || inMaterial_Secondary == null)
        {
            Debug.LogError("Mesh or materials are null - needs to be fixed");
            return;
        }

        // Determine spawn settings based on inspector values
        switch (spawnType)
        {
            case SpawnType.None:
                return;
            case SpawnType.Input:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    for(int i = 0; i < NumCubesToSpawnAtOnce; i++)
                    {
                        SpawnCube();
                    }
                }
                break;
            case SpawnType.Timer:
                timeSinceLastSpawn -= Time.deltaTime;
                if (timeSinceLastSpawn <= 0)
                {
                    for (int i = 0; i < NumCubesToSpawnAtOnce; i++)
                    {
                        SpawnCube();
                    }

                    // Set the 'timer' back to the start
                    timeSinceLastSpawn = cubeSpawnRate;
                }
                break;
        }
        
    }

    // TODO: Use system(s) to set the component values on the cubes.
    public void SpawnCube()
    {
        r = new Random((uint)UnityEngine.Random.Range(0, 34377));

        // Batch instantiate entities
        NativeArray<Entity> entities = new NativeArray<Entity>(NumCubesToSpawnAtOnce, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        entityManager.CreateEntity(cubeArch, entities);

        Entity entity;
        for (int i = 0; i < entities.Length; i++)
        {
            entity = entities[i];

            // Set all of the entity data
            entityManager.SetComponentData(entity, new Translation { Value = r.NextFloat3() }); // This is the Starting Position
            entityManager.SetComponentData(entity, new NonUniformScale { Value = r.NextFloat3(.5f, 2.0f) }); // This is the Starting Position
            entityManager.SetComponentData(entity, new Rotation { Value = r.NextQuaternionRotation() }); // This is the Starting Rotation
            entityManager.SetComponentData(entity, new LifeTime { Value = r.NextFloat(2, 10) }); // This determines how long the cube lasts
            entityManager.SetComponentData(entity, new Speed { Value = r.NextFloat(1, 20) });


            // Use two different colors just for variety's sake
            if (primaryColor)
            {
                entityManager.AddSharedComponentData(entity, new RenderMesh { mesh = inMesh, material = inMaterial_Primary }); // This is the mesh and material used (material is instanced)
                primaryColor = false;
            }
            else
            {
                entityManager.AddSharedComponentData(entity, new RenderMesh { mesh = inMesh, material = inMaterial_Secondary }); // This is the mesh and material used (material is instanced)
                primaryColor = true;
            }


            // if inspector value for rotation cube is true
            if (rotating)
            {
                // Adds a new component to the entity - specifically a tag with no data for the RotationSystem 
                entityManager.AddComponentData(entity, new RotationOnlyTag { });
            }
        }

        entities.Dispose();
    }
}

public enum SpawnType
{
    None,
    Input,
    Timer
}