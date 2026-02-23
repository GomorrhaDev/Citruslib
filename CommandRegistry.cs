using Landfall.Network;
using UnityEngine;
using WobbleBridge.Utils;

namespace WobbleBridge
{
    public static class CommandRegistry
    {
        public static void Register()
        {
            RegisterTeamCommands();
            RegisterTeleportCommands();
            RegisterPlayerInfoCommands();
            RegisterAdminCommands();
            RegisterItemCommands();
            RegisterMiscCommands();
        }

        // ─── Team ────────────────────────────────────────────────────────────────

        private static void RegisterTeamCommands()
        {
            Wobble.AddCommand("team", delegate (string[] prms, TABGPlayerServer player)
            {
                switch (prms[0])
                {
                    case "get":
                        if (prms.Length != 2)
                        {
                            Wobble.SelfParrot(player, "team get <name>");
                            return;
                        }
                        if (!Wobble.PlayerChatSearch(prms[1], out TABGPlayerServer getTarget))
                        {
                            Wobble.SelfParrot(player, getTarget != null
                                ? "multiple results for: " + prms[1]
                                : "no results for: " + prms[1]);
                            return;
                        }
                        Wobble.SelfParrot(player, "groupIndex for player " + getTarget.PlayerName + " is:" + getTarget.GroupIndex);
                        break;

                    case "set":
                        if (prms.Length != 3)
                        {
                            Wobble.SelfParrot(player, "team set <name> <index>");
                            return;
                        }
                        if (!Wobble.PlayerChatSearch(prms[1], out TABGPlayerServer setTarget))
                        {
                            Wobble.SelfParrot(player, setTarget != null
                                ? "multiple results for: " + prms[1]
                                : "no results for: " + prms[1]);
                            return;
                        }
                        if (!byte.TryParse(prms[2], out byte ind))
                        {
                            Wobble.SelfParrot(player, "could not parse group index: " + prms[2]);
                            return;
                        }
                        Wobble.SetTeam(setTarget, ind);
                        Wobble.SelfParrot(player, "set player " + setTarget.PlayerName + " to team " + prms[2]);
                        break;

                    default:
                        Wobble.SelfParrot(player, "unknown parameter: " + prms[0]);
                        break;
                }
            },
            "Wobble Lib", "Changes or queries a player's team", "<get|set> <player> [index](if setting)", 2);
        }

        // ─── Teleport ────────────────────────────────────────────────────────────

        private static void RegisterTeleportCommands()
        {
            Wobble.AddCommand("goto", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "goto <name>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                Wobble.log.Log($"taking player {player.PlayerName} to {find.PlayerName}");
                Wobble.Teleport(player, find.PlayerPosition);
            },
            "Wobble Lib", "Brings the command user to the specified player", "<player>", 2);

            Wobble.AddCommand("bring", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "bring <name>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                Wobble.log.Log($"taking player {find.PlayerName} to {player.PlayerName}");
                Wobble.Teleport(find, player.PlayerPosition);
            },
            "Wobble Lib", "Brings a player to the command user", "<player>", 2);

            Wobble.AddCommand("send", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 2) { Wobble.SelfParrot(player, "send <name> <name>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (!Wobble.PlayerChatSearch(prms[1], out TABGPlayerServer find2))
                {
                    Wobble.SelfParrot(player, find2 != null ? "multiple results for: " + prms[1] : "no results for: " + prms[1]);
                    return;
                }
                Wobble.log.Log($"taking player {find.PlayerName} to {find2.PlayerName}");
                Wobble.Teleport(find, find2.PlayerPosition);
            },
            "Wobble Lib", "Sends the first player to the second player", "<player> <player>", 2);

            Wobble.AddCommand("goto_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 3) { Wobble.SelfParrot(player, "goto_pos <x> <y> <z>"); return; }
                if (!(float.TryParse(prms[0], out float x) & float.TryParse(prms[1], out float y) & float.TryParse(prms[2], out float z)))
                {
                    Wobble.SelfParrot(player, "there was an issue parsing the coordinates you provided!");
                    return;
                }
                Wobble.log.Log($"taking player {player.PlayerName} to {x},{y},{z}");
                Wobble.Teleport(player, new Vector3(x, y, z));
            },
            "Wobble Lib", "teleports the player to the specified coordinates", "(x) (y) (z)", 2);

            Wobble.AddCommand("send_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 4) { Wobble.SelfParrot(player, "send_pos <name/id> <x> <y> <z>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (!(float.TryParse(prms[1], out float x) & float.TryParse(prms[2], out float y) & float.TryParse(prms[3], out float z)))
                {
                    Wobble.SelfParrot(player, "there was an issue parsing the coordinates you provided!");
                    return;
                }
                Wobble.log.Log($"taking player {find.PlayerName} to {x},{y},{z}");
                Wobble.Teleport(find, new Vector3(x, y, z));
            },
            "Wobble Lib", "teleports the specified player to the specified coordinates", "<name> (x) (y) (z)", 2);
        }

        // ─── Player Info ─────────────────────────────────────────────────────────

        private static void RegisterPlayerInfoCommands()
        {
            Wobble.AddCommand("get_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length > 1) { Wobble.SelfParrot(player, "get_pos <name(optional)>"); return; }
                TABGPlayerServer find = player;
                if (prms.Length == 1)
                {
                    if (!Wobble.PlayerChatSearch(prms[0], out find))
                    {
                        Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                        return;
                    }
                }
                string msg = $"player {find.PlayerName} is located at {find.PlayerPosition}";
                Wobble.SelfParrot(player, msg);
                Wobble.log.Log(msg);
            },
            "Wobble Lib", "queries a player's position", "<name>(optional)", 1);

            Wobble.AddCommand("id", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "id <name>"); return; }
                if (!Wobble.PlayerWithName(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"id for {find.PlayerName} is {find.PlayerIndex}";
                Wobble.SelfParrot(player, message);
                Wobble.log.Log(message);
            },
            "Wobble Lib", "Gets the ID of a player with the given name.", "<name>");

            Wobble.AddCommand("name", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "name <id>"); return; }
                if (!byte.TryParse(prms[0], out byte ind)) { Wobble.SelfParrot(player, "name <id>"); return; }
                if (!Wobble.PlayerWithIndex(ind, out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"name for {find.PlayerIndex} is {find.PlayerName}";
                Wobble.SelfParrot(player, message);
                Wobble.log.Log(message);
            },
            "Wobble Lib", "Gets the NAME of a player with the given byte playerindex.", "[id]");

            Wobble.AddCommand("epic", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "epic <name>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"epicid for {find.PlayerName} is {find.EpicUserName}";
                Wobble.SelfParrot(player, message);
                Wobble.log.Log(message);
            },
            "Wobble Lib", "Gets the epic id of a player with the given name or index", "<name>");
        }

        // ─── Admin ───────────────────────────────────────────────────────────────

        private static void RegisterAdminCommands()
        {
            Wobble.AddCommand("perm-get", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Wobble.SelfParrot(player, "perm-get <name>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (Wobble.permList == null) { Wobble.log.Log("Perm list doesn't exist!"); return; }

                PermList.PermPlayer ply = Wobble.permList.players.Find(p => p.epic == (string)find.EpicUserName);
                int perm = ply?.permlevel ?? 1;
                string text = $"{find.PlayerName} has perm level {perm}";
                Wobble.log.Log(text);
                Wobble.SelfParrot(player, text);
            },
            "Wobble Lib", "Gets the permission status of the player", "<player>", 1);

            Wobble.AddCommand("perm-set", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 2) { Wobble.SelfParrot(player, "perm-set <name> [level]"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (Wobble.permList == null)
                {
                    Wobble.SelfParrot(player, "the permlist doesnt exist somehow??");
                    Wobble.log.Log("Perm list doesn't exist!");
                    return;
                }
                // Bugfix: war prms[2], muss prms[1] sein
                if (!int.TryParse(prms[1], out int perm))
                {
                    Wobble.SelfParrot(player, $"Invalid integer {prms[1]}");
                    Wobble.log.Log($"Invalid integer {prms[1]}");
                    return;
                }
                PermList.PermPlayer ply = Wobble.permList.players.Find(p => p.epic == (string)find.EpicUserName);
                if (ply != null)
                {
                    ply.permlevel = perm;
                }
                else
                {
                    Wobble.permList.players.Add(new PermList.PermPlayer
                    {
                        name = find.PlayerName,
                        epic = find.EpicUserName,
                        permlevel = perm
                    });
                }
                Wobble.WriteNewPermList();
            },
            "Wobble Lib", "SETS the permission status of the player!", "<player>", 4);

            Wobble.AddCommand("reloadweps", delegate (string[] prms, TABGPlayerServer player)
            {
                WeaponPatchManager.ReloadConfig();
                Wobble.SelfParrot(player, "Weapon patches reloaded!");
            },
            "Wobble Lib", "Reloads weapon damage multipliers from json", "", 1);
        }

        // ─── Items ───────────────────────────────────────────────────────────────

        private static void RegisterItemCommands()
        {
            Wobble.AddCommand("give", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length < 1 || prms.Length > 2) { Wobble.SelfParrot(player, "give <itemID> <amount>"); return; }
                int amt = 1;
                if (prms.Length == 2 && !int.TryParse(prms[1], out amt)) { Wobble.SelfParrot(player, "invalid amount"); return; }
                if (!ItemHelper.TryParseItemId(prms[0], out int typ)) { Wobble.SelfParrot(player, "invalid item (use ID or Item enum name)"); return; }

                LootPack lp = new LootPack();
                lp.AddLoot(typ, amt);
                Wobble.GiveLoot(player, lp);
            },
            "Wobble Lib", "gives the user an item with an optional amount", "[id|ItemName] [amount(optional)]", 2);

            Wobble.AddCommand("gift", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length < 2 || prms.Length > 3) { Wobble.SelfParrot(player, "gift <player> <itemID> <amount>"); return; }
                if (!Wobble.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                int amt = 1;
                if (prms.Length == 3 && !int.TryParse(prms[2], out amt)) { Wobble.SelfParrot(player, "invalid amount"); return; }
                if (!ItemHelper.TryParseItemId(prms[1], out int typ)) { Wobble.SelfParrot(player, "invalid item (use ID or Item enum name)"); return; }

                LootPack lp = new LootPack();
                lp.AddLoot(typ, amt);
                Wobble.GiveLoot(find, lp);
            },
            "Wobble Lib", "gives the target player an item with an optional amount", "<player> [id|ItemName] [amount(optional)]", 2);
        }

        // ─── Misc ────────────────────────────────────────────────────────────────

        private static void RegisterMiscCommands()
        {
            Wobble.AddCommand("broadcast", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms == null || prms.Length == 0) { Wobble.SelfParrot(player, "broadcast <message>"); return; }
                string msg = "[SERVER] " + string.Join(" ", prms);
                foreach (TABGPlayerServer pl in Wobble.World.GameRoomReference.Players)
                {
                    if (pl != null) Wobble.SelfParrot(pl, msg);
                }
            },
            "Wobble Lib", "Sends a message to all players in the game", "<message>", 3);

            Wobble.AddCommand("kill", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length > 1) { Wobble.SelfParrot(player, "kill <name(optional)>"); return; }
                TABGPlayerServer find = player;
                if (prms.Length == 1)
                {
                    if (!Wobble.PlayerChatSearch(prms[0], out find))
                    {
                        Wobble.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                        return;
                    }
                }
                string msg = $"killing player {find.PlayerName}";
                Wobble.KillPlayer(find);
                Wobble.SelfParrot(player, msg);
                Wobble.log.Log(msg);
            },
            "Wobble Lib", "kills a player. if no player is specified it kills the user...!", "<name>(optional)", 1);

            Wobble.AddCommand("start", delegate (string[] prms, TABGPlayerServer player)
            {
                float time = 30;
                if (prms.Length > 0)
                {
                    if (prms.Length != 1) { Wobble.SelfParrot(player, "start <time(optional)>"); return; }
                    if (!float.TryParse(prms[0], out time)) { Wobble.SelfParrot(player, "invalid time: " + prms[0]); return; }
                }
                Wobble.World.GameRoomReference.StartCountDown(time);
            },
            "Wobble Lib", "Starts the countdown timer", "[time]", 1);

            Wobble.AddCommand("list", delegate (string[] prms, TABGPlayerServer player)
            {
                string players = "PLAYERS:";
                foreach (TABGPlayerServer pl in Wobble.World.GameRoomReference.Players)
                    players += $"\n{pl.PlayerName} ind:{pl.PlayerIndex} team:{pl.GroupIndex} (original team is {(byte)(Wobble.players.Find(p => p.player.PlayerIndex == pl.PlayerIndex).data["originalTeam"])}) epic:{pl.EpicUserName}";

                string playerRefs = "PLAYERREFS:";
                foreach (PlayerRef pl in Wobble.players)
                    playerRefs += $"\n{pl.player.PlayerName} ind:{pl.player.PlayerIndex} team:{pl.player.GroupIndex}";

                string teams = "TEAM REFS:\n";
                string teamsLite = "";
                foreach (PlayerTeam team in Wobble.teams)
                {
                    if (team.players.Count > 0)
                    {
                        teams += $"Team {team.groupIndex} : ";
                        teamsLite += $"Team {team.groupIndex} : ";
                        foreach (PlayerRef p in team.players) { teams += p.player.PlayerName + ", "; teamsLite += p.player.PlayerName + ", "; }
                        teams += "\n"; teamsLite += "\n";
                    }
                    else teams += $"Team {team.groupIndex} : (no players)\n";
                }

                string req = prms.Length != 0 ? prms[0] : "all";
                string result;
                switch (req)
                {
                    case "teams": case "team": case "t":
                        result = "TEAMS LIST INFO\n" + teams;
                        Wobble.SelfParrot(player, teamsLite);
                        break;
                    case "players": case "player": case "p":
                        result = "PLAYERS LIST INFO\n" + players;
                        Wobble.SelfParrot(player, players);
                        break;
                    case "playerrefs":
                        result = "PLAYERREFS LIST INFO\n" + playerRefs;
                        Wobble.SelfParrot(player, playerRefs);
                        break;
                    default:
                        result = "ALL LIST INFO\n" + players + "\n" + teams + "\n" + playerRefs;
                        Wobble.SelfParrot(player, "lists posted to console.");
                        break;
                }
                Wobble.log.Log(result);
            },
            "Wobble Lib", "lists different things in the console.", "<teams|players|playerrefs|all>", 2);
        }
    }
}