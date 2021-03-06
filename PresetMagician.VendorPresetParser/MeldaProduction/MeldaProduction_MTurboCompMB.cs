using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MTurboCompMB : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1297367917};

        protected override string PresetFile { get; } = "MTurboCompMBpresets.xml";

        protected override string RootTag { get; } = "MTurboCompMBpresets";
    }
}