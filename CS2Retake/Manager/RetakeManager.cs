﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;
using System.Security.Cryptography;

namespace CS2Retake.Manager
{
    public class RetakeManager
    {
        private static RetakeManager _instance;
        public string ModuleName { get; set; }


        public static RetakeManager GetInstance()
        {
            if(_instance == null)
            {
                _instance = new RetakeManager();
            }
            return _instance;
        }

        private RetakeManager() {}

        public void ScrambleTeams()
        {
            var nonSpectatingValidPlayers = this.GetPlayerControllers().Where(x => x.IsValid && (x.TeamNum == (int)CsTeam.Terrorist || x.TeamNum == (int)CsTeam.CounterTerrorist)).ToList();

            if(!nonSpectatingValidPlayers.Any()) 
            {
                this.Log($"No valid non spectating players have been found!");
                return;
            }

            var random = new Random();
            nonSpectatingValidPlayers = nonSpectatingValidPlayers.OrderBy(x => random.Next()).ToList();

            for(int i = 0; i < nonSpectatingValidPlayers.Count; i++) 
            {
                nonSpectatingValidPlayers[i].SwitchTeam(i % 2 == 0 ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
            }
        }

        public void PlantBomb()
        {
            var terroristPlayers = this.GetPlayerControllers().Where(x => x.IsValid && x.TeamNum == (int)CsTeam.Terrorist).ToList();

            if (!terroristPlayers.Any())
            {
                this.Log($"No terrorist players have been found!");
                return;
            }

            var random = new Random();
            var bombCarrier = terroristPlayers.OrderBy(x => random.Next()).Where(x => x.PlayerPawn.Value.InBombZone).FirstOrDefault();

            if(bombCarrier == null)
            {
                this.Log($"No terrorist players in bombzone found!");
                return;
            }
        }

        public void ConfigureForRetake()
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        private List<CCSPlayerController> GetPlayerControllers() 
        {
            var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").ToList();

            if (!playerList.Any())
            {
                this.Log($"No Players have been found!");
            }

            return playerList;
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }

    }
}