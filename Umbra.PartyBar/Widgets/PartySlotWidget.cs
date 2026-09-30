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
    protected override int OrderName => GetConfigValue<int>("OrderName");
    protected override int OrderJob => GetConfigValue<int>("OrderJob");
    protected override int OrderLabel => GetConfigValue<int>("OrderLabel");
    protected override int OrderNumbers => GetConfigValue<int>("OrderNumbers");
    protected override int OrderHp => GetConfigValue<int>("OrderHp");
    protected override int OrderMp => GetConfigValue<int>("OrderMp");
    protected override int OrderShield => GetConfigValue<int>("OrderShield");
    protected override int OrderCast => GetConfigValue<int>("OrderCast");
    protected override int OrderBuffs => GetConfigValue<int>("OrderBuffs");
    protected override int OrderDebuffs => GetConfigValue<int>("OrderDebuffs");

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
        yield return Layout("OrderName", "Name row", 10);
        yield return Layout("OrderJob", "Job icon", 0);
        yield return Layout("OrderLabel", "Name text", 1);
        yield return Layout("OrderNumbers", "Numbers", 2);
        yield return Layout("OrderHp", "HP bar", 20);
        yield return Layout("OrderMp", "MP bar", 30);
        yield return Layout("OrderShield", "Shield bar", 40);
        yield return Layout("OrderCast", "Cast bar", 50);
        yield return Layout("OrderBuffs", "Buff row", 60);
        yield return Layout("OrderDebuffs", "Debuff row", 70);
    }
}
