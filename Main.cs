using BepInEx;
using HarmonyLib;

namespace CitrusLib
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginGuid    = "citrusbird.tabg.citruslib";
        public const string pluginName    = "Citrus Lib";
        public const string pluginVersion = "0.7";

        public void Awake()
        {
            Citrus.log.Log($"{pluginName} {pluginVersion} Loading!");

            new Harmony(pluginGuid).PatchAll();
            
            CommandRegistry.Register();
            SettingsRegistry.Register();
            CustomLootTables.Register();
            WeaponPatchManager.Initialize();
        }
    }
}