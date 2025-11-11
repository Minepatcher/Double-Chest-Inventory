using System;
using System.Linq;
using HarmonyLib;
using PugMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Double_Chest_Inventory
{
    public class DoubleChestInventory : IMod
    {
        public void EarlyInit() { }
        public void Init() { }
        public void Shutdown() { }
        public void ModObjectLoaded(Object obj) { }
        public void Update() { }
    }

    [HarmonyPatch]
    // ReSharper disable once InconsistentNaming
    public static class ECSManagerPatch
    {
        [HarmonyPatch(typeof(ECSManager), nameof(ECSManager.Init))]
        [HarmonyPrefix]
        // ReSharper disable once InconsistentNaming
        public static void ECSManager_Init(ECSManager __instance)
        {
            Debug.Log("[Double Chest Inventory]: Initializing...");
            var chestList = Manager.ecs.pugDatabase.prefabList.Where(x =>
                x.name.Contains("chest", StringComparison.OrdinalIgnoreCase)
                && !x.TryGetComponent(out ChangeVariationWhenContainingObjectAuthoring _)
                && x.GetComponent<EntityMonoBehaviourData>().ObjectInfo.prefabInfos?[0].prefab
                    ?.GetComponent<EntityMonoBehaviour>() is Chest
                && x.TryGetComponent(out InventoryAuthoring _));
            foreach (var chest in chestList)
            {
                //Debug.Log($"[Double Chest Inventory]: {chest}");
                var invAuthoring = chest.GetComponent<InventoryAuthoring>();
                int totalSize = invAuthoring.sizeX * invAuthoring.sizeY;
                int newTotalSize = totalSize * 2;
                if (newTotalSize > 135)
                {
                    invAuthoring.sizeX = 15;
                    invAuthoring.sizeY = 9;
                    continue;
                }

                int newXSize = invAuthoring.sizeX;
                int newYSize = invAuthoring.sizeY;
                bool noSizeFound = true;
                int i = 15;
                while (noSizeFound)
                {
                    if (newTotalSize % i == 0 && newTotalSize / i <= 9)
                    {
                        newXSize = i;
                        newYSize = newTotalSize / i;
                        noSizeFound = false;
                    }
                    else
                    {
                        --i;
                    }
                }

                invAuthoring.sizeX = newXSize;
                invAuthoring.sizeY = newYSize;
            }

            Debug.Log("[Double Chest Inventory]: Finished Doubling Chest Inventory Size...");
        }
    }
}