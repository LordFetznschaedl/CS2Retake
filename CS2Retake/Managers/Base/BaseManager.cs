using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Base
{
    public abstract class BaseManager
    {
        public abstract void ResetForNextRound(bool completeReset = true);

        public abstract void ResetForNextMap(bool completeReset = true);
    }
}
