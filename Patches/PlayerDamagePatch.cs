using System;
using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{

    [HarmonyPatch(typeof(TABGPlayerBase), "TakeDamage", new Type[] { typeof(float) })]
    public class PlayerDamagePatch
    {
        static void Prefix(TABGPlayerBase __instance, ref float dmg)
        {
            if (__instance is not TABGPlayerServer victim)
                return;

            byte damagerIndex = victim.LastAttacker;
            if (damagerIndex == byte.MaxValue)
                return;

            if (Wobble.PlayerActiveWeapons.TryGetValue(damagerIndex, out int weaponId))
            {
                float multiplier = Wobble.GetWeaponDamageMultiplier(weaponId);
                if (multiplier != 1.0f)
                    dmg *= multiplier;
            }
        }
    }
}