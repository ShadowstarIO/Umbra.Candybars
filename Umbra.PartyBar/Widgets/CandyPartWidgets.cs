using System.Collections.Generic;
using Umbra.Widgets;

namespace Umbra.PartyBar.Widgets;

[ToolbarWidget("CandyName", "Candy Name", "A larger name plate for one person, target, or focus.")]
public sealed class CandyNameWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Name";
    protected override bool ShowName => true;
    protected override int TextSize => GetConfigValue<int>("TextSize");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("TextSize", "Text size", null, 18, 10, 36);
    }
}

[ToolbarWidget("CandyJob", "Candy Job", "A larger job icon for one person, target, or focus.")]
public sealed class CandyJobWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Job";
    protected override bool ShowJob => true;
    protected override int IconSize => GetConfigValue<int>("IconSize");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("IconSize", "Icon size", null, 36, 16, 72);
    }
}

[ToolbarWidget("CandyVitals", "Candy Vitals", "Larger HP, MP, and shield bars for one person, target, or focus.")]
public sealed class CandyVitalsWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Vitals";
    protected override bool ShowHp => true;
    protected override bool ShowMp => GetConfigValue<bool>("ShowMp");
    protected override bool ShowShieldBar => true;
    protected override bool ShowNumbers => true;
    protected override bool ShowDeathMark => GetConfigValue<bool>("DeathMark");
    protected override int BarHeight => GetConfigValue<int>("BarHeight");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new BooleanWidgetConfigVariable("ShowMp", "MP bar", null, true);
        yield return new BooleanWidgetConfigVariable("DeathMark", "Death marker", null, true);
        yield return new IntegerWidgetConfigVariable("BarHeight", "Bar height", null, 16, 6, 48);

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
            null,
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
    }
}

[ToolbarWidget("CandyCast", "Candy Cast", "Name and cast bar for one person, target, or focus.")]
public sealed class CandyCastWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Cast";
    protected override bool ShowName => true;
    protected override bool ShowCast => true;
    protected override bool KeepCastRow => true;
    protected override int TextSize => GetConfigValue<int>("TextSize");
    protected override int BarHeight => GetConfigValue<int>("BarHeight");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("TextSize", "Text size", null, 16, 10, 28);
        yield return new IntegerWidgetConfigVariable("BarHeight", "Bar height", null, 14, 6, 40);
    }
}

[ToolbarWidget("CandyBuffs", "Candy Buffs", "A larger buff row for one person, target, or focus.")]
public sealed class CandyBuffsWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Buffs";
    protected override bool ShowBuffs => true;
    protected override int IconSize => GetConfigValue<int>("IconSize");
    protected override int BuffCount => GetConfigValue<int>("BuffCount");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("IconSize", "Icon size", null, 28, 12, 64);
        yield return new IntegerWidgetConfigVariable("BuffCount", "Buff count", null, 8, 0, 16);
    }
}

[ToolbarWidget("CandyStatus", "Candy Status", "Buffs on one row and debuffs on the other. Sizes, counts, and which row sits on top are settings.")]
public sealed class CandyStatusWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Status";
    protected override bool ShowBuffs => true;
    protected override bool ShowDebuffs => true;
    protected override bool HighlightCleansable => true;
    protected override int BuffIconSize => GetConfigValue<int>("BuffIconSize");
    protected override int DebuffIconSize => GetConfigValue<int>("DebuffIconSize");
    protected override int BuffCount => GetConfigValue<int>("BuffCount");
    protected override int DebuffCount => GetConfigValue<int>("DebuffCount");
    protected override int OrderBuffs => GetConfigValue<int>("OrderBuffs");
    protected override int OrderDebuffs => GetConfigValue<int>("OrderDebuffs");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("BuffIconSize", "Buff icon size", null, 18, 10, 64);
        yield return new IntegerWidgetConfigVariable("DebuffIconSize", "Debuff icon size", null, 26, 10, 64);
        yield return new IntegerWidgetConfigVariable("BuffCount", "Buff count", null, 8, 0, 16);
        yield return new IntegerWidgetConfigVariable("DebuffCount", "Debuff count", null, 8, 0, 16);
        yield return Layout("OrderBuffs", "Buff row", 10);
        yield return Layout("OrderDebuffs", "Debuff row", 20);
    }
}

[ToolbarWidget("CandyDebuffs", "Candy Debuffs", "A larger debuff row for one person, target, or focus. Cleansable debuffs get a green border.")]
public sealed class CandyDebuffsWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : CandyWidget(info, guid, configValues)
{
    protected override string Title => "Candy Debuffs";
    protected override bool ShowDebuffs => true;
    protected override bool HighlightCleansable => true;
    protected override int IconSize => GetConfigValue<int>("IconSize");
    protected override int DebuffIconSize => IconSize;
    protected override int DebuffCount => GetConfigValue<int>("DebuffCount");

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        foreach (var variable in base.GetConfigVariables())
            yield return variable;

        yield return new IntegerWidgetConfigVariable("IconSize", "Icon size", null, 28, 12, 64);
        yield return new IntegerWidgetConfigVariable("DebuffCount", "Debuff count", null, 8, 0, 16);
    }
}
