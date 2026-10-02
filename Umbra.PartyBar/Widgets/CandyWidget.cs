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
    protected virtual bool UseLayers => false;
    protected virtual int CanvasHeight => 64;
    protected virtual int JobX => 4;
    protected virtual int JobY => 4;
    protected virtual int JobZ => 10;
    protected virtual int NameX => 28;
    protected virtual int NameY => 4;
    protected virtual int NameW => 150;
    protected virtual int NameZ => 11;
    protected virtual int NumX => 182;
    protected virtual int NumY => 4;
    protected virtual int NumW => 74;
    protected virtual int NumZ => 12;
    protected virtual int HpX => 4;
    protected virtual int HpY => 28;
    protected virtual int HpW => 0;
    protected virtual int HpZ => 20;
    protected virtual int MpX => 4;
    protected virtual int MpY => 42;
    protected virtual int MpW => 0;
    protected virtual int MpZ => 21;
    protected virtual int ShieldX => 4;
    protected virtual int ShieldY => 52;
    protected virtual int ShieldW => 0;
    protected virtual int ShieldZ => 22;
    protected virtual int CastTextX => 4;
    protected virtual int CastTextY => 60;
    protected virtual int CastTextW => 0;
    protected virtual int CastTextZ => 30;
    protected virtual int CastX => 4;
    protected virtual int CastY => 78;
    protected virtual int CastW => 0;
    protected virtual int CastZ => 31;
    protected virtual int BuffX => 4;
    protected virtual int BuffY => 92;
    protected virtual int BuffZ => 40;
    protected virtual int DebuffX => 4;
    protected virtual int DebuffY => 114;
    protected virtual int DebuffZ => 41;
    protected virtual int Width => GetConfigValue<int>("Width");

    private int _layerSig = int.MinValue;

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
        var layered = UseLayers;
        var canvasHeight = CanvasHeight;

        var height = 8;
        if (!layered)
        {
            if (header) height += System.Math.Max(textSize + 6, System.Math.Max(iconSize, numberSize)) + 2;
            if (showHp) height += barHeight + 3;
            if (showMp) height += mpHeight + 3;
            if (showShieldBar) height += shieldHeight + 3;
            if (showCast) height += castHeight + castTextSize + 4;
            if (showBuffs) height += buffSize + 3;
            if (showDebuffs) height += debuffSize + 3;
        }
        else
        {
            height = canvasHeight;
        }

        Node.Style.AutoSize = (AutoSize.Fit, AutoSize.Fit);
        Node.Style.Size = new(width, height);
        if (layered)
        {
            Node.Style.Padding = new(0);
            SizerNode.Style.Anchor = Anchor.TopLeft;
            SizerNode.Style.Margin = new EdgeSize(0);
            SizerNode.Style.Size = new(width, height);
        }
        else
        {
            SizerNode.Style.Size = new(width - 8, 0);
        }

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

        var boxWidth = layered ? width : width - 8;
        var boxHeight = layered ? height : height - 8;
        StateNode.Style.IsVisible = slot.Dead && ShowDeathMark || cleansable;
        StateNode.Style.Size = new(boxWidth, boxHeight);
        StateNode.Style.BackgroundColor = slot.Dead && ShowDeathMark
            ? new(120, 20, 20, (byte)70)
            : new(70, 180, 80, (byte)50);

        DeathNode.Style.IsVisible = slot.Dead && ShowDeathMark;
        DeathNode.Style.FontSize = textSize;
        DeathNode.Style.Size = new(boxWidth, boxHeight);

        IconNode.Style.IsVisible = showJob;
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
        CastNameNode.Style.IsVisible = showCast;
        CastTrackNode.Style.IsVisible = showCast;
        BuffRowNode.Style.IsVisible = showBuffs;
        DebuffRowNode.Style.IsVisible = showDebuffs;

        var hpWidth = layered ? Span(HpW, HpX, width) : width - 8;
        var mpWidth = layered ? Span(MpW, MpX, width) : width - 8;
        var shieldWidth = layered ? Span(ShieldW, ShieldX, width) : width - 8;
        var castWidth = layered ? Span(CastW, CastX, width) : width - 8;

        if (layered)
        {
            PlaceLayer(StateNode, 0, 0, width, height, 1);
            PlaceLayer(IconNode, JobX, JobY, iconSize, iconSize, JobZ);
            PlaceLayer(NameNode, NameX, NameY, Span(NameW, NameX, width), System.Math.Max(textSize + 4, iconSize), NameZ);
            PlaceLayer(NumbersNode, NumX, NumY, Span(NumW, NumX, width), System.Math.Max(numberSize + 4, 14), NumZ);
            PlaceLayer(HpTrackNode, HpX, HpY, hpWidth, barHeight, HpZ);
            PlaceLayer(MpTrackNode, MpX, MpY, mpWidth, mpHeight, MpZ);
            PlaceLayer(ShieldTrackNode, ShieldX, ShieldY, shieldWidth, shieldHeight, ShieldZ);
            PlaceLayer(CastNameNode, CastTextX, CastTextY, Span(CastTextW, CastTextX, width), castTextSize + 4, CastTextZ);
            PlaceLayer(CastTrackNode, CastX, CastY, castWidth, castHeight, CastZ);
            PlaceLayer(BuffRowNode, BuffX, BuffY, System.Math.Max(buffSize, BuffCount * (buffSize + 1)), buffSize, BuffZ);
            PlaceLayer(DebuffRowNode, DebuffX, DebuffY, System.Math.Max(debuffSize, DebuffCount * (debuffSize + 1)), debuffSize, DebuffZ);
            PlaceLayer(DeathNode, 0, 0, width, height, 200);
        }

        if (showHp)
        {
            if (!layered)
                HpTrackNode.Style.Size = new(hpWidth, barHeight);
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
            if (!layered)
                MpTrackNode.Style.Size = new(mpWidth, mpHeight);
            PaintBar(MpTrackNode, MpFillNode, Fraction(slot.Mp, slot.MaxMp), MpColor, mpHeight);
        }

        if (showShieldBar)
        {
            if (!layered)
                ShieldTrackNode.Style.Size = new(shieldWidth, shieldHeight);
            PaintBar(ShieldTrackNode, ShieldBarFillNode, slot.MaxHp == 0 ? 0 : slot.Shield / 100f, ShieldColor, shieldHeight);
        }

        if (showCast)
        {
            var casting = subject.Cast;
            CastNameNode.Style.FontSize = castTextSize;
            CastNameNode.NodeValue = casting?.Name ?? string.Empty;
            if (!layered)
                CastTrackNode.Style.Size = new(castWidth, castHeight);
            PaintBar(CastTrackNode, CastFillNode, casting?.Progress ?? 0, CastColor, castHeight);
        }

        if (showBuffs)
            PaintStatuses(BuffRowNode, subject.Statuses, false, buffSize, BuffCount);

        if (showDebuffs)
            PaintStatuses(DebuffRowNode, subject.Statuses, true, debuffSize, DebuffCount);

        if (layered)
        {
            StackLayers(
                (1, StateNode),
                (JobZ, IconNode),
                (NameZ, NameNode),
                (NumZ, NumbersNode),
                (HpZ, HpTrackNode),
                (MpZ, MpTrackNode),
                (ShieldZ, ShieldTrackNode),
                (CastTextZ, CastNameNode),
                (CastZ, CastTrackNode),
                (BuffZ, BuffRowNode),
                (DebuffZ, DebuffRowNode),
                (200, DeathNode));
        }

        Node.Tooltip = slot.MaxHp > 0 ? $"{slot.Name}  {slot.Hp} / {slot.MaxHp}" : slot.Name;
    }

    private static int Span(int configured, int x, int canvas)
    {
        return configured > 0 ? configured : System.Math.Max(1, canvas - System.Math.Max(0, x));
    }

    private static void PlaceLayer(Node node, int x, int y, int width, int height, int z)
    {
        node.Style.Anchor = Anchor.None;
        node.Style.Margin = new EdgeSize(y, 0, 0, x);
        node.Style.Size = new(System.Math.Max(1, width), System.Math.Max(1, height));
        node.SortIndex = z;
    }

    private void StackLayers(params (int Z, Node Node)[] layers)
    {
        var sig = 17;
        foreach (var layer in layers)
            sig = System.HashCode.Combine(sig, layer.Z);

        if (sig == _layerSig)
            return;

        _layerSig = sig;
        var ranked = new (int Z, int Tie, Node Node)[layers.Length];
        for (var i = 0; i < layers.Length; i++)
            ranked[i] = (layers[i].Z, i, layers[i].Node);

        System.Array.Sort(ranked, static (a, b) =>
        {
            var order = a.Z.CompareTo(b.Z);
            return order != 0 ? order : a.Tie.CompareTo(b.Tie);
        });

        foreach (var layer in ranked)
            Node.RemoveChild(layer.Node);

        foreach (var layer in ranked)
            Node.AppendChild(layer.Node);
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

    protected static IntegerWidgetConfigVariable Spot(string id, string name, int value, int min = -200, int max = 800)
    {
        return new IntegerWidgetConfigVariable(id, name, "From the top-left of this bar. Width 0 fills the rest. Higher layer numbers draw on top.", value, min, max);
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
