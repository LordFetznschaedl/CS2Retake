using CounterStrikeSharp.API.Core;
using CS2Retake.Managers;
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

        private Dictionary<ulong, string> _fullBuyPrimaryCache { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _fullBuySecondaryCache { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _midPrimaryCache { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _midSecondaryCache { get; set; } = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _pistolCache { get; set; } = new Dictionary<ulong, string>();


        public void AddOrUpdateFullBuyPrimaryCache (CCSPlayerController player, string weaponString)
        {
            if (_fullBuyPrimaryCache.ContainsKey(player.SteamID))
            {
                _fullBuyPrimaryCache[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuyPrimaryCache.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateFullBuySecondaryCache(CCSPlayerController player, string weaponString)
        {
            if (_fullBuySecondaryCache.ContainsKey(player.SteamID))
            {
                _fullBuySecondaryCache[player.SteamID] = weaponString;
            }
            else
            {
                _fullBuySecondaryCache.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateMidPrimaryCache(CCSPlayerController player, string weaponString)
        {
            if (_midPrimaryCache.ContainsKey(player.SteamID))
            {
                _midPrimaryCache[player.SteamID] = weaponString;
            }
            else
            {
                _midPrimaryCache.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdateMidSecondaryCache(CCSPlayerController player, string weaponString)
        {
            if (_midSecondaryCache.ContainsKey(player.SteamID))
            {
                _midSecondaryCache[player.SteamID] = weaponString;
            }
            else
            {
                _midSecondaryCache.Add(player.SteamID, weaponString);
            }
        }

        public void AddOrUpdatePistolCache(CCSPlayerController player, string weaponString)
        {
            if (_pistolCache.ContainsKey(player.SteamID))
            {
                _pistolCache[player.SteamID] = weaponString;
            }
            else
            {
                _pistolCache.Add(player.SteamID, weaponString);
            }
        }

        public (string? primaryWeapon, string? secondaryWeapon) GetFullBuyWeapons(CCSPlayerController player)
        {
            _ = this._fullBuyPrimaryCache.TryGetValue(player.SteamID, out string? primary);
            _ = this._fullBuySecondaryCache.TryGetValue(player.SteamID, out string? secondary);

            return (primary, secondary);
        }

        public (string? primaryWeapon, string? secondaryWeapon) GetMidWeapons(CCSPlayerController player)
        {
            _ = this._midPrimaryCache.TryGetValue(player.SteamID, out string? primary);
            _ = this._midSecondaryCache.TryGetValue(player.SteamID, out string? secondary);

            return (primary, secondary);
        }

        public (string? primaryWeapon, string? secondaryWeapon) GetPistolWeapons(CCSPlayerController player)
        {
            _ = this._pistolCache.TryGetValue(player.SteamID, out string? secondary);

            return (string.Empty, secondary);
        }
    }
}
