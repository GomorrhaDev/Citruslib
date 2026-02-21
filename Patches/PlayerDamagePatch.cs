using System;
using HarmonyLib;
using Landfall.Network;

namespace CitrusLib.Patches
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

            if (Citrus.PlayerActiveWeapons.TryGetValue(damagerIndex, out int weaponId))
            {
                float multiplier = Citrus.GetWeaponDamageMultiplier(weaponId);
                if (multiplier != 1.0f)
                    dmg *= multiplier;
            }
        }
    }
}