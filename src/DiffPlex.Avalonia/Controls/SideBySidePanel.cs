using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace DiffPlex.Avalonia.Controls;

internal class SideBySidePanel : Panel, ILogicalScrollable
{
    public Size Extent => LogicalScrollables.Count > 0 ? new Size(LogicalScrollables.Select(x => x.Extent.Width).Max(), LogicalScrollables.Select(x => x.Extent.Height).Max()) : new Size(0, 0);

    private Vector offset;

    public static readonly DirectProperty<SideBySidePanel, Vector> OffsetProperty =
        AvaloniaProperty.RegisterDirect<SideBySidePanel, Vector>(
            nameof(IScrollable.Offset),
            o => (o as IScrollable).Offset,
            (o, v) => (o as IScrollable).Offset = v);

    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<SideBySidePanel, double>(nameof(Spacing));

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public Vector Offset
    {
        get => offset;
        set => SetAndRaise(OffsetProperty, ref offset, value);
    }

    public Size Viewport => LogicalScrollables.Count > 0 ? new Size(LogicalScrollables.Select(x => x.Viewport.Width).Max(), LogicalScrollables.Select(x => x.Viewport.Height).Max()) : new Size(0, 0);

    public bool BringIntoView(Control target, Rect targetRect)
    {
        return true;
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

    public Size ScrollSize => LogicalScrollables.Count > 0 ? new Size(LogicalScrollables.Select(x => x.ScrollSize.Width).Max(), LogicalScrollables.Select(x => x.ScrollSize.Height).Max()) : new Size(0, 0);

    public Size PageScrollSize => LogicalScrollables.Count > 0 ? new Size(LogicalScrollables.Select(x => x.PageScrollSize.Width).Max(), LogicalScrollables.Select(x => x.PageScrollSize.Height).Max()) : new Size(0, 0);

    public event EventHandler? ScrollInvalidated;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var childrenCount = Children.Count(c => c.IsEffectivelyVisible);
        var widthPerChildren = childrenCount > 0 ? finalSize.Width / childrenCount - (Math.Max(childrenCount - 1, 0) * Spacing) : 0;
        double x = 0;
        foreach (var child in Children)
        {
            if (child.IsEffectivelyVisible)
            {
                child.Arrange(new Rect(new Point(x, 0), new Size(widthPerChildren, finalSize.Height)));
                x += widthPerChildren + Spacing;
            }
        }
        return finalSize;
    }

    static SideBySidePanel()
    {
        AffectsArrange<SideBySidePanel>(SpacingProperty);
    }

    public SideBySidePanel()
    {
        Children.CollectionChanged += OnChildrenChanged;
    }

    private List<ILogicalScrollable> LogicalScrollables { get; } = new List<ILogicalScrollable>();

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var newItem in e.NewItems)
            {
                if (newItem is ILogicalScrollable logicalScrollable)
                {
                    logicalScrollable.ScrollInvalidated += OnChildrenScrollInvlidated;
                    LogicalScrollables.Add(logicalScrollable);
                }
            }
        }
        else if (e.OldItems != null)
        {
            foreach (var oldItem in e.OldItems)
            {
                if (oldItem is ILogicalScrollable logicalScrollable)
                {
                    LogicalScrollables.Remove(logicalScrollable);
                    logicalScrollable.ScrollInvalidated -= OnChildrenScrollInvlidated;
                }
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var scroll in LogicalScrollables)
            {
                scroll.ScrollInvalidated -= OnChildrenScrollInvlidated;
            }
            LogicalScrollables.Clear();
        }
    }

    private void OnChildrenScrollInvlidated(object? sender, EventArgs e)
    {
        RaiseScrollInvalidated(e);
    }
}