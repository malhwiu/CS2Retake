﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entities;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CS2Retake.Allocators
{
    public class WeaponKitAllocator
    {
        private List<WeaponKitEntity> _weaponKitEntityList = new List<WeaponKitEntity>();

        private string _moduleDirectory { get; set; }

        public WeaponKitAllocator(string moduleDirectoy)
        {
            this._moduleDirectory = moduleDirectoy;
        }

        public bool Allocate(CCSPlayerController player)
        {
            if(player == null) 
            {
                return false;
            }

            if(!this._weaponKitEntityList.Any())
            {
                this.LoadWeaponKits();
            }

            var availableWeaponKitsForPlayer = this._weaponKitEntityList.Where(x => (x.Team == CsTeam.None || x.Team == (CsTeam)player.TeamNum));

            return true;
        }

        private void LoadWeaponKits()
        { 
            var path = this.GetPath();

            var pathExists = Path.Exists(path);

            if (pathExists)
            {
                var jsonWeaponKits = File.ReadAllText(path);

                if (!string.IsNullOrEmpty(jsonWeaponKits))
                {
                    this._weaponKitEntityList = JsonSerializer.Deserialize<List<WeaponKitEntity>>(jsonWeaponKits) ?? new List<WeaponKitEntity>();
                }
            }

            if(!this._weaponKitEntityList.Any())
            {
                this._weaponKitEntityList.Add(new WeaponKitEntity()
                {
                    KitName = "DefaultCT",
                    PrimaryWeapon = "weapon_m4a1",
                    SecondaryWeapon = "weapon_hkp2000",
                    Team = CsTeam.CounterTerrorist,
                });

                this._weaponKitEntityList.Add(new WeaponKitEntity()
                {
                    KitName = "DefaultT",
                    PrimaryWeapon = "weapon_ak47",
                    SecondaryWeapon = "weapon_glock",
                    Team = CsTeam.Terrorist,
                });
            }

            if(!pathExists) 
            {
                this.SaveWeaponKits();
            }
        }

        private void SaveWeaponKits() 
        {
            File.WriteAllText(this.GetPath(), JsonSerializer.Serialize(this._weaponKitEntityList));
        }

        private string GetPath()
        {
            var path = Path.Join(this._moduleDirectory, $"configs");

            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Join(path, $"weaponKits.json");

            return path;
        }
    }
}