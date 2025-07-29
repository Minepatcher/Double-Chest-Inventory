using System;
using System.Linq;
using PugMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Double_Chest_Inventory
{
    public class DoubleChestInventory : IMod
    {
        public void EarlyInit()
        {
        }

        public void Init()
        {
            Debug.Log("[Double Chest Inventory]: Initializing...");
            var chestList = PugDatabase.entityMonobehaviours.Where(x => 
                x.GameObject.name.Contains("chest", StringComparison.OrdinalIgnoreCase)
                && !x.GameObject.name.Contains("locked", StringComparison.OrdinalIgnoreCase)
                && x.ObjectInfo.prefabInfos?[0].prefab?.GetComponent<EntityMonoBehaviour>() is Chest
                && x.GameObject.TryGetComponent(out InventoryAuthoring _)).ToList();
            foreach (var chest in chestList) {
                var invAuthoring = chest.GameObject.GetComponent<InventoryAuthoring>();
                var totalSize = invAuthoring.sizeX * invAuthoring.sizeY;
                var newTotalSize = totalSize * 2;
                if (newTotalSize > 135) {
                    invAuthoring.sizeX = 15;
                    invAuthoring.sizeY = 9;
                    continue;
                }
                var newXSize = invAuthoring.sizeX;
                var newYSize = invAuthoring.sizeY;
                var noSizeFound = true;
                var i = 15;
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

        public void Shutdown()
        {
        }

        public void ModObjectLoaded(Object obj)
        {
        }

        public void Update()
        {
        }
    }
}
