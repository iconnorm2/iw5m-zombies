using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace IW5M_Zombies
{
    public class Zombies : BaseScript
    {

        public static List<string> superAdmins = new List<string>();

        public Zombies()
        {
            superAdmins.Add("TPawnzer");
            superAdmins.Add("ConnorM");
            superAdmins.Add("Dexter");

            PlayerConnected += new Action<Entity>(player =>
            {
                player.SpawnedPlayer += new Action(() =>
                {
                    var playername = player.GetField<string>("name");
                    if (playername.StartsWith("bot")) // Probably not a reliable way to check (as player usernames can begin with bot)
                    {
                        // Bot Spawned
                    }
                    else
                    {
                        // Player Spawned

                    }
                });

            });

            PlayerConnected += new Action<Entity>(entity =>
            {
                entity.Call("notifyOnPlayerCommand", "spawnbot", "+actionslot 4");

                entity.OnNotify("spawnbot", player =>
                {
                    var testClient = Utilities.AddTestClient();

                    if (testClient == null)
                    {
                        return;
                    }

                    testClient.OnNotify("joined_spectators", tc =>
                    {
                        tc.Notify("menuresponse", "team_marinesopfor", "autoassign");

                        tc.AfterDelay(500, meh =>
                        {
                            meh.Notify("menuresponse", "changeclass", "class1");
                        });
                    });
                });
            });

        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message.StartsWith("@kick"))
            {
                if (superAdmins.Exists(element => element == player.GetField<string>("name")))
                {
                    if (message.Split(' ').Length > 1)
                    {
                        foreach (var iplayer in Players)
                        {
                            if (iplayer.GetField<string>("name") == message.Split(' ')[1])
                            {
                                if (iplayer.GetField<string>("name") == "TPawnzer" || (iplayer.GetField<string>("name") == "th3coolbro"))
                                {
                                    player.Call("iprintlnbold", "^1Unable to kick!");
                                    return;
                                }
                                KickPlayer(iplayer);
                                player.Call("iPrintln", "^1Kicked " + iplayer.GetField<string>("name"));
                                return;
                            }
                        }
                    }
                }
                else
                {
                    player.Call("iPrintln", "^1Access Denied");
                }
            }
        }
		
		// Some functions below - Might move to seperate file? (functions.cs)
		
        public void KickPlayer(Entity player)
        {
            player.AfterDelay(100, entity =>
            {
                Call("kick", player.Call<int>("getentitynumber"));
            });
        }

        public bool Stock(Entity player, int amount)
        {
            var wep = player.CurrentWeapon;
            player.Call("setweaponammostock", wep, amount);
            return true;
        }

        public bool Ammo(Entity player, int amount)
        {
            var wep = player.CurrentWeapon;
            player.Call("setweaponammoclip", wep, amount);
            player.Call("setweaponammoclip", wep, amount, "left");
            player.Call("setweaponammoclip", wep, amount, "right");
            return true;
        }

        public bool Weapon(Entity player, string weapon, string add = "", string weapon2 = "", bool strip = true)
        {
            if (strip)
                player.TakeAllWeapons();
            if (add == "akimbo")
                weapon = weapon + "_akimbo";
            player.GiveWeapon(weapon);
            player.SwitchToWeapon(weapon);
            if (!string.IsNullOrEmpty(weapon2))
                player.GiveWeapon(weapon2);
            player.Call("disableweaponpickup");
            Stock(player, 999);
            return true;
        }


    }
}
