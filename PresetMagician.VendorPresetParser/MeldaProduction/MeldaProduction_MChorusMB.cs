using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MChorusMB : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296917352};

        protected override string PresetFile { get; } =
            "MMultiBandChoruspresets.xml";

        protected override string RootTag { get; } = "MMultiBandChoruspresetspresets";
    }
}