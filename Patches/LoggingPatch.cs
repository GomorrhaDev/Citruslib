using System;
using HarmonyLib;

namespace WobbleBridge.Patches
{
    // praying
    [HarmonyPatch(typeof(LandLog), nameof(LandLog.Log), new Type[] { typeof(string), typeof(object) })]
    class LogPatch
    {
        static bool Prefix(string logMessage, object context)
        {
            Wobble.landLog.Log(logMessage);
            return false;
        }
    }

    [HarmonyPatch(typeof(LandLog), nameof(LandLog.LogError))]
    class ErrPatch
    {
        static bool Prefix(string logMessage, object context)
        {
            Wobble.landLog.LogError(logMessage);
            return false;
        }
    }
}
