using HarmonyLib;

namespace WobbleBridge.Patches
{
    [HarmonyPatch(typeof(TABGLootPresetDatabase), "GetAllLootPresets")]
    class LootTableGetPatch
    {
        static bool Prefix(MatchModifier[] ___m_MatchModifiers)
        {
            CustomLootTables.vanillaMods = ___m_MatchModifiers;

            return true;
        }
    }

    [HarmonyPatch(typeof(TABGLootPresetDatabase), nameof(TABGLootPresetDatabase.GetNewMatchModifier))]
    class GetRandomMatchModPatch
    {
        static bool Prefix(ref MatchModifier __result)
        {
            __result = CustomLootTables.RandomMatchModifier();

            return false;
        }
    }
}
