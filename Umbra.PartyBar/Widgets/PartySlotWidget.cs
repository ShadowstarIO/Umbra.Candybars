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
    protected override int CanvasHeight => GetConfigValue<int>("CanvasHeight");
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
        yield return new IntegerWidgetConfigVariable("CanvasHeight", "Bar height", "The canvas everything is placed on.", 148, 24, 600);
        yield return Spot("JobX", "Job X", 4);
        yield return Spot("JobY", "Job Y", 4);
        yield return Spot("JobZ", "Job layer", 10, 0, 100);
        yield return Spot("NameX", "Name X", 28);
        yield return Spot("NameY", "Name Y", 4);
        yield return Spot("NameW", "Name width", 150, 0, 800);
        yield return Spot("NameZ", "Name layer", 11, 0, 100);
        yield return Spot("NumX", "Numbers X", 182);
        yield return Spot("NumY", "Numbers Y", 4);
        yield return Spot("NumW", "Numbers width", 74, 0, 800);
        yield return Spot("NumZ", "Numbers layer", 12, 0, 100);
        yield return Spot("HpX", "HP X", 4);
        yield return Spot("HpY", "HP Y", 28);
        yield return Spot("HpW", "HP width", 0, 0, 800);
        yield return Spot("HpZ", "HP layer", 20, 0, 100);
        yield return Spot("MpX", "MP X", 4);
        yield return Spot("MpY", "MP Y", 42);
        yield return Spot("MpW", "MP width", 0, 0, 800);
        yield return Spot("MpZ", "MP layer", 21, 0, 100);
        yield return Spot("ShieldX", "Shield X", 4);
        yield return Spot("ShieldY", "Shield Y", 52);
        yield return Spot("ShieldW", "Shield width", 0, 0, 800);
        yield return Spot("ShieldZ", "Shield layer", 22, 0, 100);
        yield return Spot("CastTextX", "Cast text X", 4);
        yield return Spot("CastTextY", "Cast text Y", 60);
        yield return Spot("CastTextW", "Cast text width", 0, 0, 800);
        yield return Spot("CastTextZ", "Cast text layer", 30, 0, 100);
        yield return Spot("CastX", "Cast bar X", 4);
        yield return Spot("CastY", "Cast bar Y", 78);
        yield return Spot("CastW", "Cast bar width", 0, 0, 800);
        yield return Spot("CastZ", "Cast bar layer", 31, 0, 100);
        yield return Spot("BuffX", "Buffs X", 4);
        yield return Spot("BuffY", "Buffs Y", 92);
        yield return Spot("BuffZ", "Buffs layer", 40, 0, 100);
        yield return Spot("DebuffX", "Debuffs X", 4);
        yield return Spot("DebuffY", "Debuffs Y", 114);
        yield return Spot("DebuffZ", "Debuffs layer", 41, 0, 100);
    }
}
