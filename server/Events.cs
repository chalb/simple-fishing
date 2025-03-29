using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Enums;
using System;
using AltV.Net.CApi.ClientEvents;
using AltV.Net.Data;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Events : IScript
{
    private readonly Random RNG = new Random();

    [ScriptEvent(ScriptEventType.PlayerConnect)]
    public void OnPlayerConnect(Player player, string reason)
    {
        Alt.Log($"{player.Name} has connected.");

        // Setzt den Spawnpunkt in der Nähe der ersten Fishing-Zone
        player.Spawn(new AltV.Net.Data.Position(319, -2236, 6), 0);
        player.Model = (uint)PedModel.Fabien;
        player.Emit("ShowNotification", "Welcome! You are near a fishing zone.");
    }


    [ScriptEvent(ScriptEventType.ColShape)] 
    public static void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
    {     
        switch (entity)
        {
            case IPlayer player:
                if (state) // Spieler betritt die Zone
                {
                    if (shape.HasData("FishingZone"))
                    {
                        player.SetData("InsideFishingZone", true);
                        if (!player.HasData("MayFish"))
                        {
                            player.SetData("MayFish", true);
                            player.Emit("PlayerMayFish", true);
                        }
                        player.Emit("SetFishingZone", true);
                        player.Emit("ShowNotification", "You are in a fishing zone! Aim at Water and Press [E] to start fishing.");
                        Alt.Log($"{player.Name} entered the fishing zone.");
                    }
                }
                else // Spieler verlässt die Zone
                {
                    if (shape.HasData("FishingZone"))
                    {
                        player.DeleteData("InsideFishingZone");
                        player.Emit("SetFishingZone", false);
                        player.Emit("ShowNotification", "You left the fishing zone.");                        
                        Alt.Log($"{player.Name} left the fishing zone.");
                    }
                }
                break;
        }
    }

    [ClientEvent("SelectedFishingSpot")]
    public void SelectedFishingSpot(Player player, Position position)
    {
        if (!player.HasData("InsideFishingZone"))
        {
            player.Emit("ShowNotification", "You are not in a fishing zone!");
            player.DeleteData("SelectedFishingSpot");
            return;
        }

        player.SetData("SelectedFishingSpot", position);
        Alt.Log($"{player.Name} set fishing zone @ {position}");
        player.Emit("ShowNotification", "Fishing spot selected! Press [E] to start fishing.");
    }

    [CommandEvent(CommandEventType.CommandNotFound)]
    public void OnCommandNotFound(IPlayer iplayer, string command)
    {
        iplayer.SendChatMessage("{FF0000}Befehl " + command + " nicht gefunden");
    }

    [Command("car")]
    public void Cmd_car(IPlayer iplayer, string VehicleName, int R = 0, int G = 0, int B = 0)
    {   try
        {
            IVehicle veh = Alt.CreateVehicle(VehicleName, new AltV.Net.Data.Position(iplayer.Position.X, iplayer.Position.Y + 1.0f, iplayer.Position.Z), iplayer.Rotation);
            if(veh != null)
            {
                veh.PrimaryColorRgb = new AltV.Net.Data.Rgba((byte)R, (byte)G, (byte)B, 255);
                iplayer.SendChatMessage("{04B404}Fahrzeug gespawned!");
            }
            else
            {
                iplayer.SendChatMessage("{FF0000}Fahrzeug " + VehicleName + " konnte nicht gespawned werden!");
            }
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    [Command("pos")]
    public void Cmd_pos(IPlayer iplayer)
    {   
        iplayer.SendChatMessage("{04B404}Du bist bei: " +iplayer.Position.X + ", " + iplayer.Position.Y + ", " + iplayer.Position.Z);
    }


    [Command("fishpos")]
    public void Cmd_fishpos(IPlayer iplayer)
    {   
        iplayer.SendChatMessage("{04B404}Installiere neuen Fishing Spot bei: " +iplayer.Position.X + ", " + iplayer.Position.Y + ", " + iplayer.Position.Z);
    }


    [ClientEvent("StartFishing")]
    public void StartFishing(IPlayer player)
    {
        if (!player.GetData("InsideFishingZone", out bool insideZone) || !insideZone)
        {
            player.Emit("ShowNotification", "You must be in a fishing zone!");
            return;
        }
        if (!player.GetData("MayFish", out bool mayFish) || !mayFish)
        {
            player.Emit("ShowNotification", "You can't fish right now, hold on!");
            return;
        }
            return;
        } */
        if(player.IsInWater)
        {
            player.Emit("ShowNotification", "Do not stay in Water! " + player.IsInWater);
            return;
        }
        player.SendChatMessage("{FF0000}Start Fishing!");
        player.Emit("ShowNotification", "Starting Fishing!");
        player.Emit("PlayFishingAnimation");
        _ = DelayFishing(player);
        
    }

    private async Task DelayFishing(IPlayer player)
    {
        Alt.Log($"{player.Name} started fishing.");
        player.SetData("MayFish", false);
        player.Emit("PlayerMayFish", false);
        await Task.Delay(10000);
        
        int chance = RNG.Next(5);

        string stateMsg = chance switch
        {
            0 => "a catch Karp",
            1 => "a catch shrimp",
            2 => "a catch Shoe",
            _ => "nothing"
        };

        player.Emit("ShowNotification", "You got" + stateMsg + "!");
        await Task.Delay(50000);
        
        player.SetData("MayFish", true);
        player.Emit("PlayerMayFish", true);
        Alt.Log($"{player.Name} finished fishing.");
    }
}

