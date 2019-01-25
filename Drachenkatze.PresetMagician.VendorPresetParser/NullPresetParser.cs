﻿using System.Threading.Tasks;
using SharedModels;

namespace Drachenkatze.PresetMagician.VendorPresetParser
{
    public class NullPresetParser : AbstractVendorPresetParser, IVendorPresetParser
    {
        public override bool IsNullParser => true;

        public void ScanBanks()
        {
        }

        public async Task DoScan()
        {
        }

        public virtual int GetNumPresets()
        {
            return 0;
        }

        public override bool CanHandle()
        {
            return true;
        }
    }
}