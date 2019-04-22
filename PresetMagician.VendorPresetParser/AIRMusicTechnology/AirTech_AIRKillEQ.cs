using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;
using PresetMagician.VendorPresetParser.AIRMusicTechnology.Tfx;

namespace PresetMagician.VendorPresetParser.AIRMusicTechnology
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class AirTech_AIRKillEQ : AirTech, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1818839873};
        protected override string Extension { get; } = "tfx";

        public override string Remarks { get; set; } =
            "Audio Previews are non-functional for this plugin";

        protected override string GetParseDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"AIR Music Technology\AIR Kill EQ\Presets");
        }

        protected override Tfx.Tfx GetTfxParser()
        {
            return new TfxAIRKillEQ();
        }
    }
}