using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using DiffPlex.DiffBuilder.Model;

namespace DiffPlex.Avalonia.Controls;

internal partial class DiffRenderer : Control, ILogicalScrollable
{
    private Point monoSpaceFontMetrics;

    private double LineHeight => monoSpaceFontMetrics.Y;

    private double CharWidth => monoSpaceFontMetrics.X;

    private Vector offset;

    private double maxWidthYet;

    static DiffRenderer()
    {
        AffectsArrange<DiffRenderer>(DiffProperty, FontFamilyProperty, FontSizeProperty, PaddingProperty);
        AffectsRender<DiffRenderer>(DiffProperty, FontFamilyProperty, IsNewSideProperty, FontSizeProperty, OffsetProperty, PaddingProperty, InsertedForegroundProperty, InsertedBackgroundProperty, DeletedForegroundProperty, DeletedBackgroundProperty, UnchangedForegroundProperty, UnchangedBackgroundProperty, SelectionStartProperty, SelectionEndProperty);
        FocusableProperty.OverrideDefaultValue<DiffRenderer>(true);
        ClipToBoundsProperty.OverrideDefaultValue<DiffRenderer>(true);
        OffsetProperty.Changed.AddClassHandler<DiffRenderer>((renderer, e) => renderer.UpdateExtents());
        DiffProperty.Changed.AddClassHandler<DiffRenderer>((renderer, e) =>
        {
            renderer.maxWidthYet = 0;
            renderer.SetCurrentValue(SelectionStartProperty, default);
            renderer.SetCurrentValue(SelectionEndProperty, default);
        });
    }

    private void UpdateExtents()
    {
        if (Diff != null)
        {
            var firstVisibleLine = (int)(Offset.Y / LineHeight);
            var lastVisibleLine = (int)((Offset.Y + Viewport.Height) / LineHeight) + 1;

            var actualFirstVisibleLine = Utils.Clamp(firstVisibleLine, 0, Diff.Count);
            var actualLastVisibleLine = Utils.Clamp(lastVisibleLine, 0, Diff.Count);

            for (int i = actualFirstVisibleLine; i < actualLastVisibleLine; ++i)
            {
                var diff = Diff[i];
                if (diff.Text == null)
                    continue;

                var ft = new FormattedText(diff.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, Foreground);
                var width = ft.WidthIncludingTrailingWhitespace + Padding;
                if (diff.Position.HasValue)
                {
                    var lineNumberFt = new FormattedText(diff.Position.Value.ToString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, Foreground);
                    width += lineNumberFt.WidthIncludingTrailingWhitespace;
                }
                width += CharWidth + Padding; // for +- sign

                maxWidthYet = Math.Max(maxWidthYet, width);
            }

            var oldExtents = Extent;
            Extent = new Size(maxWidthYet, LineHeight * Diff.Count);

            if (Math.Abs(oldExtents.Width - Extent.Width) > 0.1 || Math.Abs(oldExtents.Height - Extent.Height) > 0.1)
            {
                RaiseScrollInvalidated(EventArgs.Empty);
            }
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var letter = new FormattedText("A", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, Brushes.Black);
        monoSpaceFontMetrics = new Point(letter.WidthIncludingTrailingWhitespace, letter.Height);

        if (Diff != null)
        {
            Viewport = finalSize;
            ScrollSize = new Size(letter.WidthIncludingTrailingWhitespace, LineHeight);
            PageScrollSize = new Size(finalSize.Width, LineHeight * (int)(finalSize.Height / LineHeight));
            UpdateExtents();
            RaiseScrollInvalidated(EventArgs.Empty);
        }
        return base.ArrangeOverride(finalSize);
    }

    private double leftColumnWidth = 0.0;

    private double TextStartLeft => leftColumnWidth + Padding;

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.Transparent, new Rect(0, 0, Bounds.Width, Bounds.Height));

        if (Diff == null)
            return;

        var plusText = new FormattedText("+", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle.Normal, FontWeight.Bold), FontSize, InsertedForeground ?? Foreground);

        var minusText = new FormattedText("-", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle.Normal, FontWeight.Bold), FontSize, DeletedForeground ?? Foreground);

        var viewport = new Rect(Offset.X, Offset.Y, Viewport.Width, Viewport.Height);
        double y = 0;

        var firstVisibleLine = (int)(Offset.Y / LineHeight);
        var lastVisibleLine = (int)((Offset.Y + Viewport.Height) / LineHeight) + 1;

        var actualFirstVisibleLine = Utils.Clamp(firstVisibleLine, 0, Diff.Count);
        var actualLastVisibleLine = Utils.Clamp(lastVisibleLine, 0, Diff.Count);

        y = actualFirstVisibleLine * LineHeight;

        leftColumnWidth = 0.0;
        for (int i = actualFirstVisibleLine; i < actualLastVisibleLine; ++i)
        {
            var diff = Diff[i];
            if (diff.Position.HasValue)
            {
                var lineNumber = new FormattedText(diff.Position.Value.ToString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, Foreground);
                leftColumnWidth = Math.Max(leftColumnWidth, lineNumber.WidthIncludingTrailingWhitespace);

                context.DrawText(lineNumber, new Point(0, y - Offset.Y));
            }
            else
            {
            }

            y += LineHeight;
        }

        leftColumnWidth += Padding;
        y = actualFirstVisibleLine * LineHeight;
        for (int i = actualFirstVisibleLine; i < actualLastVisibleLine; ++i)
        {
            var diff = Diff[i];
            var background = UnchangedBackground;

            if (diff.Type == ChangeType.Deleted)
                background = DeletedBackground;
            else if (diff.Type == ChangeType.Inserted)
                background = InsertedBackground;
            //else if (diff.Type == ChangeType.Modified)
            //    background = IsNewSide ? InsertedBackground : DeletedBackground;

            if (background != null)
            {
                context.FillRectangle(background, new Rect(0, y - Offset.Y, Bounds.Width, LineHeight));
            }

            if (diff.Type == ChangeType.Deleted)
                context.DrawText(minusText, new Point(leftColumnWidth, y - Offset.Y));
            else if (diff.Type == ChangeType.Inserted)
                context.DrawText(plusText, new Point(leftColumnWidth, y - Offset.Y));

            y += LineHeight;
        }

        leftColumnWidth += CharWidth; // for +- sign

        using var clip = context.PushClip(new Rect(leftColumnWidth, 0, Bounds.Width - leftColumnWidth, Bounds.Height));

        y = actualFirstVisibleLine * LineHeight;
        for (int i = actualFirstVisibleLine; i < actualLastVisibleLine; ++i)
        {
            var diff = Diff[i];
            double x = TextStartLeft;

            if (diff.SubPieces != null && diff.SubPieces.Count > 0)
            {
                foreach (var sub in diff.SubPieces)
                {
                    var foreground = UnchangedForeground;
                    IBrush? background = null;
                    if (sub.Type == ChangeType.Inserted)
                    {
                        foreground = InsertedForeground;
                        background = InsertedBackground;
                    }
                    else if (sub.Type == ChangeType.Deleted)
                    {
                        foreground = DeletedForeground;
                        background = DeletedBackground;
                    }

                    if (sub.Text != null)
                    {
                        var ft = new FormattedText(sub.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, foreground ?? Foreground);

                        if (background != null)
                        {
                            context.FillRectangle(background, new Rect(x - Offset.X, y - Offset.Y, ft.WidthIncludingTrailingWhitespace, LineHeight));
                        }

                        if (i >= SelectionStart.Line && i <= SelectionEnd.Line)
                        {
                            var start = i == SelectionStart.Line ? SelectionStart.Character * CharWidth : 0;
                            var end = i == SelectionEnd.Line ? SelectionEnd.Character * CharWidth : ft.WidthIncludingTrailingWhitespace + x;

                            start = Utils.Clamp(start + TextStartLeft, x, x + ft.WidthIncludingTrailingWhitespace);
                            end = Utils.Clamp(end + TextStartLeft, x, x + ft.WidthIncludingTrailingWhitespace);

                            if (start < end)
                                context.FillRectangle(Brushes.LightBlue, new Rect(start - Offset.X, y - Offset.Y, end - start, LineHeight));
                        }

                        context.DrawText(ft, new Point(x - Offset.X, y - Offset.Y));

                        x += ft.WidthIncludingTrailingWhitespace;
                    }
                }
            }
            else
            {
                var foreground = UnchangedForeground;
                if (diff.Type == ChangeType.Inserted)
                    foreground = InsertedForeground;
                else if (diff.Type == ChangeType.Deleted)
                    foreground = DeletedForeground;

                foreground ??= Foreground;

                if (diff.Text != null)
                {
                    var ft = new FormattedText(diff.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                        new Typeface(FontFamily), FontSize, foreground);

                    if (i >= SelectionStart.Line && i <= SelectionEnd.Line)
                    {
                        var start = i == SelectionStart.Line ? SelectionStart.Character * CharWidth : 0;
                        var end = i == SelectionEnd.Line ? SelectionEnd.Character * CharWidth : ft.WidthIncludingTrailingWhitespace;

                        context.FillRectangle(Brushes.LightBlue, new Rect(TextStartLeft + start - Offset.X, y - Offset.Y, end - start, LineHeight));
                    }

                    context.DrawText(ft, new Point(x - Offset.X, y - Offset.Y));
                }
            }

            y += LineHeight;
        }
    }

    private int GetLineLength(int lineIndex)
    {
        if (Diff == null || lineIndex < 0 || lineIndex >= Diff.Count)
            return 0;

        var diff = Diff[lineIndex];
        if (diff.SubPieces != null && diff.SubPieces.Count > 0)
        {
            return diff.SubPieces.Sum(x => x.Text?.Length ?? 0);
        }
        else
        {
            return diff.Text?.Length ?? 0;
        }
    }

    private TextPosition GetLineCharacterAt(Point point)
    {
        if (Diff == null)
            return default;

        point += Offset;
        var line = (int)(point.Y / LineHeight);
        var chr = (int)((point.X - TextStartLeft) / CharWidth);

        if (line < 0)
            line = 0;

        if (chr < 0)
            chr = 0;

        if (line >= Diff.Count)
        {
            line = Diff.Count - 1;
            chr = int.MaxValue;
        }

        var lineLength = GetLineLength(line);

        if (chr >= lineLength)
            chr = lineLength;

        return new TextPosition(line, chr);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var pressPosition = GetLineCharacterAt(e.GetPosition(this));
            SetCurrentValue(SelectionStartProperty, pressPosition);
            SetCurrentValue(SelectionEndProperty, pressPosition);
            selectionDirection = 0;
        }
        base.OnPointerPressed(e);
    }

    private int selectionDirection = 0;

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var pressPosition = GetLineCharacterAt(e.GetPosition(this));

            var dir = pressPosition.CompareTo(selectionDirection > 0 ? SelectionStart : SelectionEnd);

            if (dir != selectionDirection)
            {
                var oldStart = SelectionStart;
                var oldEnd = SelectionEnd;
                if (dir > 0)
                {
                    SetCurrentValue(SelectionStartProperty, oldEnd);
                }
                else if (dir < 0)
                {
                    SetCurrentValue(SelectionEndProperty, oldStart);
                }
                selectionDirection = dir;
            }

            if (selectionDirection > 0)
            {
                SetCurrentValue(SelectionEndProperty, pressPosition);
            }
            else if (selectionDirection < 0)
            {
                SetCurrentValue(SelectionStartProperty, pressPosition);
            }
        }
        base.OnPointerMoved(e);
    }

    private readonly struct Range
    {
        private readonly int start;
        private readonly int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int Start => start;

        public int End => end;

        public int Length => Math.Max(0, end - start);

        public bool IsEmpty => start >= end;

        public bool Overlaps(Range other) => start < other.end && end > other.start;

        public bool Contains(Range other) => start <= other.start && end >= other.end;

        public Range Intersect(Range other) => new Range(Math.Max(start, other.start), Math.Min(end, other.end));
    }

    public void SelectAll()
    {
        SelectionStart = new TextPosition(0, 0);
        SelectionEnd = new TextPosition(Diff?.Count ?? 0, 0);
    }

    public void Copy()
    {
        if (TopLevel.GetTopLevel(this)?.Clipboard is not { } clipboard)
            return;

        if (Diff == null)
            return;

        var start = SelectionStart;
        var end = SelectionEnd;

        StringBuilder sb = new StringBuilder();
        for (int i = Math.Max(0, start.Line); i <= end.Line; ++i)
        {
            var selectionRange = new Range(i == start.Line ? start.Character : 0, i == end.Line ? end.Character : int.MaxValue);

            if (i >= Diff.Count)
                break;

            var diff = Diff[i];
            if (diff.SubPieces != null && diff.SubPieces.Count > 0)
            {
                int x = 0;
                foreach (var sub in diff.SubPieces)
                {
                    var subRange = new Range(x, x + sub.Text?.Length ?? 0);
                    var intersection = selectionRange.Intersect(subRange);
                    if (intersection.IsEmpty)
                    {
                        x += sub.Text?.Length ?? 0;
                        continue;
                    }

                    if (sub.Text != null)
                    {
                        sb.Append(sub.Text.Substring(intersection.Start - x, intersection.Length));
                        x += sub.Text.Length;
                    }
                }
            }
            else if (diff.Text != null)
            {
                if (i == start.Line && i == end.Line)
                {
                    sb.Append(diff.Text.Substring(start.Character, end.Character - start.Character));
                }
                else if (i == start.Line)
                {
                    sb.Append(diff.Text.Substring(start.Character));
                }
                else if (i == end.Line)
                {
                    sb.Append(diff.Text.Substring(0, end.Character));
                }
                else
                {
                    sb.Append(diff.Text);
                }
            }
            if (i != end.Line)
                sb.AppendLine();
        }

        clipboard.SetTextAsync(sb.ToString());
    }

    public Size Extent { get; set; }

    public Size Viewport { get; set; }

    public bool BringIntoView(Control target, Rect targetRect)
    {
        Offset = new Vector(targetRect.Left, targetRect.Top);
        return true;
        //throw new NotImplementedException();
    }

    public Control? GetControlInDirection(NavigationDirection direction, Control? from)
    {
        return null;
    }

    public void RaiseScrollInvalidated(EventArgs e)
    {
        ScrollInvalidated?.Invoke(this, e);
    }

    public bool CanHorizontallyScroll { get; set; }
    public bool CanVerticallyScroll { get; set; }
    public bool IsLogicalScrollEnabled => true;
    public Size ScrollSize { get; set; }
    public Size PageScrollSize { get; set; }
    public event EventHandler? ScrollInvalidated;
}