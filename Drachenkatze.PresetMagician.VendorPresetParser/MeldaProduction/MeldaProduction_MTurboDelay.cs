using System.Collections.Generic;
using JetBrains.Annotations;

namespace Drachenkatze.PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MTurboDelay: MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1297380676};

        public void ScanBanks()
        {
            ScanPresetXMLFile("MTurboDelaypresets.xml", "MTurboDelaypresets");
        }
    }
}