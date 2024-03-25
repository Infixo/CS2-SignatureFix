using System;
using Unity.Entities;
using Game;
using Game.Prefabs;
using Game.Economy;
//using HarmonyLib;

namespace SignatureFix
{

    //[HarmonyPatch]
    internal class Patches
    {
        //[HarmonyPatch(typeof(Game.Prefabs.PrefabSystem), "AddPrefab")]
        //[HarmonyPrefix]
        public static bool PrefabSystem_AddPrefab_Prefix(PrefabBase prefab)
        {
            if (prefab.GetType().Name == "BuildingPrefab" && prefab.Has<SignatureBuilding>())
            {
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
                    BuildingProperties bp = prefab.GetComponent<BuildingProperties>();
                    bp.m_AllowedManufactured[0] = res;
                    Mod.log.Info($"Patched {prefab.name} for {res}");
                }
            }
            return true;
        }

        //[HarmonyPatch(typeof(Game.Prefabs.BuildingProperties), "Initialize")]
        //[HarmonyPostfix]
        public static void BuildingProperties_Initialize(BuildingProperties __instance, EntityManager entityManager, Entity entity)
        {
            BuildingPropertyData bpd = __instance.GetPropertyData();
            //if (bpd.m_AllowedManufactured != Resource.NoResource)
            {
                Mod.log.Info($"Initialize: {EconomyUtils.GetNames(bpd.m_AllowedManufactured)}");
            }
        }
        
        /* 
         * DOES NOT WORK! Harmony cannot patch it!
         * 
        [HarmonyPatch(typeof(Game.Prefabs.BuildingPrefab), "RefreshArchetype")]
        [HarmonyReversePatch]
        public static void BuildingPrefab_RefreshArchetype(EntityManager entityManager, Entity entity)
            // its a stub so it has no initial content
            => throw new NotImplementedException("It's a stub");
        */
    }

}