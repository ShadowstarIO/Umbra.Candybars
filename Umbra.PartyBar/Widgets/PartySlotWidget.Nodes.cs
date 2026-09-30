using Una.Drawing;

namespace Umbra.PartyBar.Widgets;

public sealed partial class PartySlotWidget
{
    public override Node Node { get; } = new()
    {
        Stylesheet = Stylesheet,
        ClassList = ["party-slot"],
        ChildNodes =
        [
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
            new()
            {
                Id = "hp",
                ClassList = ["track", "hp"],
                ChildNodes =
                [
                    new() { Id = "hp-fill", ClassList = ["fill"] },
                    new() { Id = "shield-fill", ClassList = ["fill", "shield"] },
                ],
            },
            new()
            {
                Id = "mp",
                ClassList = ["track", "mp"],
                ChildNodes =
                [
                    new() { Id = "mp-fill", ClassList = ["fill"] },
                ],
            },
        ],
    };

    private Node HeaderNode => Node.QuerySelector("#header")!;
    private Node IconNode => Node.QuerySelector("#icon")!;
    private Node NameNode => Node.QuerySelector("#name")!;
    private Node NumbersNode => Node.QuerySelector("#numbers")!;
    private Node HpTrackNode => Node.QuerySelector("#hp")!;
    private Node HpFillNode => Node.QuerySelector("#hp-fill")!;
    private Node ShieldNode => Node.QuerySelector("#shield-fill")!;
    private Node MpTrackNode => Node.QuerySelector("#mp")!;
    private Node MpFillNode => Node.QuerySelector("#mp-fill")!;

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                ".party-slot",
                new()
                {
                    Flow = Flow.Vertical,
                    Gap = 2,
                    Anchor = Anchor.TopLeft,
                    Size = new(220, 40),
                    Padding = new(4),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor = new("Widget.Border"),
                    StrokeWidth = 1,
                    BorderRadius = 4,
                }
            ),
            new(
                ".header",
                new()
                {
                    Flow = Flow.Horizontal,
                    Gap = 4,
                    Size = new(0, 18),
                    Anchor = Anchor.TopLeft,
                }
            ),
            new(
                ".icon",
                new()
                {
                    Size = new(18, 18),
                    Anchor = Anchor.MiddleLeft,
                }
            ),
            new(
                ".name",
                new()
                {
                    FontSize = 13,
                    Color = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize = 1,
                    AutoSize = (AutoSize.Grow, AutoSize.Fit),
                    Size = new(0, 18),
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
                    Size = new(0, 18),
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
                    AutoSize = (AutoSize.Grow, AutoSize.Fit),
                    Size = new(0, 8),
                    BackgroundColor = new(0, 0, 0, (byte)120),
                    BorderRadius = 2,
                }
            ),
            new(
                ".track.mp",
                new()
                {
                    Size = new(0, 4),
                }
            ),
            new(
                ".fill",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    Size = new(0, 8),
                    BorderRadius = 2,
                }
            ),
        ]
    );
}
