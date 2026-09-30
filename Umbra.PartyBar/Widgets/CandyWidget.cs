using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Widgets;
using Una.Drawing;

namespace Umbra.PartyBar.Widgets;

public abstract partial class CandyWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    private IObjectTable Objects { get; } = Framework.Service<IObjectTable>();
    private ITargetManager Targets { get; } = Framework.Service<ITargetManager>();
    private ICommandManager Commands { get; } = Framework.Service<ICommandManager>();

    private uint _entityId;
    private string _name = string.Empty;

    protected abstract string Title { get; }

    protected virtual bool ShowName => false;
    protected virtual bool ShowJob => false;
    protected virtual bool ShowHp => false;
    protected virtual bool ShowMp => false;
    protected virtual bool ShowShield => false;
    protected virtual bool ShowShieldBar => false;
    protected virtual bool ShowCast => false;
    protected virtual bool KeepCastRow => false;
    protected virtual bool ShowBuffs => false;
    protected virtual bool ShowDebuffs => false;
    protected virtual bool ShowNumbers => false;
    protected virtual bool HighlightCleansable => true;
    protected virtual bool ShowDeathMark => true;

    protected virtual int BarHeight => 10;
    protected virtual int MpBarHeight => System.Math.Max(4, BarHeight - 4);
    protected virtual int ShieldBarHeight => System.Math.Max(4, BarHeight - 6);
    protected virtual int CastBarHeight => BarHeight;
    protected virtual int TextSize => 13;
    protected virtual int NumberSize => System.Math.Max(10, TextSize - 1);
    protected virtual int CastTextSize => System.Math.Max(10, TextSize - 1);
    protected virtual int IconSize => 20;
    protected virtual int BuffIconSize => IconSize;
    protected virtual int DebuffIconSize => IconSize;
    protected virtual int BuffCount => 8;
    protected virtual int DebuffCount => 8;
    protected virtual int OrderName => 10;
    protected virtual int OrderJob => 0;
    protected virtual int OrderLabel => 1;
    protected virtual int OrderNumbers => 2;
    protected virtual int OrderHp => 20;
    protected virtual int OrderMp => 30;
    protected virtual int OrderShield => 40;
    protected virtual int OrderCast => 50;
    protected virtual int OrderBuffs => 60;
    protected virtual int OrderDebuffs => 70;
    protected virtual int Width => GetConfigValue<int>("Width");

    private int _rowOrder = int.MinValue;
    private int _headerOrder = int.MinValue;

    public override string GetInstanceName()
    {
        return $"{Title} · {HudReader.WhoLabel(GetConfigValue<string>("Who"))}";
    }

    protected override void Initialize()
    {
        Node.OnClick += _ =>
        {
            if (GetConfigValue<bool>("ClickToTarget"))
                SelectSubject();
        };
    }

    protected override void OnUpdate()
    {
        var who = GetConfigValue<string>("Who");
        var subject = HudReader.Read(who);

        if (subject == null)
        {
            _entityId = 0;
            _name = string.Empty;

            if (GetConfigValue<bool>("HideWhenEmpty"))
            {
                IsVisible = false;
                Node.Style.Size = new(0, 0);
                return;
            }

            subject = HudReader.Empty(HudReader.WhoLabel(who));
        }

        IsVisible = true;
        Draw(subject.Value);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return
        [
            new SelectWidgetConfigVariable("Who", "Who", "Which person this bar follows.", "p1", HudReader.WhoOptions()),
            new BooleanWidgetConfigVariable("HideWhenEmpty", "Hide when empty", "The stack closes up. Turn this off while placing bars.", true),
            new BooleanWidgetConfigVariable("Decorate", "Background", "Turn off to hide the widget background and border.", true),
            new BooleanWidgetConfigVariable("ClickToTarget", "Click to target", null, true),
            new IntegerWidgetConfigVariable("Width", "Width", "Used in a vertical aux bar as well as on the main toolbar.", 260, 36, 800),
        ];
    }

    private void Draw(HudReader.Subject subject)
    {
        var slot = subject.Slot;
        _entityId = slot.EntityId;
        _name = slot.Name;

        var width = Width;
        var barHeight = BarHeight;
        var mpHeight = MpBarHeight;
        var shieldHeight = ShieldBarHeight;
        var castHeight = CastBarHeight;
        var textSize = TextSize;
        var numberSize = NumberSize;
        var castTextSize = CastTextSize;
        var iconSize = IconSize;
        var buffSize = BuffIconSize;
        var debuffSize = DebuffIconSize;
        var showName = ShowName;
        var showJob = ShowJob && slot.Job != 0;
        var showHp = ShowHp;
        var showMp = ShowMp && slot.MaxMp > 0;
        var showShield = ShowShield && slot.Shield > 0 && !slot.Dead;
        var showShieldBar = ShowShieldBar;
        var showCast = ShowCast && (subject.Cast != null || KeepCastRow);
        var showBuffs = ShowBuffs;
        var showDebuffs = ShowDebuffs;
        var showNumbers = ShowNumbers && GetConfigValue<string>("Numbers") != "None";
        var header = showName || showJob || showNumbers;

        var height = 8;
        if (header) height += System.Math.Max(textSize + 6, System.Math.Max(iconSize, numberSize)) + 2;
        if (showHp) height += barHeight + 3;
        if (showMp) height += mpHeight + 3;
        if (showShieldBar) height += shieldHeight + 3;
        if (showCast) height += castHeight + castTextSize + 4;
        if (showBuffs) height += buffSize + 3;
        if (showDebuffs) height += debuffSize + 3;

        PlaceRows();
        PlaceHeader();

        Node.Style.AutoSize = (AutoSize.Fit, AutoSize.Fit);
        Node.Style.Size = new(width, height);
        SizerNode.Style.Size = new(width - 8, 0);

        var decorated = GetConfigValue<bool>("Decorate");
        if (decorated) Node.TagsList.Remove("plain");
        else Node.TagsList.Add("plain");

        var cleansable = HighlightCleansable && subject.HasCleansable && !slot.Dead;
        if (slot.Dead && ShowDeathMark)
            Node.Style.StrokeColor = new(170, 48, 48);
        else if (cleansable)
            Node.Style.StrokeColor = new(90, 210, 110);
        else
            Node.Style.StrokeColor = decorated ? new("Widget.Border") : new(0);

        StateNode.Style.IsVisible = slot.Dead && ShowDeathMark || cleansable;
        StateNode.Style.Size = new(width - 8, height - 8);
        StateNode.Style.BackgroundColor = slot.Dead && ShowDeathMark
            ? new(120, 20, 20, (byte)70)
            : new(70, 180, 80, (byte)50);

        DeathNode.Style.IsVisible = slot.Dead && ShowDeathMark;
        DeathNode.Style.FontSize = textSize;
        DeathNode.Style.Size = new(width - 8, height - 8);

        HeaderNode.Style.IsVisible = header;
        HeaderNode.Style.Size = new(width - 8, System.Math.Max(textSize + 4, iconSize));
        IconNode.Style.IsVisible = showJob;
        IconNode.Style.Size = new(iconSize, iconSize);
        if (showJob)
            IconNode.Style.IconId = 62100u + slot.Job;

        NameNode.Style.IsVisible = showName;
        NameNode.Style.FontSize = textSize;
        NameNode.NodeValue = showName ? slot.Name : string.Empty;
        NameNode.Style.Color = slot.Dead ? new(150, 150, 150) : new("Widget.Text");

        NumbersNode.Style.IsVisible = showNumbers;
        NumbersNode.Style.FontSize = numberSize;
        NumbersNode.NodeValue = showNumbers ? Numbers(slot) : string.Empty;

        HpTrackNode.Style.IsVisible = showHp;
        MpTrackNode.Style.IsVisible = showMp;
        ShieldTrackNode.Style.IsVisible = showShieldBar;
        CastWrapNode.Style.IsVisible = showCast;
        BuffRowNode.Style.IsVisible = showBuffs;
        DebuffRowNode.Style.IsVisible = showDebuffs;

        if (showHp)
        {
            HpTrackNode.Style.Size = new(width - 8, barHeight);
            PaintBar(HpTrackNode, HpFillNode, slot.Dead ? 1f : Fraction(slot.Hp, slot.MaxHp), slot.Dead ? DeadColor : BarColor(slot.Job), barHeight);
        }

        ShieldNode.Style.IsVisible = false;
        if (showHp && showShield)
        {
            ShieldNode.Style.IsVisible = true;
            PaintBar(HpTrackNode, ShieldNode, slot.Shield / 100f, ShieldColor, barHeight);
        }

        if (showMp)
        {
            MpTrackNode.Style.Size = new(width - 8, mpHeight);
            PaintBar(MpTrackNode, MpFillNode, Fraction(slot.Mp, slot.MaxMp), MpColor, mpHeight);
        }

        if (showShieldBar)
        {
            ShieldTrackNode.Style.Size = new(width - 8, shieldHeight);
            PaintBar(ShieldTrackNode, ShieldBarFillNode, slot.MaxHp == 0 ? 0 : slot.Shield / 100f, ShieldColor, shieldHeight);
        }

        if (showCast)
        {
            var casting = subject.Cast;
            CastNameNode.Style.FontSize = castTextSize;
            CastNameNode.NodeValue = casting?.Name ?? string.Empty;
            CastTrackNode.Style.Size = new(width - 8, castHeight);
            PaintBar(CastTrackNode, CastFillNode, casting?.Progress ?? 0, CastColor, castHeight);
            CastWrapNode.Style.IsVisible = true;
        }

        if (showBuffs)
            PaintStatuses(BuffRowNode, subject.Statuses, false, buffSize, BuffCount);

        if (showDebuffs)
            PaintStatuses(DebuffRowNode, subject.Statuses, true, debuffSize, DebuffCount);

        Node.Tooltip = slot.MaxHp > 0 ? $"{slot.Name}  {slot.Hp} / {slot.MaxHp}" : slot.Name;
    }

    private void PaintStatuses(Node row, IReadOnlyList<HudReader.StatusIcon> statuses, bool debuff, int iconSize, int maxCount)
    {
        var matched = 0;
        var index = 0;
        maxCount = System.Math.Clamp(maxCount, 0, row.ChildNodes.Count);

        for (var i = 0; i < row.ChildNodes.Count; i++)
        {
            var node = row.ChildNodes[i];
            HudReader.StatusIcon? match = null;
            if (matched < maxCount)
            {
                while (index < statuses.Count)
                {
                    var candidate = statuses[index++];
                    if (candidate.Debuff == debuff)
                    {
                        match = candidate;
                        matched++;
                        break;
                    }
                }
            }

            if (match == null)
            {
                node.Style.IsVisible = false;
                continue;
            }

            var icon = match.Value;
            node.Style.IsVisible = true;
            node.Style.Size = new(iconSize, iconSize);
            node.Style.IconId = icon.IconId;
            node.NodeValue = icon.Stacks > 1 ? icon.Stacks.ToString() : string.Empty;
            node.Style.StrokeColor = debuff && icon.Cleansable ? new(90, 210, 110) : new(0);
            node.Tooltip = icon.Remaining > 0 ? $"{icon.Remaining:0}s" : null;
        }
    }

    private void PlaceRows()
    {
        var sig = System.HashCode.Combine(OrderName, OrderHp, OrderMp, OrderShield, OrderCast, OrderBuffs, OrderDebuffs);
        if (sig == _rowOrder)
            return;

        _rowOrder = sig;
        Place(Node,
            (OrderName, HeaderNode),
            (OrderHp, HpTrackNode),
            (OrderMp, MpTrackNode),
            (OrderShield, ShieldTrackNode),
            (OrderCast, CastWrapNode),
            (OrderBuffs, BuffRowNode),
            (OrderDebuffs, DebuffRowNode),
            (1000, DeathNode));
    }

    private void PlaceHeader()
    {
        var sig = System.HashCode.Combine(OrderJob, OrderLabel, OrderNumbers);
        if (sig == _headerOrder)
            return;

        _headerOrder = sig;
        Place(HeaderNode,
            (OrderJob, IconNode),
            (OrderLabel, NameNode),
            (OrderNumbers, NumbersNode));
    }

    private static void Place(Node parent, params (int Order, Node Node)[] rows)
    {
        var ranked = new (int Order, int Tie, Node Node)[rows.Length];
        for (var i = 0; i < rows.Length; i++)
            ranked[i] = (rows[i].Order, i, rows[i].Node);

        System.Array.Sort(ranked, static (a, b) =>
        {
            var order = a.Order.CompareTo(b.Order);
            return order != 0 ? order : a.Tie.CompareTo(b.Tie);
        });

        foreach (var row in ranked)
            parent.RemoveChild(row.Node);

        foreach (var row in ranked)
            parent.AppendChild(row.Node);
    }

    private static void PaintBar(Node track, Node fill, float fraction, Color color, int height)
    {
        var width = track.Style.Size?.Width ?? track.InnerWidth;
        if (width < 1)
            width = track.ParentNode?.InnerWidth ?? 0;

        var span = (int)System.Math.Clamp(width * System.Math.Clamp(fraction, 0f, 1f), 0, System.Math.Max(0, width));
        fill.Style.Size = new(span, height);
        fill.Style.BackgroundColor = color;
        fill.Style.IsVisible = span > 0;
    }

    private string Numbers(HudReader.Slot slot)
    {
        if (slot.MaxHp == 0)
            return "—";

        var percent = (int)(Fraction(slot.Hp, slot.MaxHp) * 100);
        return GetConfigValue<string>("Numbers") switch
        {
            "Percent" => $"{percent}%",
            "Current" => slot.Hp.ToString(),
            "CurrentMax" => $"{slot.Hp} / {slot.MaxHp}",
            "CurrentMaxPercent" => $"{slot.Hp} / {slot.MaxHp} ({percent}%)",
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

        return job switch
        {
            1 or 3 or 19 or 21 or 32 or 37 => new(49, 120, 198),
            6 or 24 or 28 or 33 or 40 => new(46, 168, 96),
            _ => new(186, 64, 64),
        };
    }

    private void SelectSubject()
    {
        if (_entityId is not (0 or 0xE0000000) && Objects.SearchById(_entityId) is IGameObject actor && actor.IsValid())
        {
            Targets.Target = actor;
            return;
        }

        if (!string.IsNullOrWhiteSpace(_name))
            Commands.ProcessCommand($"/target {_name}");
    }

    private static float Fraction(uint current, uint max)
    {
        return max == 0 ? 0 : current / (float)max;
    }

    protected static IntegerWidgetConfigVariable Layout(string id, string name, int value)
    {
        return new IntegerWidgetConfigVariable(id, name, "Lower numbers come first. Rows stack downward. Job, name, and numbers run left to right.", value, 0, 100);
    }

    protected static IEnumerable<IWidgetConfigVariable> NumberOptions()
    {
        yield return new SelectWidgetConfigVariable(
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
                { "CurrentMaxPercent", "Current / Max / Percent" },
            }
        );
    }

    private static readonly Color DeadColor = new(80, 80, 80);
    private static readonly Color ShieldColor = new(235, 235, 235);
    private static readonly Color MpColor = new(70, 130, 200);
    private static readonly Color CastColor = new(230, 196, 90);
}
