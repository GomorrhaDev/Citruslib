using System;
using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{
    // does stuff early but AFTER other mods have registered their WobbleBridge chat commands
    [HarmonyPatch(typeof(GameRoom), "InitActions")]
    class StartPatch
    {
        static void Prefix()
        {
            Wobble.WriteAllCommands();
            Wobble.ExtraSettings.ReadSettings();
            GuestBook.LoadGuestBook();

            CustomLootTables.ReadLoot(); // reads loot tables!

            GameSetting supp;
            if (Wobble.ExtraSettings.TryGetSetting("Suppress Landlog", out supp))
            {
                bool sup = false;
                if (Boolean.TryParse(supp.value, out sup))
                {
                    Wobble.landLogSupressed = sup;
                }
            }
            else
            {
                Wobble.log.Log("Log supress setting is missing?");
            }

            GameSetting adminLoc;
            if (Wobble.ExtraSettings.TryGetSetting("AdminFileLocation", out adminLoc))
            {
                string p = "";
                if (adminLoc.value != "")
                {
                    Wobble.log.Log("Loading player perms at " + adminLoc.value);
                    p = adminLoc.value + "/PlayerPerms";
                }
                Wobble.LoadPermSettings(p);
            }

            // bool dis = false;
            GameSetting disCom = null;
            if (Wobble.ExtraSettings.TryGetSetting("Disable Missing Command Parrot", out disCom))
            {
                bool sup = false;
                if (Boolean.TryParse(disCom.value, out sup))
                {
                    Wobble.disableMissingCommandParrot = sup;
                }
            }
        }
    }
}
