using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Managers;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Manager
{
    public class CacheManager
    {
        private static CacheManager? _instance = null;
        public static CacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CacheManager();
                }
                return _instance;
            }
        }

        private CacheManager() { }

        private Dictionary<ulong, string> _fullBuyPrimaryCacheCT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _fullBuySecondaryCacheCT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, int> _fullBuyAWPChanceCacheCT { get; set; } = new Dictionary<ulong, int>();
        private Dictionary<ulong, string> _midPrimaryCacheCT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _midSecondaryCacheCT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _pistolCacheCT { get; set; } = new Dictionary<ulong, string>();


        public void AddOrUpdateFullBuyPrimaryCacheCT(CCSPlayerController player, string weaponString)
        {
            if (_fullBuyPrimaryCacheCT.ContainsKey(player.SteamID))
            {
                _fullBuyPrimaryCacheCT[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuyPrimaryCacheCT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateFullBuySecondaryCacheCT(CCSPlayerController player, string weaponString)
        {
            if (_fullBuySecondaryCacheCT.ContainsKey(player.SteamID))
            {
                _fullBuySecondaryCacheCT[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuySecondaryCacheCT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateFullBuyAWPChanceCacheCT(CCSPlayerController player, int awpChance)
        {
            if (_fullBuyAWPChanceCacheCT.ContainsKey(player.SteamID))
            {
                _fullBuyAWPChanceCacheCT[player.SteamID] = awpChance;
            }
            else
            {
                _fullBuyAWPChanceCacheCT.Add(player.SteamID, awpChance);
            }
        }

        public void AddOrUpdateMidPrimaryCacheCT(CCSPlayerController player, string weaponString)
        {
            if (_midPrimaryCacheCT.ContainsKey(player.SteamID))
            {
                _midPrimaryCacheCT[player.SteamID] = weaponString;
            }
            else
            {
                _midPrimaryCacheCT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateMidSecondaryCacheCT(CCSPlayerController player, string weaponString)
        {
            if (_midSecondaryCacheCT.ContainsKey(player.SteamID))
            {
                _midSecondaryCacheCT[player.SteamID] = weaponString;
            }
            else
            {
                _midSecondaryCacheCT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdatePistolCacheCT(CCSPlayerController player, string weaponString)
        {
            if (_pistolCacheCT.ContainsKey(player.SteamID))
            {
                _pistolCacheCT[player.SteamID] = weaponString;
            }
            else
            {
                _pistolCacheCT.Add(player.SteamID, weaponString);
            }
        }

        private Dictionary<ulong, string> _fullBuyPrimaryCacheT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _fullBuySecondaryCacheT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, int> _fullBuyAWPChanceCacheT { get; set; } = new Dictionary<ulong, int>();
        private Dictionary<ulong, string> _midPrimaryCacheT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _midSecondaryCacheT { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _pistolCacheT { get; set; } = new Dictionary<ulong, string>();


        public void AddOrUpdateFullBuyPrimaryCacheT(CCSPlayerController player, string weaponString)
        {
            if (_fullBuyPrimaryCacheT.ContainsKey(player.SteamID))
            {
                _fullBuyPrimaryCacheT[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuyPrimaryCacheT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateFullBuySecondaryCacheT(CCSPlayerController player, string weaponString)
        {
            if (_fullBuySecondaryCacheT.ContainsKey(player.SteamID))
            {
                _fullBuySecondaryCacheT[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuySecondaryCacheT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateFullBuyAWPChanceCacheT(CCSPlayerController player, int awpChance)
        {
            if (_fullBuyAWPChanceCacheT.ContainsKey(player.SteamID))
            {
                _fullBuyAWPChanceCacheT[player.SteamID] = awpChance;
            }
            else
            {
                _fullBuyAWPChanceCacheT.Add(player.SteamID, awpChance);
            }
        }

        public void AddOrUpdateMidPrimaryCacheT(CCSPlayerController player, string weaponString)
        {
            if (_midPrimaryCacheT.ContainsKey(player.SteamID))
            {
                _midPrimaryCacheT[player.SteamID] = weaponString;
            }
            else
            {
                _midPrimaryCacheT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateMidSecondaryCacheT(CCSPlayerController player, string weaponString)
        {
            if (_midSecondaryCacheT.ContainsKey(player.SteamID))
            {
                _midSecondaryCacheT[player.SteamID] = weaponString;
            }
            else
            {
                _midSecondaryCacheT.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdatePistolCacheT(CCSPlayerController player, string weaponString)
        {
            if (_pistolCacheT.ContainsKey(player.SteamID))
            {
                _pistolCacheT[player.SteamID] = weaponString;
            }
            else
            {
                _pistolCacheT.Add(player.SteamID, weaponString);
            }
        }


        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetFullBuyWeapons(CCSPlayerController player)
        {

            if (player.Team == CsTeam.CounterTerrorist)
            {
                _ = this._fullBuyPrimaryCacheCT.TryGetValue(player.SteamID, out string? primary);
                _ = this._fullBuySecondaryCacheCT.TryGetValue(player.SteamID, out string? secondary);
                _ = this._fullBuyAWPChanceCacheCT.TryGetValue(player.SteamID, out int awpChance);

                return (primary, secondary, awpChance);
            }
            else
            {
                _ = this._fullBuyPrimaryCacheT.TryGetValue(player.SteamID, out string? primary);
                _ = this._fullBuySecondaryCacheT.TryGetValue(player.SteamID, out string? secondary);
                _ = this._fullBuyAWPChanceCacheT.TryGetValue(player.SteamID, out int awpChance);

                return (primary, secondary, awpChance);
            }
               

            
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetMidWeapons(CCSPlayerController player)
        {
            if (player.Team == CsTeam.CounterTerrorist)
            {
                _ = this._midPrimaryCacheCT.TryGetValue(player.SteamID, out string? primary);
                _ = this._midSecondaryCacheCT.TryGetValue(player.SteamID, out string? secondary);

                return (primary, secondary, null);
            }
            else
            {
                _ = this._midPrimaryCacheT.TryGetValue(player.SteamID, out string? primary);
                _ = this._midSecondaryCacheT.TryGetValue(player.SteamID, out string? secondary);

                return (primary, secondary, null);
            }
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetPistolWeapons(CCSPlayerController player)
        {
            if (player.Team == CsTeam.CounterTerrorist)
            {
                _ = this._pistolCacheCT.TryGetValue(player.SteamID, out string? secondary);

                return (string.Empty, secondary, null);
            }
            else
            {
                _ = this._pistolCacheT.TryGetValue(player.SteamID, out string? secondary);

                return (string.Empty, secondary, null);
            }

            
        }

        public void AddOrUpdateFullBuyPrimaryCache(CCSPlayerController player, string weaponString, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdateFullBuyPrimaryCacheCT(player, weaponString);
            }
            else
            {
                this.AddOrUpdateFullBuyPrimaryCacheT(player, weaponString);
            }
        }

        public void AddOrUpdateFullBuySecondaryCache(CCSPlayerController player, string weaponString, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdateFullBuySecondaryCacheCT(player, weaponString);
            }
            else
            {
                this.AddOrUpdateFullBuySecondaryCacheT(player, weaponString);
            }
        }

        public void AddOrUpdateFullBuyAWPChanceCache(CCSPlayerController player, int awpChance, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdateFullBuyAWPChanceCacheCT(player, awpChance);
            }
            else
            {
                this.AddOrUpdateFullBuyAWPChanceCacheT(player, awpChance);
            }
        }

        public void AddOrUpdateMidPrimaryCache(CCSPlayerController player, string weaponString, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdateMidPrimaryCacheCT(player, weaponString);
            }
            else
            {
                this.AddOrUpdateMidPrimaryCacheT(player, weaponString);
            }
        }

        public void AddOrUpdateMidSecondaryCache(CCSPlayerController player, string weaponString, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdateMidSecondaryCacheCT(player, weaponString);
            }
            else
            {
                this.AddOrUpdateMidSecondaryCacheT(player, weaponString);
            }
        }

        public void AddOrUpdatePistolCache(CCSPlayerController player, string weaponString, CsTeam team)
        {
            if (team == CsTeam.CounterTerrorist)
            {
                this.AddOrUpdatePistolCacheCT(player, weaponString);
            }
            else
            {
                this.AddOrUpdatePistolCacheT(player, weaponString);
            }
        }

        public void RemoveUserFromCache(CCSPlayerController player)
        {
            var userId = player.SteamID;

            this._fullBuyPrimaryCacheCT.Remove(userId);
            this._fullBuySecondaryCacheCT.Remove(userId);
            this._fullBuyAWPChanceCacheCT.Remove(userId);
            this._midPrimaryCacheCT.Remove(userId);
            this._midSecondaryCacheCT.Remove(userId);
            this._pistolCacheCT.Remove(userId);

            this._fullBuyPrimaryCacheT.Remove(userId);
            this._fullBuySecondaryCacheT.Remove(userId);
            this._fullBuyAWPChanceCacheT.Remove(userId);
            this._midPrimaryCacheT.Remove(userId);
            this._midSecondaryCacheT.Remove(userId);
            this._pistolCacheT.Remove(userId);
        }
    }
}
