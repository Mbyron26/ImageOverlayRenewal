using System;
using CSLModdingFramework.Core;

namespace ImageOverlayRenewal.UI;

public class OptionsPanelSystem : OptionsPanelSystemBase {
    protected override Type GetOptionsPanelType() => typeof(OptionsPanel);
}