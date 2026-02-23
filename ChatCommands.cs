using System;
using System.Collections.Generic;
using System.IO;
using Landfall.Network;
using UnityEngine;
using Newtonsoft.Json;

namespace CitrusLib
{
    public static partial class Citrus
    {
        internal static PermList permList;

        static SettingsFile<PermList> perms;

        static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public static bool disableMissingCommandParrot = false;

        internal static void WriteNewPermList()
        {
            perms.settings = new List<PermList>() { permList };
            perms.WriteSettings();
        }

        internal static void LoadPermSettings(string path)
        {
            bool absolute = true;
            if (path == "")
            {
                absolute = false;
                path = "PlayerPerms";
            }

            perms = new SettingsFile<PermList>(path, absolute);

            PermList defaultPermList = new PermList();
            defaultPermList.name = "players";
            defaultPermList.description = "List of players with modified permission level. Default permission level is 0.";

            PermList.PermPlayer dp = new PermList.PermPlayer();
            dp.epic = "epic goes here";
            dp.name = "player name (does not need to be exact!)";
            dp.permlevel = 1;

            defaultPermList.players.Add(dp);
            defaultPermList.players.Add(dp);

            perms.AddSetting(defaultPermList);
            perms.ReadSettings();

            if (perms.TryGetSetting("players", out permList))
            {
                Citrus.log.Log("Player Perm List loaded!");
                foreach (PermList.PermPlayer ply in permList.players)
                    Citrus.log.Log($"{ply.name} ({ply.epic}), PermLevel: {ply.permlevel}");
            }
            else
            {
                Citrus.log.LogError("Missing player permission list!");
            }
        }

        /// <summary>
        /// Adds a chat command that can be triggered by multiple names.
        /// </summary>
        public static void AddCommand(string[] names, Action<string[], TABGPlayerServer> function, string modName = "", string description = "", string paramDesc = "")
        {
            foreach (string name in names)
            {
                AddCommand(name, function, modName, description, paramDesc);
                paramDesc   = "";
                description = "";
            }
        }

        /// <summary>
        /// Adds a chat command with a single name.
        /// </summary>
        public static void AddCommand(string name, Action<string[], TABGPlayerServer> function, string modName = "", string description = "", string paramDesc = "", int permLevel = 1)
        {
            if (commands.ContainsKey(name))
            {
                Citrus.log.LogError($"Command \"{name}\" is already registered by another mod!");
                return;
            }
            if (function == null)
            {
                Citrus.log.LogError($"Refusing to register command \"{name}\" — it has no function!");
                return;
            }

            commands.Add(name, new Command(name, function, modName, description, paramDesc, permLevel));
            Citrus.log.Log($"[Commands] Registered command: \"{name}\" (mod: {modName}, permLevel: {permLevel})");
        }

        internal static bool RunCommand(string name, string[] prms, TABGPlayerServer player)
        {

            if (commands.TryGetValue(name, out Command cmd))
            {
                return cmd.Run(prms, player);
            }
            else
            {
                Citrus.log.LogWarning($"[Commands] Unknown command: \"{name}\" — registered commands: [{string.Join(", ", commands.Keys)}]");

                if (!disableMissingCommandParrot)
                    Citrus.SelfParrot(player, "unknown command: " + name);

                return false;
            }
        }

        static int CommandCompare(Command a, Command b) => a.permLevel.CompareTo(b.permLevel);

        internal static void WriteAllCommands()
        {
            string f = "COMMANDS LIST:";
            Citrus.log.Log("Writing Command List to file!");

            Dictionary<string, List<Command>> sort = new Dictionary<string, List<Command>>();

            foreach (Command c in commands.Values)
            {
                string key = c.modName != "" ? c.modName : "Unnamed Commands";
                if (!sort.ContainsKey(key))
                    sort.Add(key, new List<Command>());
                sort[key].Add(c);
            }

            foreach (List<Command> lc in sort.Values)
            {
                f += "\n\n";
                f += "======" + (lc[0].modName != "" ? lc[0].modName : "General\n") + "======";
                lc.Sort(CommandCompare);
                foreach (Command c in lc)
                {
                    f += "\n\n";
                    f += c.ToString();
                }
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
            string text = Path.Combine(directoryInfo.Parent.FullName, "Commands_List.txt");
            File.WriteAllText(text, f);
        }
    }


    [Serializable]
    internal class PermList : SettingObject
    {
        [JsonProperty(Order = 1)]
        public List<PermPlayer> players = new List<PermPlayer>();

        [Serializable]
        public class PermPlayer
        {
            public string name;
            public string epic;
            public int permlevel;
        }
    }


    class Command
    {
        string name;
        public Action<string[], TABGPlayerServer> func;
        string description;
        string paramDesc;
        public readonly string modName;
        public readonly int permLevel;

        public Command(string n, Action<string[], TABGPlayerServer> f, string mName = "", string desc = "", string pDesc = "", int plev = 1)
        {
            name        = n;
            func        = f;
            description = desc;
            paramDesc   = pDesc;
            modName     = mName;
            permLevel   = plev;
        }

        public override string ToString()
        {
            string ret = description;
            if (ret != "") ret += "\n";
            ret += $"Perm Level: {permLevel}\n";
            ret += "/" + name + " " + paramDesc;
            return ret;
        }

        public bool Run(string[] prms, TABGPlayerServer player)
        {
            if (player == null)
            {
                Citrus.log.LogWarning($"[Commands] Command \"{name}\" called with null player — aborting.");
                return false;
            }

            int plev = 0;

            if (Citrus.permList != null)
            {
                PermList.PermPlayer ply = Citrus.permList.players.Find(p => p.epic == player.EpicUserName);
                if (ply != null) plev = ply.permlevel;
            }
            else
            {
                Citrus.log.LogWarning($"[Commands] permList is null — treating player {player.PlayerName} as perm level 0.");
            }
            
            if (plev < permLevel)
            {
                Citrus.log.Log($"[Commands] Player \"{player.PlayerName}\" lacks permission for \"{name}\" (has {plev}, needs {permLevel}).");
                if (!Citrus.disableMissingCommandParrot)
                    Citrus.SelfParrot(player, $"Command \"{name}\" requires a higher permission level.");
                return false;
            }

            Citrus.log.Log($"[Commands] Executing \"{name}\" for player \"{player.PlayerName}\".");
            func(prms, player);
            return true;
        }
    }
}