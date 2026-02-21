using System;
using HarmonyLib;
using Landfall.Network;

namespace CitrusLib.Patches
{
    // does stuff early but AFTER other mods have registered their citlib chat commands
    [HarmonyPatch(typeof(GameRoom), "InitActions")]
    class StartPatch
    {
        static void Prefix()
        {
            Citrus.WriteAllCommands();
            Citrus.ExtraSettings.ReadSettings();
            GuestBook.LoadGuestBook();

            CustomLootTables.ReadLoot(); // reads loot tables!

            GameSetting supp;
            if (Citrus.ExtraSettings.TryGetSetting("Suppress Landlog", out supp))
            {
                bool sup = false;
                if (Boolean.TryParse(supp.value, out sup))
                {
                    Citrus.landLogSupressed = sup;
                }
            }
            else
            {
                Citrus.log.Log("Log supress setting is missing?");
            }

            GameSetting adminLoc;
            if (Citrus.ExtraSettings.TryGetSetting("AdminFileLocation", out adminLoc))
            {
                string p = "";
                if (adminLoc.value != "")
                {
                    Citrus.log.Log("Loading player perms at " + adminLoc.value);
                    p = adminLoc.value + "/PlayerPerms";
                }
                Citrus.LoadPermSettings(p);
            }

            // bool dis = false;
            GameSetting disCom = null;
            if (Citrus.ExtraSettings.TryGetSetting("Disable Missing Command Parrot", out disCom))
            {
                bool sup = false;
                if (Boolean.TryParse(disCom.value, out sup))
                {
                    Citrus.disableMissingCommandParrot = sup;
                }
            }
        }
    }
}
