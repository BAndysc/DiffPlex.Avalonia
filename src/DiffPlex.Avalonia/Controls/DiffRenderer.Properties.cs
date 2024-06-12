using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using DiffPlex.DiffBuilder.Model;

namespace DiffPlex.Avalonia.Controls;

internal partial class DiffRenderer
{
    public static readonly StyledProperty<IReadOnlyList<DiffPiece>?> DiffProperty = AvaloniaProperty.Register<DiffRenderer, IReadOnlyList<DiffPiece>?>(nameof(Diff));

    public IReadOnlyList<DiffPiece>? Diff
    {
        get => GetValue(DiffProperty);
        set => SetValue(DiffProperty, value);
    }

    public static readonly StyledProperty<bool> IsNewSideProperty = AvaloniaProperty.Register<DiffRenderer, bool>(nameof(IsNewSide));

    public bool IsNewSide
    {
        get => GetValue(IsNewSideProperty);
        set => SetValue(IsNewSideProperty, value);
    }

    public static readonly StyledProperty<FontFamily> FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner<DiffRenderer>();

    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly StyledProperty<double> FontSizeProperty = TextElement.FontSizeProperty.AddOwner<DiffRenderer>();

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly StyledProperty<double> PaddingProperty = AvaloniaProperty.Register<DiffRenderer, double>(nameof(Padding), 10);

    public double Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public static readonly StyledProperty<TextPosition> SelectionStartProperty = AvaloniaProperty.Register<DiffRenderer, TextPosition>(nameof(SelectionStart));

    public TextPosition SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    public static readonly StyledProperty<TextPosition> SelectionEndProperty = AvaloniaProperty.Register<DiffRenderer, TextPosition>(nameof(SelectionEnd));

    public TextPosition SelectionEnd
    {
        get => GetValue(SelectionEndProperty);
        set => SetValue(SelectionEndProperty, value);
    }

    public static readonly DirectProperty<DiffRenderer, Vector> OffsetProperty =
        AvaloniaProperty.RegisterDirect<DiffRenderer, Vector>(
            nameof(Offset),
            o => o.Offset,
            (o, v) => o.Offset = v);

    public static readonly StyledProperty<IBrush?> ForegroundProperty = TextElement.ForegroundProperty.AddOwner<DiffViewer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InsertedForegroundProperty = DiffViewer.InsertedForegroundProperty.AddOwner<DiffRenderer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InsertedBackgroundProperty = DiffViewer.InsertedBackgroundProperty.AddOwner<DiffRenderer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DeletedForegroundProperty = DiffViewer.DeletedForegroundProperty.AddOwner<DiffRenderer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DeletedBackgroundProperty = DiffViewer.DeletedBackgroundProperty.AddOwner<DiffRenderer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> UnchangedForegroundProperty = DiffViewer.UnchangedForegroundProperty.AddOwner<DiffRenderer>();

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> UnchangedBackgroundProperty = DiffViewer.UnchangedBackgroundProperty.AddOwner<DiffRenderer>();

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush? InsertedForeground
    {
        get => (IBrush?)GetValue(InsertedForegroundProperty);
        set => SetValue(InsertedForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush of the line added.
    /// </summary>
    public IBrush? InsertedBackground
    {
        get => (IBrush?)GetValue(InsertedBackgroundProperty);
        set => SetValue(InsertedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush of the line deleted.
    /// </summary>
    public IBrush? DeletedForeground
    {
        get => (IBrush?)GetValue(DeletedForegroundProperty);
        set => SetValue(DeletedForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush of the line deleted.
    /// </summary>
    public IBrush? DeletedBackground
    {
        get => (IBrush?)GetValue(DeletedBackgroundProperty);
        set => SetValue(DeletedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush of the line unchanged.
    /// </summary>
    public IBrush? UnchangedForeground
    {
        get => (IBrush?)GetValue(UnchangedForegroundProperty);
        set => SetValue(UnchangedForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush of the line unchanged.
    /// </summary>
    public IBrush? UnchangedBackground
    {
        get => (IBrush?)GetValue(UnchangedBackgroundProperty);
        set => SetValue(UnchangedBackgroundProperty, value);
    }

    public Vector Offset
    {
        get => offset;
        set => SetAndRaise(OffsetProperty, ref offset, value);
    }

}