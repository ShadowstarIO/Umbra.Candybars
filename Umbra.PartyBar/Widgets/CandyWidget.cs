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
    protected virtual bool PreviewShowsAll => false;
    protected virtual int CanvasHeight => GetConfigValue<int>("Height");
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
        ApplyLayout();

        var preview = GetConfigValue<bool>("Preview");
        var who = GetConfigValue<string>("Who");
        var subject = HudReader.Read(who);

        if (subject == null)
        {
            _entityId = 0;
            _name = string.Empty;

            if (!preview && GetConfigValue<bool>("HideWhenEmpty"))
            {
                IsVisible = false;
                Node.Style.Size = new(0, 0);
                return;
            }

            subject = preview ? HudReader.Preview(null) : HudReader.Empty(HudReader.WhoLabel(who));
        }
        else if (preview)
        {
            subject = HudReader.Preview(subject);
        }

        IsVisible = true;
        Draw(subject.Value);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return
        [
            new SelectWidgetConfigVariable("Who", "Who", "F1 to F8 follow the party list, including your sort settings. A1, B1, and C1 follow the alliance list.", "p1", HudReader.WhoOptions()) { Category = "Bar" },
            new BooleanWidgetConfigVariable("Preview", "Preview layout", "Draw every piece with sample bars, icons, and text so you can place them without a party.", false) { Category = "Bar" },
            new BooleanWidgetConfigVariable("HideWhenEmpty", "Hide when empty", "The stack closes up. Turn this off while placing bars.", true) { Category = "Bar" },
            new BooleanWidgetConfigVariable("Decorate", "Background", "Turn off to hide the widget background and border.", true) { Category = "Bar" },
            new BooleanWidgetConfigVariable("ClickToTarget", "Click to target", null, true) { Category = "Bar" },
            new IntegerWidgetConfigVariable("Width", "Width", "Space this widget reserves. Pieces can hang past it.", 168, 24, 800) { Category = "Bar", Group = "Size" },
            new IntegerWidgetConfigVariable("Height", "Height", "0 matches the toolbar and does not stretch widgets beside this one.", 0, 0, 600) { Category = "Bar", Group = "Size" },
        ];
    }

    protected virtual void ApplyLayout()
    {
    }

    protected void SetInt(string key, int value) => SetConfigValue(key, value);

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
        var previewAll = GetConfigValue<bool>("Preview") && PreviewShowsAll;
        var showName = ShowName;
        var showJob = ShowJob && (previewAll || slot.Job != 0);
        var showHp = ShowHp;
        var showMp = ShowMp && (previewAll || slot.MaxMp > 0);
        var showShield = ShowShield && !slot.Dead && (previewAll || slot.Shield > 0);
        var showShieldBar = ShowShieldBar;
        var showCast = ShowCast && (previewAll || subject.Cast != null || KeepCastRow);
        var showBuffs = ShowBuffs;
        var showDebuffs = ShowDebuffs;
        var showNumbers = ShowNumbers && GetConfigValue<string>("Numbers") != "None";
        if (previewAll)
        {
            showName = true;
            showJob = true;
            showHp = true;
            showMp = true;
            showShield = true;
            showCast = true;
            showBuffs = true;
            showDebuffs = true;
        }
        var header = showName || showJob || showNumbers;
        var layered = UseLayers;
        var natural = 8;
        if (!layered)
        {
            if (header) natural += System.Math.Max(textSize + 6, System.Math.Max(iconSize, numberSize)) + 2;
            if (showHp) natural += barHeight + 3;
            if (showMp) natural += mpHeight + 3;
            if (showShieldBar) natural += shieldHeight + 3;
            if (showCast) natural += castHeight + castTextSize + 4;
            if (showBuffs) natural += buffSize + 3;
            if (showDebuffs) natural += debuffSize + 3;
        }

        // 0 keeps the widget the height of the toolbar, so pieces can hang off it
        // without stretching the widgets beside it.
        var height = CanvasHeight > 0
            ? CanvasHeight
            : layered ? SafeHeight : System.Math.Min(natural, SafeHeight);

        Node.Overflow = true;
        Node.Style.AutoSize = null;
        Node.Style.Size = new(width, System.Math.Max(1, height));
        Node.Style.Padding = layered ? new(0) : new(2);
        SizerNode.Style.Anchor = Anchor.TopLeft;
        SizerNode.Style.Margin = new EdgeSize(0);
        SizerNode.Style.Size = new(System.Math.Max(1, width - (layered ? 0 : 4)), System.Math.Max(1, height));

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
        NameNode.Style.TextAlign = Anchor.MiddleCenter;
        NameNode.NodeValue = showName ? slot.Name : string.Empty;
        NameNode.Style.Color = slot.Dead ? new(150, 150, 150) : new("Widget.Text");

        NumbersNode.Style.IsVisible = showNumbers;
        NumbersNode.Style.FontSize = numberSize;
        NumbersNode.Style.Color = new(255, 255, 255);
        NumbersNode.Style.OutlineColor = new(0, 0, 0);
        NumbersNode.Style.OutlineSize = 1;
        NumbersNode.Style.TextAlign = Anchor.MiddleCenter;
        NumbersNode.Style.TextOverflow = true;
        NumbersNode.Style.WordWrap = false;
        NumbersNode.NodeValue = showNumbers ? Numbers(slot) : string.Empty;

        IconNode.Style.ImageScaleMode = ImageScaleMode.Adapt;

        HpTrackNode.Style.IsVisible = showHp;
        MpTrackNode.Style.IsVisible = showMp;
        ShieldTrackNode.Style.IsVisible = showShieldBar;
        CastNameNode.Style.IsVisible = showCast;
        CastTrackNode.Style.IsVisible = showCast;
        BuffRowNode.Style.IsVisible = showBuffs;
        DebuffRowNode.Style.IsVisible = showDebuffs;

        var hpWidth = layered ? Span(HpW, 120) : System.Math.Max(1, width - 4);
        var mpWidth = layered ? Span(MpW, 120) : System.Math.Max(1, width - 4);
        var shieldWidth = layered ? Span(ShieldW, 120) : System.Math.Max(1, width - 4);
        var castWidth = layered ? Span(CastW, 120) : System.Math.Max(1, width - 4);
        var nameBox = System.Math.Max(textSize + 4, 12);
        var numberBox = System.Math.Max(numberSize + 4, 12);

        if (layered)
        {
            PlaceLayer(StateNode, 0, 0, width, height, 1);
            PlaceLayer(IconNode, JobX, JobY, iconSize, iconSize, JobZ);
            PlaceLayer(NameNode, NameX, NameY, Span(NameW, 90), nameBox, NameZ);
            PlaceLayer(NumbersNode, NumX, NumY, Span(NumW, 64), numberBox, System.Math.Max(NumZ, HpZ + 1));
            PlaceLayer(HpTrackNode, HpX, HpY, hpWidth, barHeight, HpZ);
            PlaceLayer(MpTrackNode, MpX, MpY, mpWidth, mpHeight, MpZ);
            PlaceLayer(ShieldTrackNode, ShieldX, ShieldY, shieldWidth, shieldHeight, ShieldZ);
            PlaceLayer(CastNameNode, CastTextX, CastTextY, Span(CastTextW, 120), castTextSize + 4, CastTextZ);
            PlaceLayer(CastTrackNode, CastX, CastY, castWidth, castHeight, CastZ);
            PlaceLayer(BuffRowNode, BuffX, BuffY, System.Math.Max(buffSize, BuffCount * (buffSize + 1)), buffSize, BuffZ);
            PlaceLayer(DebuffRowNode, DebuffX, DebuffY, System.Math.Max(debuffSize, DebuffCount * (debuffSize + 1)), debuffSize, DebuffZ);
            PlaceLayer(DeathNode, 0, 0, width, height, 200);
        }
        else
        {
            PlaceFitted(width, height, showJob, showName, showNumbers, showHp, showMp, showShieldBar, showCast, showBuffs, showDebuffs, iconSize, nameBox, numberBox, barHeight, mpHeight, shieldHeight, castTextSize, castHeight, buffSize, debuffSize, hpWidth);
        }

        if (showHp)
            PaintBar(HpTrackNode, HpFillNode, slot.Dead ? 1f : Fraction(slot.Hp, slot.MaxHp), slot.Dead ? DeadColor : BarColor(slot.Job), barHeight);

        ShieldNode.Style.IsVisible = false;
        if (showHp && showShield)
        {
            ShieldNode.Style.IsVisible = true;
            PaintBar(HpTrackNode, ShieldNode, slot.Shield / 100f, ShieldColor, barHeight);
        }

        if (showMp)
            PaintBar(MpTrackNode, MpFillNode, Fraction(slot.Mp, slot.MaxMp), MpColor, mpHeight);

        if (showShieldBar)
            PaintBar(ShieldTrackNode, ShieldBarFillNode, slot.MaxHp == 0 ? 0 : slot.Shield / 100f, ShieldColor, shieldHeight);

        if (showCast)
        {
            var casting = subject.Cast;
            CastNameNode.Style.FontSize = castTextSize;
            CastNameNode.Style.TextAlign = Anchor.MiddleCenter;
            CastNameNode.NodeValue = casting?.Name ?? string.Empty;
            PaintBar(CastTrackNode, CastFillNode, casting?.Progress ?? 0, CastColor, castHeight);
        }

        if (showBuffs)
        {
            PaintStatuses(BuffRowNode, subject.Statuses, false, buffSize, BuffCount);
            CenterRow(BuffRowNode, buffSize);
        }

        if (showDebuffs)
        {
            PaintStatuses(DebuffRowNode, subject.Statuses, true, debuffSize, DebuffCount);
            CenterRow(DebuffRowNode, debuffSize);
        }

        if (layered)
        {
            StackLayers(
                (1, StateNode),
                (JobZ, IconNode),
                (NameZ, NameNode),
                (System.Math.Max(NumZ, HpZ + 1), NumbersNode),
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

    private static int Span(int configured, int fallback)
    {
        return configured > 0 ? configured : fallback;
    }

    private static void PlaceLayer(Node node, int x, int y, int width, int height, int z)
    {
        node.Style.Anchor = Anchor.None;
        node.Style.Margin = new EdgeSize(y, 0, 0, x);
        node.Style.Size = new(System.Math.Max(1, width), System.Math.Max(1, height));
        node.SortIndex = z;
    }

    private void PlaceFitted(
        int width,
        int height,
        bool job,
        bool name,
        bool numbers,
        bool hp,
        bool mp,
        bool shield,
        bool cast,
        bool buffs,
        bool debuffs,
        int icon,
        int nameBox,
        int numberBox,
        int bar,
        int mpBar,
        int shieldBar,
        int castText,
        int castBar,
        int buff,
        int debuff,
        int barWidth)
    {
        PlaceLayer(StateNode, 0, 0, width, height, 1);
        PlaceLayer(DeathNode, 0, 0, width, height, 200);

        if (job && !name && !numbers && !hp && !mp && !shield && !cast && !buffs && !debuffs)
        {
            PlaceLayer(IconNode, (width - icon) / 2, (height - icon) / 2, icon, icon, 10);
            return;
        }

        if (name && !job && !numbers && !hp && !mp && !shield && !cast && !buffs && !debuffs)
        {
            PlaceLayer(NameNode, 0, 0, width, height, 11);
            return;
        }

        var gap = 2;
        var x = 2;
        var header = 0;
        if (job)
            header = System.Math.Max(header, icon);
        if (name)
            header = System.Math.Max(header, nameBox);
        if (numbers && !hp)
            header = System.Math.Max(header, numberBox);

        var total = 0;
        var blocks = 0;
        void Count(int box)
        {
            if (box <= 0)
                return;

            total += box;
            blocks++;
        }

        Count(header);
        if (hp)
            Count(bar);
        if (mp)
            Count(mpBar);
        if (shield)
            Count(shieldBar);
        if (cast)
            Count(castText + gap + castBar);
        if (buffs)
            Count(buff);
        if (debuffs)
            Count(debuff);
        if (blocks > 1)
            total += gap * (blocks - 1);

        var y = (height - total) / 2;
        if (header > 0)
        {
            var textX = job ? x + icon + 4 : 0;
            var textW = job ? System.Math.Max(1, barWidth - icon - 4) : width;
            if (job)
                PlaceLayer(IconNode, name || numbers || hp ? x : (width - icon) / 2, y + (header - icon) / 2, icon, icon, 10);
            if (name)
                PlaceLayer(NameNode, textX, y + (header - nameBox) / 2, textW, nameBox, 11);
            if (numbers && !hp)
                PlaceLayer(NumbersNode, textX, y + (header - numberBox) / 2, textW, numberBox, 12);

            y += header + gap;
        }

        if (hp)
        {
            PlaceLayer(HpTrackNode, x, y, barWidth, bar, 20);
            if (numbers)
                PlaceLayer(NumbersNode, x, y, barWidth, bar, 30);

            y += bar + gap;
        }

        if (mp)
        {
            PlaceLayer(MpTrackNode, x, y, barWidth, mpBar, 21);
            y += mpBar + gap;
        }

        if (shield)
        {
            PlaceLayer(ShieldTrackNode, x, y, barWidth, shieldBar, 22);
            y += shieldBar + gap;
        }

        if (cast)
        {
            PlaceLayer(CastNameNode, 0, y, width, castText, 30);
            y += castText + gap;
            PlaceLayer(CastTrackNode, x, y, barWidth, castBar, 31);
            y += castBar + gap;
        }

        if (buffs)
        {
            var row = System.Math.Max(buff, BuffCount * (buff + 1));
            PlaceLayer(BuffRowNode, (width - row) / 2, y, row, buff, 40);
            y += buff + gap;
        }

        if (debuffs)
        {
            var row = System.Math.Max(debuff, DebuffCount * (debuff + 1));
            PlaceLayer(DebuffRowNode, (width - row) / 2, y, row, debuff, 41);
        }
    }

    private static void CenterRow(Node row, int iconSize)
    {
        var shown = 0;
        foreach (var child in row.ChildNodes)
        {
            if (child.Style.IsVisible == true)
                shown++;
        }

        var width = row.Style.Size?.Width ?? 0;
        var used = shown <= 0 ? 0 : shown * iconSize + (shown - 1);
        var pad = System.Math.Max(0, (width - used) / 2);
        row.Style.Padding = new EdgeSize(0, 0, 0, pad);
        row.Style.Flow = Flow.Horizontal;
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
        fill.Style.Anchor = Anchor.MiddleLeft;
        fill.Style.Size = new(span, height);
        fill.Style.BackgroundColor = color;
        fill.Style.IsVisible = span > 0;
    }

    private string Numbers(HudReader.Slot slot)
    {
        if (slot.MaxHp == 0)
            return "—";

        var percent = (int)(Fraction(slot.Hp, slot.MaxHp) * 100);
        var mode = GetConfigValue<string>("Numbers").Replace(" ", "").Replace("/", "").ToLowerInvariant();
        return mode switch
        {
            "none" => string.Empty,
            "current" => slot.Hp.ToString(),
            "currentmax" => $"{slot.Hp} / {slot.MaxHp}",
            "currentmaxpercent" => $"{slot.Hp} / {slot.MaxHp} ({percent}%)",
            _ => $"{percent}%",
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

    protected static IntegerWidgetConfigVariable Spot(string id, string name, int value, string category, string group, int min = -800, int max = 1200)
    {
        var moves = name is "X" or "Y" || name.EndsWith(" X") || name.EndsWith(" Y");
        var sizes = name.Contains("width", System.StringComparison.OrdinalIgnoreCase);
        var description = moves
            ? "Moves this piece. Does not change its size."
            : sizes
                ? "Width of this piece. Does not move it. 0 uses the default width."
                : "Draw order. Higher numbers paint on top.";

        return new IntegerWidgetConfigVariable(id, name, description, value, min, max)
        {
            Category = category,
            Group = group,
        };
    }

    protected static IEnumerable<IWidgetConfigVariable> NumberOptions(string category = "Numbers")
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
        ) { Category = category };
    }

    private static readonly Color DeadColor = new(80, 80, 80);
    private static readonly Color ShieldColor = new(235, 235, 235);
    private static readonly Color MpColor = new(70, 130, 200);
    private static readonly Color CastColor = new(230, 196, 90);
}
