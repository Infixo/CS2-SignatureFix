using Game;
using Game.Prefabs;
using Game.Economy;
using HarmonyLib;

namespace SignatureFix;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPatch(typeof(Game.Prefabs.PrefabSystem), "AddPrefab")]
    [HarmonyPrefix]
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
                Plugin.Log($"Patched {prefab.name} for {res}");
            }
        }
        return true;
    }
}
