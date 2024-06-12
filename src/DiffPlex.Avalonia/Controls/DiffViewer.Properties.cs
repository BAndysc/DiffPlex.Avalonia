using Avalonia;
using Avalonia.Media;
using DiffPlex.DiffBuilder.Model;

namespace DiffPlex.Avalonia.Controls;

public partial class DiffViewer
{
    public static readonly StyledProperty<IReadOnlyList<DiffPiece>?> LeftDiffProperty = AvaloniaProperty.Register<DiffViewer, IReadOnlyList<DiffPiece>?>(nameof(LeftDiff));

    public IReadOnlyList<DiffPiece>? LeftDiff
    {
        get => GetValue(LeftDiffProperty);
        set => SetValue(LeftDiffProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<DiffPiece>?> RightDiffProperty = AvaloniaProperty.Register<DiffViewer, IReadOnlyList<DiffPiece>?>(nameof(RightDiff));

    public IReadOnlyList<DiffPiece>? RightDiff
    {
        get => GetValue(RightDiffProperty);
        set => SetValue(RightDiffProperty, value);
    }

    public static readonly StyledProperty<bool> IgnoreUnchangedProperty = AvaloniaProperty.Register<DiffViewer, bool>(nameof(IgnoreUnchanged), false);

    public bool IgnoreUnchanged
    {
        get => GetValue(IgnoreUnchangedProperty);
        set => SetValue(IgnoreUnchangedProperty, value);
    }

    public static readonly StyledProperty<int> LinesContextProperty = AvaloniaProperty.Register<DiffViewer, int>(nameof(LinesContext), 1);

    public int LinesContext
    {
        get => GetValue(LinesContextProperty);
        set => SetValue(LinesContextProperty, value);
    }

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InsertedForegroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(InsertedForeground));

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InsertedBackgroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(InsertedBackground), new SolidColorBrush(Color.FromArgb(64, 96, 216, 32)));

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DeletedForegroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(DeletedForeground));

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DeletedBackgroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(DeletedBackground), new SolidColorBrush(Color.FromArgb(64, 216, 32, 32)));

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> UnchangedForegroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(UnchangedForeground));

    /// <summary>
    /// The property of text inserted background brush.
    /// </summary>
    public static readonly StyledProperty<IBrush?> UnchangedBackgroundProperty = AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(UnchangedBackground));

    public static readonly StyledProperty<bool> IsSideBySideProperty = AvaloniaProperty.Register<DiffViewer, bool>(nameof(IsSideBySide));

    public static readonly StyledProperty<string?> OldTextProperty = AvaloniaProperty.Register<DiffViewer, string?>(nameof(OldText));

    public static readonly StyledProperty<string?> NewTextProperty = AvaloniaProperty.Register<DiffViewer, string?>(nameof(NewText));

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

    public bool IsSideBySide
    {
        get => GetValue(IsSideBySideProperty);
        set => SetValue(IsSideBySideProperty, value);
    }

    public string? OldText
    {
        get { return (string?)GetValue(OldTextProperty); }
        set { SetValue(OldTextProperty, value); }
    }

    public string? NewText
    {
        get { return (string?)GetValue(NewTextProperty); }
        set { SetValue(NewTextProperty, value); }
    }
}