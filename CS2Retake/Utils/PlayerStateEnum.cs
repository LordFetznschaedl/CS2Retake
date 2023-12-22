using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Utils
{
    public enum PlayerStateEnum
    {
        None = 0,
        Connecting = 1, 
        Connected = 2,
        Spectating = 3,
        Queue = 4,
        Playing = 5,
    }
}
