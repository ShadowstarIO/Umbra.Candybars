using System.Collections.Generic;
using Umbra.Widgets;

namespace Umbra.PartyBar.Widgets;

[ToolbarWidget(
    "PartySlot",
    "Candybar",
    "One bar for one person. Add a copy for each slot, then stack them on a vertical aux bar."
)]
public sealed class PartySlotWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candybar";
    protected override bool ShowName => GetConfigValue<bool>("ShowName");
    protected override bool ShowJob => GetConfigValue<bool>("ShowJobIcon");
    protected override bool ShowHp => GetConfigValue<bool>("ShowHp");
    protected override bool ShowMp => GetConfigValue<bool>("ShowMp");
    protected override bool ShowShield => GetConfigValue<bool>("ShowShield");
    protected override bool ShowCast => GetConfigValue<bool>("ShowCast");
    protected override bool ShowBuffs => GetConfigValue<bool>("ShowBuffs");
    protected override bool ShowDebuffs => GetConfigValue<bool>("ShowDebuffs");
    protected override bool ShowNumbers => true;
    protected override bool HighlightCleansable => GetConfigValue<bool>("CleansableHighlight");
    protected override bool ShowDeathMark => GetConfigValue<bool>("DeathMark");
    protected override int BarHeight => GetConfigValue<int>("BarHeight");
    protected override int MpBarHeight => GetConfigValue<int>("MpBarHeight");
    protected override int ShieldBarHeight => GetConfigValue<int>("ShieldBarHeight");
    protected override int CastBarHeight => GetConfigValue<int>("CastBarHeight");
    protected override int TextSize => GetConfigValue<int>("TextSize");
    protected override int NumberSize => GetConfigValue<int>("NumberSize");
    protected override int CastTextSize => GetConfigValue<int>("CastTextSize");
    protected override int IconSize => GetConfigValue<int>("IconSize");
    protected override int BuffIconSize => GetConfigValue<int>("BuffIconSize");
    protected override int DebuffIconSize => GetConfigValue<int>("DebuffIconSize");
    protected override int BuffCount => GetConfigValue<int>("BuffCount");
    protected override int DebuffCount => GetConfigValue<int>("DebuffCount");
    protected override bool UseLayers => true;
    protected override bool PreviewShowsAll => true;
    protected override int JobX => GetConfigValue<int>("JobX");
    protected override int JobY => GetConfigValue<int>("JobY");
    protected override int JobZ => GetConfigValue<int>("JobZ");
    protected override int NameX => GetConfigValue<int>("NameX");
    protected override int NameY => GetConfigValue<int>("NameY");
    protected override int NameW => GetConfigValue<int>("NameW");
    protected override int NameZ => GetConfigValue<int>("NameZ");
    protected override int NumX => GetConfigValue<int>("NumX");
    protected override int NumY => GetConfigValue<int>("NumY");
    protected override int NumW => GetConfigValue<int>("NumW");
    protected override int NumZ => GetConfigValue<int>("NumZ");
    protected override int HpX => GetConfigValue<int>("HpX");
    protected override int HpY => GetConfigValue<int>("HpY");
    protected override int HpW => GetConfigValue<int>("HpW");
    protected override int HpZ => GetConfigValue<int>("HpZ");
    protected override int MpX => GetConfigValue<int>("MpX");
    protected override int MpY => GetConfigValue<int>("MpY");
    protected override int MpW => GetConfigValue<int>("MpW");
    protected override int MpZ => GetConfigValue<int>("MpZ");
    protected override int ShieldX => GetConfigValue<int>("ShieldX");
    protected override int ShieldY => GetConfigValue<int>("ShieldY");
    protected override int ShieldW => GetConfigValue<int>("ShieldW");
    protected override int ShieldZ => GetConfigValue<int>("ShieldZ");
    protected override int CastTextX => GetConfigValue<int>("CastTextX");
    protected override int CastTextY => GetConfigValue<int>("CastTextY");
    protected override int CastTextW => GetConfigValue<int>("CastTextW");
    protected override int CastTextZ => GetConfigValue<int>("CastTextZ");
    protected override int CastX => GetConfigValue<int>("CastX");
    protected override int CastY => GetConfigValue<int>("CastY");
    protected override int CastW => GetConfigValue<int>("CastW");
    protected override int CastZ => GetConfigValue<int>("CastZ");
    protected override int BuffX => GetConfigValue<int>("BuffX");
    protected override int BuffY => GetConfigValue<int>("BuffY");
    protected override int BuffZ => GetConfigValue<int>("BuffZ");
    protected override int DebuffX => GetConfigValue<int>("DebuffX");
    protected override int DebuffY => GetConfigValue<int>("DebuffY");
    protected override int DebuffZ => GetConfigValue<int>("DebuffZ");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return Flag("ShowName", "Show name", null, true, "Name");
        yield return Flag("ShowJobIcon", "Show job icon", null, true, "Job");
        yield return Flag("ShowHp", "Show HP bar", null, true, "HP");
        yield return Flag("ShowMp", "Show MP bar", "Hidden when that person has no MP, unless Preview layout is on.", true, "MP");
        yield return Flag("ShowShield", "Shield on the HP bar", "Only while that character is loaded nearby.", true, "Shield");
        yield return Flag("ShowCast", "Show cast", "Only while that character is loaded nearby, unless Preview layout is on.", true, "Cast");
        yield return Flag("ShowBuffs", "Show buffs", null, false, "Buffs");
        yield return Flag("ShowDebuffs", "Show debuffs", null, false, "Debuffs");
        yield return Flag("CleansableHighlight", "Cleansable highlight", "Green border when a debuff can be cleansed.", true, "Debuffs");
        yield return Flag("DeathMark", "Death marker", "Greys the bar and stamps DEAD.", true, "Bar");

        foreach (var variable in NumberOptions())
            yield return variable;

        yield return new SelectWidgetConfigVariable("ColorMode", "Color", null, "Role", new() { { "Role", "Role" }, { "Flat", "One color" } }) { Category = "HP" };
        yield return new SelectWidgetConfigVariable("FlatColor", "Flat color", "Used when Color is One color.", "Blue", new() { { "Blue", "Blue" }, { "Green", "Green" }, { "Red", "Red" }, { "Yellow", "Yellow" }, { "White", "White" } }) { Category = "HP", Group = "Color" };
        yield return Size("BarHeight", "Height", null, 10, 4, 48, "HP");
        yield return Size("MpBarHeight", "Height", null, 6, 3, 48, "MP");
        yield return Size("ShieldBarHeight", "Height", "Used when the shield is its own bar.", 4, 3, 48, "Shield");
        yield return Size("CastBarHeight", "Bar height", null, 10, 4, 48, "Cast");
        yield return Size("TextSize", "Text size", null, 13, 10, 28, "Name");
        yield return Size("NumberSize", "Text size", null, 12, 8, 28, "Numbers");
        yield return Size("CastTextSize", "Text size", null, 12, 8, 28, "Cast");
        yield return Size("IconSize", "Icon size", null, 20, 12, 64, "Job");
        yield return Size("BuffIconSize", "Icon size", null, 18, 10, 64, "Buffs");
        yield return Size("DebuffIconSize", "Icon size", null, 22, 10, 64, "Debuffs");
        yield return Size("BuffCount", "Count", null, 8, 0, 16, "Buffs");
        yield return Size("DebuffCount", "Count", null, 8, 0, 16, "Debuffs");
        yield return new SelectWidgetConfigVariable("Layout", "Layout", "Pick a starting arrangement. It is applied once, then this returns to Custom so your own X and Y edits stay.", "Custom", new() { { "Custom", "Custom" }, { "Compact", "Compact, fits the toolbar" }, { "Stacked", "Stacked, reserves height" } }) { Category = "Bar" };
        var stamp = new StringWidgetConfigVariable("LayoutStamp", "Layout stamp", null, "");
        stamp.IsHidden = true;
        yield return stamp;
        yield return Spot("JobX", "X", 2, "Job", "Place");
        yield return Spot("JobY", "Y", 5, "Job", "Place");
        yield return Spot("JobZ", "Layer", 10, "Job", "Place", 0, 100);
        yield return Spot("NameX", "X", 22, "Name", "Place");
        yield return Spot("NameY", "Y", 1, "Name", "Place");
        yield return Spot("NameW", "Width", 90, "Name", "Size", 0, 800);
        yield return Spot("NameZ", "Layer", 12, "Name", "Place", 0, 100);
        yield return Spot("NumX", "X", 112, "Numbers", "Place");
        yield return Spot("NumY", "Y", 1, "Numbers", "Place");
        yield return Spot("NumW", "Width", 64, "Numbers", "Size", 0, 800);
        yield return Spot("NumZ", "Layer", 13, "Numbers", "Place", 0, 100);
        yield return Spot("HpX", "X", 22, "HP", "Place");
        yield return Spot("HpY", "Y", 15, "HP", "Place");
        yield return Spot("HpW", "Width", 120, "HP", "Size", 0, 800);
        yield return Spot("HpZ", "Layer", 20, "HP", "Place", 0, 100);
        yield return Spot("MpX", "X", 22, "MP", "Place");
        yield return Spot("MpY", "Y", 24, "MP", "Place");
        yield return Spot("MpW", "Width", 120, "MP", "Size", 0, 800);
        yield return Spot("MpZ", "Layer", 21, "MP", "Place", 0, 100);
        yield return Spot("ShieldX", "X", 22, "Shield", "Place");
        yield return Spot("ShieldY", "Y", 24, "Shield", "Place");
        yield return Spot("ShieldW", "Width", 120, "Shield", "Size", 0, 800);
        yield return Spot("ShieldZ", "Layer", 22, "Shield", "Place", 0, 100);
        yield return Spot("CastTextX", "Text X", 22, "Cast", "Place");
        yield return Spot("CastTextY", "Text Y", -14, "Cast", "Place");
        yield return Spot("CastTextW", "Text width", 120, "Cast", "Size", 0, 800);
        yield return Spot("CastTextZ", "Text layer", 40, "Cast", "Place", 0, 100);
        yield return Spot("CastX", "Bar X", 22, "Cast", "Place");
        yield return Spot("CastY", "Bar Y", 15, "Cast", "Place");
        yield return Spot("CastW", "Bar width", 120, "Cast", "Size", 0, 800);
        yield return Spot("CastZ", "Bar layer", 35, "Cast", "Place", 0, 100);
        yield return Spot("BuffX", "X", 2, "Buffs", "Place");
        yield return Spot("BuffY", "Y", 32, "Buffs", "Place");
        yield return Spot("BuffZ", "Layer", 50, "Buffs", "Place", 0, 100);
        yield return Spot("DebuffX", "X", 2, "Debuffs", "Place");
        yield return Spot("DebuffY", "Y", 48, "Debuffs", "Place");
        yield return Spot("DebuffZ", "Layer", 51, "Debuffs", "Place", 0, 100);
    }

    private static BooleanWidgetConfigVariable Flag(string id, string name, string? description, bool value, string category)
    {
        return new BooleanWidgetConfigVariable(id, name, description, value) { Category = category };
    }

    private static IntegerWidgetConfigVariable Size(string id, string name, string? description, int value, int min, int max, string category)
    {
        return new IntegerWidgetConfigVariable(id, name, description ?? "Size of this piece. Does not move it.", value, min, max)
        {
            Category = category,
            Group = "Size",
        };
    }

    protected override void ApplyLayout()
    {
        var choice = GetConfigValue<string>("Layout");
        var first = GetConfigValue<string>("LayoutStamp") != "2";
        if (!first && (string.IsNullOrEmpty(choice) || choice == "Custom"))
            return;

        var stacked = choice == "Stacked";
        SetInt("Height", stacked ? 132 : 0);
        SetInt("BarHeight", stacked ? 10 : 8);
        SetInt("MpBarHeight", stacked ? 6 : 3);
        SetInt("IconSize", stacked ? 20 : 18);
        SetInt("TextSize", stacked ? 13 : 12);
        SetInt("NumberSize", stacked ? 12 : 11);
        SetInt("BuffIconSize", stacked ? 16 : 14);
        SetInt("DebuffIconSize", stacked ? 16 : 14);
        SetInt("JobX", stacked ? 4 : 2);
        SetInt("JobY", stacked ? 4 : 5);
        SetInt("NameX", stacked ? 28 : 22);
        SetInt("NameY", stacked ? 4 : 1);
        SetInt("NameW", stacked ? 140 : 90);
        SetInt("NumX", stacked ? 170 : 112);
        SetInt("NumY", stacked ? 4 : 1);
        SetInt("NumW", stacked ? 80 : 0);
        SetInt("HpX", stacked ? 4 : 22);
        SetInt("HpY", stacked ? 28 : 15);
        SetInt("MpX", stacked ? 4 : 22);
        SetInt("MpY", stacked ? 42 : 24);
        SetInt("ShieldX", stacked ? 4 : 22);
        SetInt("ShieldY", stacked ? 52 : 24);
        SetInt("CastTextX", stacked ? 4 : 22);
        SetInt("CastTextY", stacked ? 64 : -14);
        SetInt("CastX", stacked ? 4 : 22);
        SetInt("CastY", stacked ? 80 : 15);
        SetInt("BuffX", 4);
        SetInt("BuffY", stacked ? 96 : 32);
        SetInt("DebuffX", 4);
        SetInt("DebuffY", stacked ? 116 : 48);
        SetConfigValue("LayoutStamp", "2");
        if (!first)
            SetConfigValue("Layout", "Custom");
    }
}
