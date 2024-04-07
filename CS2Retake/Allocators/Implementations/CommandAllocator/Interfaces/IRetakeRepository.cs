using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Interfaces
{
    public interface IRetakeRepository
    {
        void Init();

        bool InsertOrUpdateFullBuyPrimaryWeaponString(ulong userId, string weaponString, int team);
        bool InsertOrUpdateFullBuySecondaryWeaponString(ulong userId, string weaponString, int team);
        bool InsertOrUpdateFullBuyAWPChance(ulong userId, int chance, int team);

        bool InsertOrUpdateMidPrimaryWeaponString(ulong userId, string weaponString, int team);
        bool InsertOrUpdateMidSecondaryWeaponString(ulong userId, string weaponString, int team);

        bool InsertOrUpdatePistolWeaponString(ulong userId, string weaponString, int team);


        (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetFullBuyWeapons(ulong userId, int team);

        (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetMidWeapons(ulong userId, int team);

        (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetPistolWeapons(ulong userId, int team);
    }
}
