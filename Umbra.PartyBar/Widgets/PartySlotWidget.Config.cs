using System.Collections.Generic;
using Umbra.Widgets;

namespace Umbra.PartyBar.Widgets;

public sealed partial class PartySlotWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return
        [
            new SelectWidgetConfigVariable(
                "Who",
                "Who",
                "Which person this copy of the bar follows. Add another Party Slot widget for the next person.",
                "p1",
                WhoOptions()
            ),
            new BooleanWidgetConfigVariable("ShowName", "Show name", null, true),
            new BooleanWidgetConfigVariable("ShowJobIcon", "Show job icon", null, true),
            new BooleanWidgetConfigVariable("ShowHp", "HP bar", null, true),
            new BooleanWidgetConfigVariable("ShowMp", "MP bar", "Hidden when that person has no MP.", true),
            new BooleanWidgetConfigVariable(
                "ShowShield",
                "Shield on the HP bar",
                "Only while that character is loaded nearby.",
                true
            ),
            new SelectWidgetConfigVariable(
                "Numbers",
                "Numbers",
                null,
                "Percent",
                new()
                {
                    { "None", "None" },
                    { "Percent", "Percent" },
                    { "Current", "Current" },
                    { "CurrentMax", "Current / Max" },
                }
            ),
            new BooleanWidgetConfigVariable(
                "HideWhenEmpty",
                "Hide when the slot is empty",
                "The stack closes up. Turn this off while you are placing bars.",
                true
            ),
            new SelectWidgetConfigVariable(
                "ColorMode",
                "Color",
                null,
                "Role",
                new()
                {
                    { "Role", "Role" },
                    { "Flat", "One color" },
                }
            ),
            new SelectWidgetConfigVariable(
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
            ),
            new IntegerWidgetConfigVariable(
                "Width",
                "Width",
                "Used on the main toolbar. A vertical aux bar uses the bar width instead.",
                220,
                80,
                800
            ),
        ];
    }

    private static Dictionary<string, string> WhoOptions()
    {
        var options = new Dictionary<string, string> { ["me"] = "Me" };

        for (var i = 1; i <= 8; i++)
            options[$"p{i}"] = $"Party {i}";

        foreach (var group in new[] { 'a', 'b', 'c' })
        {
            for (var i = 1; i <= 8; i++)
                options[$"{group}{i}"] = $"Alliance {char.ToUpperInvariant(group)}{i}";
        }

        return options;
    }
}
