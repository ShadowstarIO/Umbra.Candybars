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

        yield return new BooleanWidgetConfigVariable("ShowName", "Show name", null, true);
        yield return new BooleanWidgetConfigVariable("ShowJobIcon", "Show job icon", null, true);
        yield return new BooleanWidgetConfigVariable("ShowHp", "HP bar", null, true);
        yield return new BooleanWidgetConfigVariable("ShowMp", "MP bar", "Hidden when that person has no MP.", true);
        yield return new BooleanWidgetConfigVariable("ShowShield", "Shield on the HP bar", "Only while that character is loaded nearby.", true);
        yield return new BooleanWidgetConfigVariable("ShowCast", "Cast bar", "Only while that character is loaded nearby.", true);
        yield return new BooleanWidgetConfigVariable("ShowBuffs", "Buffs", null, false);
        yield return new BooleanWidgetConfigVariable("ShowDebuffs", "Debuffs", null, false);
        yield return new BooleanWidgetConfigVariable("CleansableHighlight", "Cleansable highlight", "Green border when a debuff can be cleansed.", true);
        yield return new BooleanWidgetConfigVariable("DeathMark", "Death marker", "Greys the bar and stamps DEAD.", true);

        foreach (var variable in NumberOptions())
            yield return variable;

        yield return new SelectWidgetConfigVariable(
            "ColorMode",
            "Color",
            null,
            "Role",
            new() { { "Role", "Role" }, { "Flat", "One color" } }
        );
        yield return new SelectWidgetConfigVariable(
            "FlatColor",
            "Flat color",
            "Used when Color is One color.",
            "Blue",
            new()
            {
                { "Blue", "Blue" },
                { "Green", "Green" },
                { "Red", "Red" },
                { "Yellow", "Yellow" },
                { "White", "White" },
            }
        );
        yield return new IntegerWidgetConfigVariable("BarHeight", "HP bar height", null, 10, 4, 48);
        yield return new IntegerWidgetConfigVariable("MpBarHeight", "MP bar height", null, 6, 3, 48);
        yield return new IntegerWidgetConfigVariable("ShieldBarHeight", "Shield bar height", "Used when the shield is its own bar.", 4, 3, 48);
        yield return new IntegerWidgetConfigVariable("CastBarHeight", "Cast bar height", null, 10, 4, 48);
        yield return new IntegerWidgetConfigVariable("TextSize", "Name size", null, 13, 10, 28);
        yield return new IntegerWidgetConfigVariable("NumberSize", "Number size", null, 12, 8, 28);
        yield return new IntegerWidgetConfigVariable("CastTextSize", "Cast text size", null, 12, 8, 28);
        yield return new IntegerWidgetConfigVariable("IconSize", "Job icon size", null, 20, 12, 64);
        yield return new IntegerWidgetConfigVariable("BuffIconSize", "Buff icon size", null, 18, 10, 64);
        yield return new IntegerWidgetConfigVariable("DebuffIconSize", "Debuff icon size", null, 22, 10, 64);
        yield return new IntegerWidgetConfigVariable("BuffCount", "Buff count", null, 8, 0, 16);
        yield return new IntegerWidgetConfigVariable("DebuffCount", "Debuff count", null, 8, 0, 16);
        yield return new SelectWidgetConfigVariable(
            "Layout",
            "Layout",
            "Pick a starting arrangement. It is applied once, then this returns to Custom so your own X and Y edits stay.",
            "Custom",
            new()
            {
                { "Custom", "Custom" },
                { "Compact", "Compact, fits the toolbar" },
                { "Stacked", "Stacked, reserves height" },
            }
        );
        var stamp = new StringWidgetConfigVariable("LayoutStamp", "Layout stamp", null, "");
        stamp.IsHidden = true;
        yield return stamp;
        yield return Spot("JobX", "Job X", 2);
        yield return Spot("JobY", "Job Y", 5);
        yield return Spot("JobZ", "Job layer", 10, 0, 100);
        yield return Spot("NameX", "Name X", 22);
        yield return Spot("NameY", "Name Y", 1);
        yield return Spot("NameW", "Name width", 90, 0, 800);
        yield return Spot("NameZ", "Name layer", 12, 0, 100);
        yield return Spot("NumX", "Numbers X", 112);
        yield return Spot("NumY", "Numbers Y", 1);
        yield return Spot("NumW", "Numbers width", 0, 0, 800);
        yield return Spot("NumZ", "Numbers layer", 13, 0, 100);
        yield return Spot("HpX", "HP X", 22);
        yield return Spot("HpY", "HP Y", 15);
        yield return Spot("HpW", "HP width", 0, 0, 800);
        yield return Spot("HpZ", "HP layer", 20, 0, 100);
        yield return Spot("MpX", "MP X", 22);
        yield return Spot("MpY", "MP Y", 24);
        yield return Spot("MpW", "MP width", 0, 0, 800);
        yield return Spot("MpZ", "MP layer", 21, 0, 100);
        yield return Spot("ShieldX", "Shield X", 22);
        yield return Spot("ShieldY", "Shield Y", 24);
        yield return Spot("ShieldW", "Shield width", 0, 0, 800);
        yield return Spot("ShieldZ", "Shield layer", 22, 0, 100);
        yield return Spot("CastTextX", "Cast text X", 22);
        yield return Spot("CastTextY", "Cast text Y", -14);
        yield return Spot("CastTextW", "Cast text width", 0, 0, 800);
        yield return Spot("CastTextZ", "Cast text layer", 40, 0, 100);
        yield return Spot("CastX", "Cast bar X", 22);
        yield return Spot("CastY", "Cast bar Y", 15);
        yield return Spot("CastW", "Cast bar width", 0, 0, 800);
        yield return Spot("CastZ", "Cast bar layer", 35, 0, 100);
        yield return Spot("BuffX", "Buffs X", 2);
        yield return Spot("BuffY", "Buffs Y", 32);
        yield return Spot("BuffZ", "Buffs layer", 50, 0, 100);
        yield return Spot("DebuffX", "Debuffs X", 2);
        yield return Spot("DebuffY", "Debuffs Y", 48);
        yield return Spot("DebuffZ", "Debuffs layer", 51, 0, 100);
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
