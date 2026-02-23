using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{

//required for kicking players properly. dont ask why.
    [HarmonyPatch(typeof(TABGPlayerServer), nameof(TABGPlayerServer.Kill))]
    public class KillPatch
    {
        static void Prefix(TABGPlayerServer __instance)
        {
            PlayerRef pref = Wobble.players.Find(pl => pl.player == __instance);

            if (pref == null)
            {
                Wobble.log.LogError("no playerref found for respawning player??");
                return;
            }

            pref.data["aliveAware"] = false;


        }
    }
}