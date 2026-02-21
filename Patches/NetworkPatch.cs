using HarmonyLib;
using Landfall.Network;
using Unity.Networking.Transport;

namespace CitrusLib.Patches
{
    // grabs the Network object when the game is hosted
    [HarmonyPatch(typeof(UnityTransportServer), "ActuallyHost")]
    class HostPatch
    {
        static void Prefix(ref NetworkSettings networkSettings, bool ___m_isHosting)
        {
            if (___m_isHosting)
            {
                // Citrus.WriteAllCommands();
                // not the best place to do this but WHO CARES

                // Citrus.log.Log("trying to increase buffer sizes tremendously!!");
                // networkSettings = networkSettings.WithBaselibNetworkInterfaceParameters(2048, 2048, 4000U);
            }
        }

        static void Postfix(ref NetworkDriver ___m_ServerHandler)
        {
            Citrus.Network = ___m_ServerHandler;
        }
    }

    // [HarmonyPatch(typeof(NetworkDriver), nameof(NetworkDriver.Create), typeof(NetworkSettings))]
    class NetworkPatch
    {
        static void Prefix(ref NetworkSettings settings)
        {
            // settings = settings.WithBaselibNetworkInterfaceParameters(2048, 2048, 4000U);
        }
    }
}
