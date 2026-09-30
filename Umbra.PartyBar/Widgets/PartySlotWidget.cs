using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Widgets;
using Una.Drawing;

namespace Umbra.PartyBar.Widgets;

[ToolbarWidget(
    "PartySlot",
    "Party Slot",
    "One bar for one person. Add a copy for each slot, then stack them on a vertical aux bar."
)]
public sealed partial class PartySlotWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    private IObjectTable Objects { get; } = Framework.Service<IObjectTable>();

    public override string GetInstanceName()
    {
        return $"Party Slot · {WhoLabel(GetConfigValue<string>("Who"))}";
    }

    protected override void Initialize() { }

    protected override void OnUpdate()
    {
        var who = GetConfigValue<string>("Who");
        var data = PartySlotReader.Read(who);

        if (data == null)
        {
            if (GetConfigValue<bool>("HideWhenEmpty"))
            {
                IsVisible = false;
                Node.Style.Size = new(0, 0);
                return;
            }

            data = PartySlotReader.Empty(WhoLabel(who));
        }

        IsVisible = true;
        Draw(data.Value);
    }

    private void Draw(PartySlotReader.Slot slot)
    {
        var showName = GetConfigValue<bool>("ShowName");
        var showIcon = GetConfigValue<bool>("ShowJobIcon") && slot.Job != 0;
        var showHp = GetConfigValue<bool>("ShowHp");
        var showMp = GetConfigValue<bool>("ShowMp") && slot.MaxMp > 0;
        var showNumbers = GetConfigValue<string>("Numbers") != "None";
        var height = RowHeight(showName || showIcon || showNumbers, showHp, showMp);

        Node.Style.Size = IsMemberOfVerticalBar
            ? new(0, height)
            : new(GetConfigValue<int>("Width"), height);

        HeaderNode.Style.IsVisible = showName || showIcon || showNumbers;
        IconNode.Style.IsVisible = showIcon;
        NameNode.Style.IsVisible = showName;
        NumbersNode.Style.IsVisible = showNumbers;
        HpTrackNode.Style.IsVisible = showHp;
        MpTrackNode.Style.IsVisible = showMp;

        if (showIcon)
            IconNode.Style.IconId = 62100u + slot.Job;

        NameNode.NodeValue = showName ? slot.Name : string.Empty;
        NameNode.Style.Color = slot.Dead ? new(140, 140, 140) : new("Widget.Text");
        NumbersNode.NodeValue = showNumbers ? Numbers(slot) : string.Empty;

        Node.Tooltip = slot.MaxHp > 0
            ? $"{slot.Name}  {slot.Hp} / {slot.MaxHp}"
            : slot.Name;

        if (showHp)
            PaintBar(HpTrackNode, HpFillNode, slot.Dead ? 1f : Fraction(slot.Hp, slot.MaxHp), slot.Dead ? Dead : BarColor(slot.Job));

        ShieldNode.Style.IsVisible = false;
        if (showHp && !slot.Dead && GetConfigValue<bool>("ShowShield"))
        {
            var shield = ShieldOf(slot.EntityId);
            if (shield > 0)
            {
                ShieldNode.Style.IsVisible = true;
                PaintBar(HpTrackNode, ShieldNode, shield / 100f, Shield);
            }
        }

        if (showMp)
            PaintBar(MpTrackNode, MpFillNode, Fraction(slot.Mp, slot.MaxMp), Mp);
    }

    private static void PaintBar(Node track, Node fill, float fraction, Color color)
    {
        var width = track.InnerWidth;
        if (width < 1)
            width = track.ParentNode?.InnerWidth ?? 0;

        var span = (int)System.Math.Clamp(width * System.Math.Clamp(fraction, 0f, 1f), 0, System.Math.Max(0, width));
        fill.Style.Size = new(span, System.Math.Max(1, track.InnerHeight > 1 ? track.InnerHeight : 6));
        fill.Style.BackgroundColor = color;
        fill.Style.IsVisible = span > 0;
    }

    private byte ShieldOf(uint entityId)
    {
        if (entityId is 0 or 0xE0000000)
            return 0;

        return Objects.SearchById(entityId) is ICharacter character
            ? character.ShieldPercentage
            : (byte)0;
    }

    private string Numbers(PartySlotReader.Slot slot)
    {
        if (slot.MaxHp == 0)
            return "—";

        return GetConfigValue<string>("Numbers") switch
        {
            "Percent" => $"{(int)(Fraction(slot.Hp, slot.MaxHp) * 100)}%",
            "Current" => slot.Hp.ToString(),
            "CurrentMax" => $"{slot.Hp} / {slot.MaxHp}",
            _ => string.Empty,
        };
    }

    private Color BarColor(byte job)
    {
        if (GetConfigValue<string>("ColorMode") == "Flat")
        {
            return GetConfigValue<string>("FlatColor") switch
            {
                "Green" => new(46, 168, 96),
                "Red" => new(186, 64, 64),
                "Yellow" => new(214, 176, 46),
                "White" => new(230, 230, 230),
                _ => new(49, 120, 198),
            };
        }

        return Role(job) switch
        {
            "tank" => new(49, 120, 198),
            "healer" => new(46, 168, 96),
            _ => new(186, 64, 64),
        };
    }

    private static int RowHeight(bool header, bool hp, bool mp)
    {
        var height = 8;
        if (header) height += 22;
        if (hp) height += 10;
        if (mp) height += 8;
        return height;
    }

    private static float Fraction(uint current, uint max)
    {
        return max == 0 ? 0 : current / (float)max;
    }

    private static string Role(byte job) => job switch
    {
        1 or 3 or 19 or 21 or 32 or 37 => "tank",
        6 or 24 or 28 or 33 or 40 => "healer",
        _ => "dps",
    };

    internal static string WhoLabel(string who)
    {
        if (who == "me") return "Me";
        if (who.Length >= 2 && who[0] == 'p' && int.TryParse(who[1..], out var party))
            return $"Party {party}";
        if (who.Length >= 2 && who[0] is 'a' or 'b' or 'c' && int.TryParse(who[1..], out var index))
            return $"Alliance {char.ToUpperInvariant(who[0])}{index}";

        return who;
    }

    private static readonly Color Dead = new(80, 80, 80);
    private static readonly Color Shield = new(235, 235, 235);
    private static readonly Color Mp = new(70, 130, 200);
}
