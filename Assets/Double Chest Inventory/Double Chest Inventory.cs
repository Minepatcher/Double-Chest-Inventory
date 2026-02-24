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
        [HarmonyPostfix]
        // ReSharper disable once InconsistentNaming
        public static void ECSManager_Init(ECSManager __instance)
        {
            Debug.Log("[Double Chest Inventory]: Initializing...");
            var chestList = PugDatabase.entityMonobehaviours
                .Select(monoBehaviour => monoBehaviour.GameObject)
                .Where(monoBehaviour =>
                !monoBehaviour.TryGetComponent(out ChangeVariationWhenContainingObjectAuthoring _)
                && monoBehaviour.TryGetComponent(out InventoryAuthoring inv)
                && inv.sizeX > 2 && inv.sizeY > 2
                && (monoBehaviour.TryGetComponent(out EntityMonoBehaviourData e)
                    ? e.ObjectInfo.prefabInfos?[0].prefab?.GetComponent<EntityMonoBehaviour>() is Chest
                    : monoBehaviour.TryGetComponent(out ObjectAuthoring o) && o.graphicalPrefab?.GetComponent<EntityMonoBehaviour>() is Chest));
            foreach (var chest in chestList)
            {
                //Debug.Log($"[Double Chest Inventory]: {chest}");
                var invAuthoring = chest.GetComponent<InventoryAuthoring>();
                int totalSize = invAuthoring.sizeX * invAuthoring.sizeY;
                int newTotalSize = totalSize * 2;
                if (newTotalSize > 144)
                {
                    invAuthoring.sizeX = 16;
                    invAuthoring.sizeY = 9;
                    continue;
                }

                int newXSize = invAuthoring.sizeX;
                int newYSize = invAuthoring.sizeY;
                bool noSizeFound = true;
                int i = 16;
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