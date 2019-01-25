using System.Collections.Generic;
using JetBrains.Annotations;
using SharedModels;

namespace Drachenkatze.PresetMagician.VendorPresetParser.Arturia
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class Arturia_Solina : Arturia, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1399811122};

        protected override List<string> GetInstrumentNames()
        {
            return new List<string> {"Solina"};
        }
    }
}