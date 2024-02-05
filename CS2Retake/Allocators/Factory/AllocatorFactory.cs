using CS2Retake.Allocators.Implementations.CommandAllocator;
using CS2Retake.Utils;
using CSZoneNet.Plugin.CS2BaseAllocator.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Factory
{
    public class AllocatorFactory
    {
        public IBaseAllocator GetAllocator(AllocatorEnum allocator)
        {
            IBaseAllocator chosenAllocator;

            switch (allocator) 
            {
                case AllocatorEnum.Command:
                    chosenAllocator = new CommandAllocator();
                    break;
                case AllocatorEnum.Custom:
                    //TODO: Custom Allocator implementation way.
                    chosenAllocator = new CommandAllocator();
                    break;
                default:
                    chosenAllocator = new CommandAllocator();
                    break;
            }

            chosenAllocator.InitializeConfig(chosenAllocator, chosenAllocator.GetType());

            return chosenAllocator;
        }
    }
}
