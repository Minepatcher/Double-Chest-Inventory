using System;
using System.Linq;
using Inventory;
using PugMod;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Double_Chest_Inventory
{
    public class DoubleChestInventory : IMod
    {
        public void EarlyInit() { }

        public void Init()
        {
            Debug.Log("[Double Chest Inventory]: Initializing...");
            var chestList = PugDatabase.entityMonobehaviours.Where(x => 
                x.GameObject.name.Contains("chest", StringComparison.OrdinalIgnoreCase)
                && !x.GameObject.TryGetComponent(out ChangeVariationWhenContainingObjectAuthoring _)
                && x.ObjectInfo.prefabInfos?[0].prefab?.GetComponent<EntityMonoBehaviour>() is Chest
                && x.GameObject.TryGetComponent(out InventoryAuthoring _));
            //var chestList = ;
            foreach (var chest in chestList) {
                Debug.Log($"[Double Chest Inventory]: {chest}");
                var invAuthoring = chest.GameObject.GetComponent<InventoryAuthoring>();
                int totalSize = invAuthoring.sizeX * invAuthoring.sizeY;
                int newTotalSize = totalSize * 2;
                if (newTotalSize > 135) {
                    invAuthoring.sizeX = 15;
                    invAuthoring.sizeY = 9;
                    continue;
                }
                int newXSize = invAuthoring.sizeX;
                int newYSize = invAuthoring.sizeY;
                bool noSizeFound = true;
                int i = 15;
                while (noSizeFound) { 
                    if (newTotalSize % i == 0 && newTotalSize / i <= 9)
                    {
                        newXSize = i;
                        newYSize = newTotalSize / i;
                        noSizeFound = false;
                    } else {
                        --i;
                    }
                }
                invAuthoring.sizeX = newXSize;
                invAuthoring.sizeY = newYSize;
            }
            Debug.Log("[Double Chest Inventory]: Finished Doubling Chest Inventory Size...");
        }

        public void Shutdown() { }
        public void ModObjectLoaded(Object obj) { }
        public void Update() { }
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof (InventorySystemGroup))]
    [UpdateAfter(typeof (InventoryUpdateSystem))]
    public struct DoubleChestInventoryServerSystem
    {
        
    }
}
