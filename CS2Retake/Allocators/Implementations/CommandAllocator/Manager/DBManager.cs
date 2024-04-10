using CS2Retake.Allocators.Implementations.CommandAllocator.Interfaces;
using CS2Retake.Allocators.Implementations.CommandAllocator.Repository;
using CS2Retake.Allocators.Implementations.CommandAllocator.Utils;
using CS2Retake.Utils;
using CSZoneNet.Plugin.CS2BaseAllocator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Manager
{
    public class DBManager : IRetakeRepository
    {
        private static DBManager? _instance = null;

        public DBType DBType { get; set; } = DBType.Cache;
        public string AllocatorConfigDirectoryPath { get; set; } = string.Empty;

        private IRetakeRepository? _retakeDB = null;

        public static DBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBManager();
                }
                return _instance;
            }
        }

        public DBManager() { }

        public void Init()
        {
            MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Information, $"Init DBManager for Type {this.DBType} - Path: {this.AllocatorConfigDirectoryPath}");

            switch (this.DBType)
            {
                case DBType.Cache:
                    this._retakeDB = null;
                    break;
                case DBType.SQLite:
                    this._retakeDB = new SQLiteRepository(this.AllocatorConfigDirectoryPath);
                    break;
                default:
                    this._retakeDB = null;
                    break;
            }
        }

        public bool InsertOrUpdateFullBuyPrimaryWeaponString(ulong userId, string weaponString, int team)
        {

            if(this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdateFullBuyPrimaryWeaponString(userId, weaponString, team);
        }

        public bool InsertOrUpdateFullBuySecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdateFullBuySecondaryWeaponString(userId, weaponString, team);
        }

        public bool InsertOrUpdateFullBuyAWPChance(ulong userId, int chance, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdateFullBuyAWPChance(userId, chance, team);
        }

        public bool InsertOrUpdateMidPrimaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdateMidPrimaryWeaponString(userId, weaponString, team);
        }

        public bool InsertOrUpdateMidSecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdateMidSecondaryWeaponString(userId, weaponString, team);
        }

        public bool InsertOrUpdatePistolWeaponString(ulong userId, string weaponString, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return false;
            }

            return this._retakeDB.InsertOrUpdatePistolWeaponString(userId, weaponString, team);
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetFullBuyWeapons(ulong userId, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return (null,null,null);
            }

            return this._retakeDB.GetFullBuyWeapons(userId, team);
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetMidWeapons(ulong userId, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return (null, null, null);
            }

            return this._retakeDB.GetMidWeapons(userId, team);
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetPistolWeapons(ulong userId, int team)
        {
            if (this.DBType == DBType.Cache || this._retakeDB == null)
            {
                return (null, null, null);
            }

            return this._retakeDB.GetPistolWeapons(userId, team);
        }
    }
}
