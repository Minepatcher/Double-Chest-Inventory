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

    public class Logger
    {
        private readonly string _tag;
        public Logger(string tag) => _tag = $"[{tag}]";
        public void LogInfo(string text) => Debug.unityLogger.Log(LogType.Log, _tag, text);
        public void LogWarning(string text) => Debug.unityLogger.Log(LogType.Warning, _tag, text);
        public void LogError(string text) => Debug.unityLogger.Log(LogType.Error, _tag, text);
    }

    [HarmonyPatch]
    // ReSharper disable once InconsistentNaming
    public static class ECSManagerPatch
    {
        public const string Version = "0.3.0";
        public const string ModID = "DoubleChestInventoryMod";
        public const string FriendlyName = "Double Chest Inventory Mod";
        internal static readonly Logger Log = new(FriendlyName);

        [HarmonyPatch(typeof(ECSManager), nameof(ECSManager.Init))]
        [HarmonyPostfix]
        // ReSharper disable once InconsistentNaming
        public static void ECSManager_Init(ECSManager __instance)
        {
            Log.LogInfo($"v{Version}");
            Log.LogInfo("Initializing...");
            var chestList = PugDatabase.entityMonobehaviours
                .Select(monoBehaviour => monoBehaviour.GameObject)
                .Where(monoBehaviour =>
                    !monoBehaviour.TryGetComponent(out ChangeVariationWhenContainingObjectAuthoring _)
                    && monoBehaviour.TryGetComponent(out InventoryAuthoring inv)
                    && inv.sizeX > 2 && inv.sizeY > 2
                    && (monoBehaviour.TryGetComponent(out EntityMonoBehaviourData e)
                        ? e.ObjectInfo.prefabInfo.GetGraphical().GetComponent<EntityMonoBehaviour>() is Chest
                        : monoBehaviour.TryGetComponent(out ObjectAuthoring o) &&
                          o.graphicalPrefab?.GetComponent<EntityMonoBehaviour>() is Chest));
            foreach (var chest in chestList)
            {
                //Log.LogInfo($"{chest}");
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
            Log.LogInfo("Finished Doubling Chest Inventory Size...");
        }
    }
}