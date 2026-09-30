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
    protected override int TextSize => GetConfigValue<int>("TextSize");
    protected override int IconSize => GetConfigValue<int>("IconSize");

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
        yield return new IntegerWidgetConfigVariable("BarHeight", "Bar height", null, 10, 4, 48);
        yield return new IntegerWidgetConfigVariable("TextSize", "Text size", null, 13, 10, 28);
        yield return new IntegerWidgetConfigVariable("IconSize", "Icon size", null, 20, 12, 64);
    }
}
