using System.Reflection;
using System.Runtime;
using System.Linq;
using Unity.Entities;
using Colossal.Logging;
using Game;
using Game.Economy;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
//using HarmonyLib;

namespace SignatureFix
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(SignatureFix)}").SetShowsErrorsInUI(false);
        //public static readonly string harmonyID = "Infixo.SignatureFix";

        //private PrefabSystem m_PrefabSystem;
        //private EntityManager m_EntityManager;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            // Harmony
            /*
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyID);
            var patchedMethods = harmony.GetPatchedMethods().ToArray();

            log.Info($"Plugin {harmonyID} made patches! Patched methods: " + patchedMethods.Length);

            foreach (var patchedMethod in patchedMethods)
            {
                log.Info($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
            }
            */

            // Patch industrial signature buildings
            //m_PrefabSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
            //m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //PatchBuilding("IndustrialManufacturingSignature02", ResourceInEditor.Minerals);  // Ground Earth
            //PatchBuilding("IndustrialManufacturingSignature03", ResourceInEditor.Food); // Dairy House
            //PatchBuilding("IndustrialManufacturingSignature09", ResourceInEditor.Petrochemicals); // Oil Refinery
            updateSystem.UpdateAfter<SignatureFixSystem>(SystemUpdatePhase.PrefabUpdate);
            updateSystem.UpdateBefore<SignatureFixSystem>(SystemUpdatePhase.PrefabReferences);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }

        /*
         * OLD VERSION FOR REFERENCE
         * 
        private void PatchBuilding(string name, ResourceInEditor resource)
        {
            PrefabID prefabID = new PrefabID(nameof(BuildingPrefab), name);
            if (m_PrefabSystem.TryGetPrefab(prefabID, out PrefabBase prefab) && m_PrefabSystem.TryGetEntity(prefab, out Entity entity))
            {
                // Prefab itself
                BuildingProperties bp = prefab.GetComponent<BuildingProperties>();
                bp.m_AllowedManufactured[0] = resource;
                log.Info($"Prefab: {prefab.name} res {EconomyUtils.GetNames(EconomyUtils.GetResources(bp.m_AllowedManufactured))}");

                // Get the component data that needs to be patched
                
                if (m_PrefabSystem.TryGetComponentData<BuildingPropertyData>(prefab, out BuildingPropertyData buildingPropertyData))
                {
                    buildingPropertyData.m_AllowedManufactured = EconomyUtils.GetResource(resource);
                    m_PrefabSystem.AddComponentData<BuildingPropertyData>(prefab, buildingPropertyData);
                    log.Info($"Patched {prefab.name} for {resource}");
                }
                
                // Entity check
                // List components from entity
                foreach (ComponentType componentType in m_EntityManager.GetComponentTypes(entity))
                {
                    //log.Info("Component: " + componentType.GetManagedType() + " " + componentType.ToString());
                }
                BuildingPropertyData bpd = m_EntityManager.GetComponentData<BuildingPropertyData>(entity);
                PrefabData pd = m_EntityManager.GetComponentData<PrefabData>(entity);
                log.Info($"Entity: prefab {m_PrefabSystem.GetPrefabName(entity)} idx {pd.m_Index} resource {bpd.m_AllowedManufactured}");
                m_PrefabSystem.UpdatePrefab(prefab);//, entity);
            }
        }
        */
    }
}
