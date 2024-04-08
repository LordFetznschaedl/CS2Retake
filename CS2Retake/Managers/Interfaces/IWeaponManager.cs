using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface IWeaponManager
    {
        public void OnGunsCommand(CCSPlayerController? player);
    }
}
