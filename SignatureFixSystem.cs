using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;
using Game;
using Game.Prefabs;
using Game.Economy;
using Game.Buildings;
using System.Reflection;
using System.Security.AccessControl;
using Mono.Cecil;

namespace SignatureFix
{

    public partial class SignatureFixSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_SignaturesQuery;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = base.World.GetOrCreateSystemManaged<PrefabSystem>();
            m_SignaturesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[3]
                {
                ComponentType.ReadOnly<PrefabData>(),
                ComponentType.ReadOnly<SignatureBuildingData>(),
                ComponentType.ReadWrite<BuildingPropertyData>(),
                }
            });
            RequireForUpdate(m_SignaturesQuery);
        }

        [Preserve]
        protected override void OnUpdate()
        {
            //Mod.log.Info($"OnUpdate: {m_SignaturesQuery.CalculateEntityCount()} entities");
            foreach (Entity prefabEntity in m_SignaturesQuery.ToEntityArray(Allocator.Temp))
            {
                //Mod.log.Info($"{prefabEntity}");
                PrefabData prefabData = base.EntityManager.GetComponentData<PrefabData>(prefabEntity);
                BuildingPropertyData buildingData = base.EntityManager.GetComponentData<BuildingPropertyData>(prefabEntity);
                
                if (!m_PrefabSystem.TryGetPrefab(prefabData, out PrefabBase prefab))
                {
                    Mod.log.Info($"Warning. Failed to get PrefabBase for {prefabData.m_Index}.");
                    continue;
                }

                ResourceInEditor res = ResourceInEditor.NoResource;
                switch (prefab.name)
                {
                    case "IndustrialManufacturingSignature02": // Ground Earth
                        res = ResourceInEditor.Minerals;
                        break;
                    case "IndustrialManufacturingSignature03": // Dairy House
                        res = ResourceInEditor.Food;
                        break;
                    case "IndustrialManufacturingSignature09": // Oil Refinery
                        res = ResourceInEditor.Petrochemicals;
                        break;
                    default:
                        break;
                }

                if (res != ResourceInEditor.NoResource)
                {

                    // Patch PrefabBase
                    BuildingProperties bp = prefab.GetComponent<BuildingProperties>();
                    bp.m_AllowedManufactured[0] = res;
                    Mod.log.Info($"Patched {prefab.name} (Prefab) for {EconomyUtils.GetNames(EconomyUtils.GetResources(bp.m_AllowedManufactured))}");

                    // Patch Prefab Entity
                    buildingData.m_AllowedManufactured = EconomyUtils.GetResource(res);
                    base.EntityManager.SetComponentData(prefabEntity, buildingData);

                    // The issue is that Archetype still contains ExtractorProperty that must be removed
                    // There is RefreshArchetype() method in BuildingPrefab that could be used but it is protected
                    BuildingPrefab buildingPrefab = prefab as BuildingPrefab;
                    if (m_PrefabSystem.TryGetEntity(buildingPrefab, out Entity entity))
                    {
                        // For some reason Harmony cannot make a ReversePatch :(
                        //Patches.BuildingPrefab_RefreshArchetype(base.EntityManager, entity);

                        // Using Reflection to call a protected member
                        Type buildingPrefabType = typeof(BuildingPrefab);
                        MethodInfo methodInfo = buildingPrefabType.GetMethod("RefreshArchetype", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (methodInfo != null)
                        {
                            object[] parameters = { base.EntityManager, entity };
                            methodInfo.Invoke(buildingPrefab, parameters);
                            Mod.log.Info($"Patched {prefab.name} (Archetype) for {res}.");
                        }
                        else
                        {
                            Mod.log.Info($"Warning. Failed to call RefreshArchetype on {prefab.name}!");
                        }
                    }
                    else
                    {
                        Mod.log.Info($"Warning. Failed to get Entity for {prefab.name}.");
                    }
                }
                //Mod.log.Info($"Entity {entity.Index}: resources {buildingData.m_AllowedSold} {buildingData.m_AllowedManufactured} {buildingData.m_AllowedStored}");
            }
            base.Enabled = false;
        }

        [Preserve]
        public SignatureFixSystem()
        {
        }

    } // class

} // namespace
