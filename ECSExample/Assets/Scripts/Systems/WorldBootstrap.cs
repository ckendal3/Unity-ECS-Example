using Unity.Entities;

public class WorldBootstrap : ICustomBootstrap
{
    public bool Initialize(string defaultWorldName)
    {
        var world = new World("BootstrapWorld");
        World.DefaultGameObjectInjectionWorld = world;

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        return true;
    }
}