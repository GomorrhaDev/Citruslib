using BepInEx;
using HarmonyLib;

namespace WobbleBridge
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginGuid    = "gmrrh.tabg.wobblebridge";
        public const string pluginName    = "WobbleBridge";
        public const string pluginVersion = "0.1.1";

        public void Awake()
        {
            Wobble.log.Log($"{pluginName} {pluginVersion} Loading!");

            new Harmony(pluginGuid).PatchAll();
            
            CommandRegistry.Register();
            SettingsRegistry.Register();
            CustomLootTables.Register();
            WeaponPatchManager.Initialize();
        }
    }
}