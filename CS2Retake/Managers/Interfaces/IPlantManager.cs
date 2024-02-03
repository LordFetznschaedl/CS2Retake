using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface IPlantManager
    {
        public void HandlePlant();

        public void HasBombBeenPlanted();
        public void HasBombBeenPlantedCallback();
    }
}
