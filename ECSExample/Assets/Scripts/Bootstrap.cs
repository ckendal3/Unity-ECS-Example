using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
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
    public bool rotationCube = false;

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
    quaternion rQuat;
    float3 rPos, rScale;
    float rLife, rSpeed;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get the entity manager (or create it)
        entityManager = World.Active.EntityManager;

        // Create our Entity archetype to spawn our cubes from later
        cubeArch = entityManager.CreateArchetype(
                                                ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<NonUniformScale>(),
                                                ComponentType.ReadWrite<LifeTime>(), ComponentType.ReadWrite<Speed>(), 
                                                ComponentType.ReadOnly<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
                                                );
    }

    public void UpdateCubeSpawnRate(Text text)
    {
        float result;

        if (float.TryParse(text.text, out result))
        {
            cubeSpawnRate = result;
        }

        timeSinceLastSpawn = cubeSpawnRate;
    }

    public void UpdateCubeSpawnAmount(Text text)
    {
        int result;

        if (int.TryParse(text.text, out result))
        {
            NumCubesToSpawnAtOnce = result;
        }
        
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

    // TRY TODO: Batch instantiate cubes then use system(s) to set the random values on the cubes.
    // Spawn Cubes and set their values
    public void SpawnCube()
    {
#region Random Values Generation
        // Initiate random
        r = new Random((uint)UnityEngine.Random.Range(0, 1000000));

        rPos = r.NextFloat3();
        rScale = r.NextFloat3(.5f, 2.0f);
        rLife = r.NextFloat(2, 10);
        rSpeed = r.NextFloat(1, 20);
        rQuat = r.NextQuaternionRotation();
#endregion

        // Create the entity
        Entity cubeEntity = entityManager.CreateEntity(cubeArch);

        // Set all of the entity data
        entityManager.SetComponentData(cubeEntity, new Translation { Value = rPos }); // This is the Starting Position
        entityManager.SetComponentData(cubeEntity, new NonUniformScale { Value = rScale }); // This is the Starting Position
        entityManager.SetComponentData(cubeEntity, new Rotation { Value = rQuat }); // This is the Starting Rotation
        entityManager.SetComponentData(cubeEntity, new LifeTime { Value = rLife }); // This determines how long the cube lasts
        entityManager.SetComponentData(cubeEntity, new Speed { Value = rSpeed }); // This is determines the rotation and movement speed


        // Use two different colors just for variety's sack
        if(primaryColor)
        {
            entityManager.AddSharedComponentData(cubeEntity, new RenderMesh { mesh = inMesh, material = inMaterial_Primary }); // This is the mesh and material used (material is instanced)
            primaryColor = false;
        }
        else
        {
            entityManager.AddSharedComponentData(cubeEntity, new RenderMesh { mesh = inMesh, material = inMaterial_Secondary }); // This is the mesh and material used (material is instanced)
            primaryColor = true;
        }
        

        // if inspector value for rotation cube is true
        if(rotationCube)
        {
            // Adds a new component to the entity - specifically a tag with no data for the RotationSystem 
            entityManager.AddComponentData(cubeEntity, new RotationOnlyTag { });
        }
    }
}

public enum SpawnType
{
    None,
    Input,
    Timer
}
