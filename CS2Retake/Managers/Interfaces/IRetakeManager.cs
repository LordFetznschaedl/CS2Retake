using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface IRetakeManager
    {
        public void PlaySpotAnnouncer();
        public void AssignRandomPlayerInBombZoneAsPlanter();

        public void ConfigureForRetake(bool complete = true);
    }
}
