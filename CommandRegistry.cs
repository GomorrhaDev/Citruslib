using Landfall.Network;
using UnityEngine;

namespace CitrusLib
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
            Citrus.AddCommand("team", delegate (string[] prms, TABGPlayerServer player)
            {
                switch (prms[0])
                {
                    case "get":
                        if (prms.Length != 2)
                        {
                            Citrus.SelfParrot(player, "team get <name>");
                            return;
                        }
                        if (!Citrus.PlayerChatSearch(prms[1], out TABGPlayerServer getTarget))
                        {
                            Citrus.SelfParrot(player, getTarget != null
                                ? "multiple results for: " + prms[1]
                                : "no results for: " + prms[1]);
                            return;
                        }
                        Citrus.SelfParrot(player, "groupIndex for player " + getTarget.PlayerName + " is:" + getTarget.GroupIndex);
                        break;

                    case "set":
                        if (prms.Length != 3)
                        {
                            Citrus.SelfParrot(player, "team set <name> <index>");
                            return;
                        }
                        if (!Citrus.PlayerChatSearch(prms[1], out TABGPlayerServer setTarget))
                        {
                            Citrus.SelfParrot(player, setTarget != null
                                ? "multiple results for: " + prms[1]
                                : "no results for: " + prms[1]);
                            return;
                        }
                        if (!byte.TryParse(prms[2], out byte ind))
                        {
                            Citrus.SelfParrot(player, "could not parse group index: " + prms[2]);
                            return;
                        }
                        Citrus.SetTeam(setTarget, ind);
                        Citrus.SelfParrot(player, "set player " + setTarget.PlayerName + " to team " + prms[2]);
                        break;

                    default:
                        Citrus.SelfParrot(player, "unknown parameter: " + prms[0]);
                        break;
                }
            },
            "Citrus Lib", "Changes or queries a player's team", "<get|set> <player> [index](if setting)", 2);
        }

        // ─── Teleport ────────────────────────────────────────────────────────────

        private static void RegisterTeleportCommands()
        {
            Citrus.AddCommand("goto", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "goto <name>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                Citrus.log.Log($"taking player {player.PlayerName} to {find.PlayerName}");
                Citrus.Teleport(player, find.PlayerPosition);
            },
            "Citrus Lib", "Brings the command user to the specified player", "<player>", 2);

            Citrus.AddCommand("bring", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "bring <name>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                Citrus.log.Log($"taking player {find.PlayerName} to {player.PlayerName}");
                Citrus.Teleport(find, player.PlayerPosition);
            },
            "Citrus Lib", "Brings a player to the command user", "<player>", 2);

            Citrus.AddCommand("send", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 2) { Citrus.SelfParrot(player, "send <name> <name>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (!Citrus.PlayerChatSearch(prms[1], out TABGPlayerServer find2))
                {
                    Citrus.SelfParrot(player, find2 != null ? "multiple results for: " + prms[1] : "no results for: " + prms[1]);
                    return;
                }
                Citrus.log.Log($"taking player {find.PlayerName} to {find2.PlayerName}");
                Citrus.Teleport(find, find2.PlayerPosition);
            },
            "Citrus Lib", "Sends the first player to the second player", "<player> <player>", 2);

            Citrus.AddCommand("goto_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 3) { Citrus.SelfParrot(player, "goto_pos <x> <y> <z>"); return; }
                if (!(float.TryParse(prms[0], out float x) & float.TryParse(prms[1], out float y) & float.TryParse(prms[2], out float z)))
                {
                    Citrus.SelfParrot(player, "there was an issue parsing the coordinates you provided!");
                    return;
                }
                Citrus.log.Log($"taking player {player.PlayerName} to {x},{y},{z}");
                Citrus.Teleport(player, new Vector3(x, y, z));
            },
            "Citrus Lib", "teleports the player to the specified coordinates", "(x) (y) (z)", 2);

            Citrus.AddCommand("send_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 4) { Citrus.SelfParrot(player, "send_pos <name/id> <x> <y> <z>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (!(float.TryParse(prms[1], out float x) & float.TryParse(prms[2], out float y) & float.TryParse(prms[3], out float z)))
                {
                    Citrus.SelfParrot(player, "there was an issue parsing the coordinates you provided!");
                    return;
                }
                Citrus.log.Log($"taking player {find.PlayerName} to {x},{y},{z}");
                Citrus.Teleport(find, new Vector3(x, y, z));
            },
            "Citrus Lib", "teleports the specified player to the specified coordinates", "<name> (x) (y) (z)", 2);
        }

        // ─── Player Info ─────────────────────────────────────────────────────────

        private static void RegisterPlayerInfoCommands()
        {
            Citrus.AddCommand("get_pos", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length > 1) { Citrus.SelfParrot(player, "get_pos <name(optional)>"); return; }
                TABGPlayerServer find = player;
                if (prms.Length == 1)
                {
                    if (!Citrus.PlayerChatSearch(prms[0], out find))
                    {
                        Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                        return;
                    }
                }
                string msg = $"player {find.PlayerName} is located at {find.PlayerPosition}";
                Citrus.SelfParrot(player, msg);
                Citrus.log.Log(msg);
            },
            "Citrus Lib", "queries a player's position", "<name>(optional)", 1);

            Citrus.AddCommand("id", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "id <name>"); return; }
                if (!Citrus.PlayerWithName(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"id for {find.PlayerName} is {find.PlayerIndex}";
                Citrus.SelfParrot(player, message);
                Citrus.log.Log(message);
            },
            "Citrus Lib", "Gets the ID of a player with the given name.", "<name>");

            Citrus.AddCommand("name", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "name <id>"); return; }
                if (!byte.TryParse(prms[0], out byte ind)) { Citrus.SelfParrot(player, "name <id>"); return; }
                if (!Citrus.PlayerWithIndex(ind, out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"name for {find.PlayerIndex} is {find.PlayerName}";
                Citrus.SelfParrot(player, message);
                Citrus.log.Log(message);
            },
            "Citrus Lib", "Gets the NAME of a player with the given byte playerindex.", "[id]");

            Citrus.AddCommand("epic", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "epic <name>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                string message = $"epicid for {find.PlayerName} is {find.EpicUserName}";
                Citrus.SelfParrot(player, message);
                Citrus.log.Log(message);
            },
            "Citrus Lib", "Gets the epic id of a player with the given name or index", "<name>");
        }

        // ─── Admin ───────────────────────────────────────────────────────────────

        private static void RegisterAdminCommands()
        {
            Citrus.AddCommand("perm-get", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 1) { Citrus.SelfParrot(player, "perm-get <name>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (Citrus.permList == null) { Citrus.log.Log("Perm list doesn't exist!"); return; }

                PermList.PermPlayer ply = Citrus.permList.players.Find(p => p.epic == (string)find.EpicUserName);
                int perm = ply?.permlevel ?? 1;
                string text = $"{find.PlayerName} has perm level {perm}";
                Citrus.log.Log(text);
                Citrus.SelfParrot(player, text);
            },
            "Citrus Lib", "Gets the permission status of the player", "<player>", 1);

            Citrus.AddCommand("perm-set", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length != 2) { Citrus.SelfParrot(player, "perm-set <name> [level]"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                if (Citrus.permList == null)
                {
                    Citrus.SelfParrot(player, "the permlist doesnt exist somehow??");
                    Citrus.log.Log("Perm list doesn't exist!");
                    return;
                }
                // Bugfix: war prms[2], muss prms[1] sein
                if (!int.TryParse(prms[1], out int perm))
                {
                    Citrus.SelfParrot(player, $"Invalid integer {prms[1]}");
                    Citrus.log.Log($"Invalid integer {prms[1]}");
                    return;
                }
                PermList.PermPlayer ply = Citrus.permList.players.Find(p => p.epic == (string)find.EpicUserName);
                if (ply != null)
                {
                    ply.permlevel = perm;
                }
                else
                {
                    Citrus.permList.players.Add(new PermList.PermPlayer
                    {
                        name = find.PlayerName,
                        epic = find.EpicUserName,
                        permlevel = perm
                    });
                }
                Citrus.WriteNewPermList();
            },
            "Citrus Lib", "SETS the permission status of the player!", "<player>", 4);

            Citrus.AddCommand("reloadweps", delegate (string[] prms, TABGPlayerServer player)
            {
                WeaponPatchManager.ReloadConfig();
                Citrus.SelfParrot(player, "Weapon patches reloaded!");
            },
            "Citrus Lib", "Reloads weapon damage multipliers from json", "", 1);
        }

        // ─── Items ───────────────────────────────────────────────────────────────

        private static void RegisterItemCommands()
        {
            Citrus.AddCommand("give", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length < 1 || prms.Length > 2) { Citrus.SelfParrot(player, "give <itemID> <amount>"); return; }
                int amt = 1;
                if (prms.Length == 2 && !int.TryParse(prms[1], out amt)) { Citrus.SelfParrot(player, "invalid amount"); return; }
                if (!ItemHelper.TryParseItemId(prms[0], out int typ)) { Citrus.SelfParrot(player, "invalid item (use ID or Item enum name)"); return; }

                LootPack lp = new LootPack();
                lp.AddLoot(typ, amt);
                Citrus.GiveLoot(player, lp);
            },
            "Citrus Lib", "gives the user an item with an optional amount", "[id|ItemName] [amount(optional)]", 2);

            Citrus.AddCommand("gift", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length < 2 || prms.Length > 3) { Citrus.SelfParrot(player, "gift <player> <itemID> <amount>"); return; }
                if (!Citrus.PlayerChatSearch(prms[0], out TABGPlayerServer find))
                {
                    Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                    return;
                }
                int amt = 1;
                if (prms.Length == 3 && !int.TryParse(prms[2], out amt)) { Citrus.SelfParrot(player, "invalid amount"); return; }
                if (!ItemHelper.TryParseItemId(prms[1], out int typ)) { Citrus.SelfParrot(player, "invalid item (use ID or Item enum name)"); return; }

                LootPack lp = new LootPack();
                lp.AddLoot(typ, amt);
                Citrus.GiveLoot(find, lp);
            },
            "Citrus Lib", "gives the target player an item with an optional amount", "<player> [id|ItemName] [amount(optional)]", 2);
        }

        // ─── Misc ────────────────────────────────────────────────────────────────

        private static void RegisterMiscCommands()
        {
            Citrus.AddCommand("broadcast", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms == null || prms.Length == 0) { Citrus.SelfParrot(player, "broadcast <message>"); return; }
                string msg = "[SERVER] " + string.Join(" ", prms);
                foreach (var pr in Citrus.players)
                {
                    if (pr?.player != null) Citrus.SelfParrot(pr.player, msg);
                }
            },
            "Citrus Lib", "Sends a message to all players in the game", "<message>", 3);

            Citrus.AddCommand("kill", delegate (string[] prms, TABGPlayerServer player)
            {
                if (prms.Length > 1) { Citrus.SelfParrot(player, "kill <name(optional)>"); return; }
                TABGPlayerServer find = player;
                if (prms.Length == 1)
                {
                    if (!Citrus.PlayerChatSearch(prms[0], out find))
                    {
                        Citrus.SelfParrot(player, find != null ? "multiple results for: " + prms[0] : "no results for: " + prms[0]);
                        return;
                    }
                }
                string msg = $"killing player {find.PlayerName}";
                Citrus.KillPlayer(find);
                Citrus.SelfParrot(player, msg);
                Citrus.log.Log(msg);
            },
            "Citrus Lib", "kills a player. if no player is specified it kills the user...!", "<name>(optional)", 1);

            Citrus.AddCommand("start", delegate (string[] prms, TABGPlayerServer player)
            {
                float time = 30;
                if (prms.Length > 0)
                {
                    if (prms.Length != 1) { Citrus.SelfParrot(player, "start <time(optional)>"); return; }
                    if (!float.TryParse(prms[0], out time)) { Citrus.SelfParrot(player, "invalid time: " + prms[0]); return; }
                }
                Citrus.World.GameRoomReference.StartCountDown(time);
            },
            "Citrus Lib", "Starts the countdown timer", "[time]", 1);

            Citrus.AddCommand("list", delegate (string[] prms, TABGPlayerServer player)
            {
                string players = "PLAYERS:";
                foreach (TABGPlayerServer pl in Citrus.World.GameRoomReference.Players)
                    players += $"\n{pl.PlayerName} ind:{pl.PlayerIndex} team:{pl.GroupIndex} (original team is {(byte)(Citrus.players.Find(p => p.player.PlayerIndex == pl.PlayerIndex).data["originalTeam"])}) epic:{pl.EpicUserName}";

                string playerRefs = "PLAYERREFS:";
                foreach (PlayerRef pl in Citrus.players)
                    playerRefs += $"\n{pl.player.PlayerName} ind:{pl.player.PlayerIndex} team:{pl.player.GroupIndex}";

                string teams = "TEAM REFS:\n";
                string teamsLite = "";
                foreach (PlayerTeam team in Citrus.teams)
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
                        Citrus.SelfParrot(player, teamsLite);
                        break;
                    case "players": case "player": case "p":
                        result = "PLAYERS LIST INFO\n" + players;
                        Citrus.SelfParrot(player, players);
                        break;
                    case "playerrefs":
                        result = "PLAYERREFS LIST INFO\n" + playerRefs;
                        Citrus.SelfParrot(player, playerRefs);
                        break;
                    default:
                        result = "ALL LIST INFO\n" + players + "\n" + teams + "\n" + playerRefs;
                        Citrus.SelfParrot(player, "lists posted to console.");
                        break;
                }
                Citrus.log.Log(result);
            },
            "Citrus Lib", "lists different things in the console.", "<teams|players|playerrefs|all>", 2);
        }
    }
}