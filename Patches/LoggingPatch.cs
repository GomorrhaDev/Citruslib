using System;
using HarmonyLib;

namespace CitrusLib.Patches
{
    // praying
    [HarmonyPatch(typeof(LandLog), nameof(LandLog.Log), new Type[] { typeof(string), typeof(object) })]
    class LogPatch
    {
        static bool Prefix(string logMessage, object context)
        {
            Citrus.landLog.Log(logMessage);
            return false;
        }
    }

    [HarmonyPatch(typeof(LandLog), nameof(LandLog.LogError))]
    class ErrPatch
    {
        static bool Prefix(string logMessage, object context)
        {
            Citrus.landLog.LogError(logMessage);
            return false;
        }
    }
}
