using System.Collections.Generic;
using JetBrains.Annotations;
using SharedModels;

namespace Drachenkatze.PresetMagician.VendorPresetParser.Arturia
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class Arturia_M12Filter : Arturia, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1298220649};

        protected override List<string> GetInstrumentNames()
        {
            return new List<string> {"M12-Filter"};
        }
    }
}