using System.Collections.Generic;
using Una.Drawing;

namespace Umbra.PartyBar.Widgets;

public abstract partial class CandyWidget
{
    public override Node Node { get; } = BuildNode();

    private Node SizerNode => Node.QuerySelector("#sizer")!;
    private Node StateNode => Node.QuerySelector("#state")!;
    private Node DeathNode => Node.QuerySelector("#death")!;
    private Node HeaderNode => Node.QuerySelector("#header")!;
    private Node IconNode => Node.QuerySelector("#icon")!;
    private Node NameNode => Node.QuerySelector("#name")!;
    private Node NumbersNode => Node.QuerySelector("#numbers")!;
    private Node HpTrackNode => Node.QuerySelector("#hp")!;
    private Node HpFillNode => Node.QuerySelector("#hp-fill")!;
    private Node ShieldNode => Node.QuerySelector("#shield-fill")!;
    private Node MpTrackNode => Node.QuerySelector("#mp")!;
    private Node MpFillNode => Node.QuerySelector("#mp-fill")!;
    private Node ShieldTrackNode => Node.QuerySelector("#shield-bar")!;
    private Node ShieldBarFillNode => Node.QuerySelector("#shield-bar-fill")!;
    private Node CastWrapNode => Node.QuerySelector("#cast")!;
    private Node CastNameNode => Node.QuerySelector("#cast-name")!;
    private Node CastTrackNode => Node.QuerySelector("#cast-track")!;
    private Node CastFillNode => Node.QuerySelector("#cast-fill")!;
    private Node BuffRowNode => Node.QuerySelector("#buffs")!;
    private Node DebuffRowNode => Node.QuerySelector("#debuffs")!;

    private static Node BuildNode()
    {
        return new Node
        {
            Stylesheet = Stylesheet,
            ClassList = ["candy"],
            ChildNodes =
            [
                new() { Id = "sizer", ClassList = ["sizer"] },
                new() { Id = "state", ClassList = ["state"] },
                new()
                {
                    Id = "header",
                    ClassList = ["header"],
                    ChildNodes =
                    [
                        new() { Id = "icon", ClassList = ["icon"] },
                        new() { Id = "name", ClassList = ["name"] },
                        new() { Id = "numbers", ClassList = ["numbers"] },
                    ],
                },
                Track("hp", "hp-fill", "shield-fill"),
                Track("mp", "mp-fill"),
                Track("shield-bar", "shield-bar-fill"),
                new()
                {
                    Id = "cast",
                    ClassList = ["cast"],
                    ChildNodes =
                    [
                        new() { Id = "cast-name", ClassList = ["cast-name"] },
                        Track("cast-track", "cast-fill"),
                    ],
                },
                StatusRow("buffs"),
                StatusRow("debuffs"),
                new() { Id = "death", ClassList = ["death"], NodeValue = "DEAD" },
            ],
        };
    }

    private static Node Track(string id, params string[] fills)
    {
        var track = new Node { Id = id, ClassList = ["track"] };
        foreach (var fill in fills)
            track.AppendChild(new Node { Id = fill, ClassList = ["fill"] });

        return track;
    }

    private static Node StatusRow(string id)
    {
        var row = new Node { Id = id, ClassList = ["statuses"] };
        for (var i = 0; i < 12; i++)
            row.AppendChild(new Node { Id = $"{id}-{i}", ClassList = ["status"] });

        return row;
    }

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                ".candy",
                new()
                {
                    Flow = Flow.Vertical,
                    Gap = 2,
                    Anchor = Anchor.TopLeft,
                    Padding = new(4),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor = new("Widget.Border"),
                    StrokeWidth = 1,
                    BorderRadius = 4,
                }
            ),
            new(
                ".candy:plain",
                new()
                {
                    BackgroundColor = new(0),
                    StrokeColor = new(0),
                    Padding = new(0),
                }
            ),
            new(".sizer", new() { Size = new(240, 0) }),
            new(
                ".state",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    BorderRadius = 3,
                }
            ),
            new(
                ".death",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    FontSize = 13,
                    Color = new(235, 210, 210),
                    OutlineColor = new(0, 0, 0),
                    OutlineSize = 1,
                    TextAlign = Anchor.MiddleCenter,
                }
            ),
            new(
                ".header",
                new()
                {
                    Flow = Flow.Horizontal,
                    Gap = 4,
                    Anchor = Anchor.TopLeft,
                }
            ),
            new(".icon", new() { Anchor = Anchor.MiddleLeft, Size = new(20, 20) }),
            new(
                ".name",
                new()
                {
                    FontSize = 13,
                    Color = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize = 1,
                    AutoSize = (AutoSize.Grow, AutoSize.Fit),
                    Anchor = Anchor.MiddleLeft,
                    TextAlign = Anchor.MiddleLeft,
                    TextOverflow = false,
                    WordWrap = false,
                }
            ),
            new(
                ".numbers",
                new()
                {
                    FontSize = 12,
                    Color = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize = 1,
                    Anchor = Anchor.MiddleRight,
                    TextAlign = Anchor.MiddleRight,
                    TextOverflow = false,
                    WordWrap = false,
                }
            ),
            new(
                ".track",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    BackgroundColor = new(0, 0, 0, (byte)120),
                    BorderRadius = 2,
                }
            ),
            new(".fill", new() { Anchor = Anchor.TopLeft, BorderRadius = 2 }),
            new(
                ".cast",
                new()
                {
                    Flow = Flow.Vertical,
                    Gap = 1,
                    Anchor = Anchor.TopLeft,
                }
            ),
            new(
                ".cast-name",
                new()
                {
                    FontSize = 11,
                    Color = new("Widget.Text"),
                    OutlineSize = 1,
                    OutlineColor = new("Widget.TextOutline"),
                    TextOverflow = false,
                    WordWrap = false,
                }
            ),
            new(
                ".statuses",
                new()
                {
                    Flow = Flow.Horizontal,
                    Gap = 1,
                    Anchor = Anchor.TopLeft,
                }
            ),
            new(
                ".status",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    FontSize = 10,
                    Color = new(255, 255, 255),
                    OutlineColor = new(0, 0, 0),
                    OutlineSize = 1,
                    TextAlign = Anchor.BottomRight,
                    StrokeWidth = 1,
                    BorderRadius = 2,
                }
            ),
        ]
    );
}
