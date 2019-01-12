using System.Collections.Generic;
using JetBrains.Annotations;

namespace Drachenkatze.PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MTurboReverb: MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1299468901};

        public void ScanBanks()
        {
            ScanPresetXMLFile("MTurboReverbpresets.xml", "MTurboReverbpresets");
        }
    }
}