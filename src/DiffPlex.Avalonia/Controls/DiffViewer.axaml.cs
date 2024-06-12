using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using DiffPlex.DiffBuilder.Model;

namespace DiffPlex.Avalonia.Controls;

public partial class DiffViewer : TemplatedControl
{
    private DiffRenderer? leftRenderer;
    private DiffRenderer? rightRenderer;

    static DiffViewer()
    {
        IsSideBySideProperty.Changed.AddClassHandler<DiffViewer>((d, e) =>
        {
            d.UpdateDiff();
        });
        OldTextProperty.Changed.AddClassHandler<DiffViewer>((d, e) =>
        {
            d.UpdateDiff();
        });
        NewTextProperty.Changed.AddClassHandler<DiffViewer>((d, e) =>
        {
            d.UpdateDiff();
        });
        IgnoreUnchangedProperty.Changed.AddClassHandler<DiffViewer>((d, e) =>
        {
            d.UpdateDiff();
        });
        LinesContextProperty.Changed.AddClassHandler<DiffViewer>((d, e) =>
        {
            d.UpdateDiff();
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        leftRenderer = e.NameScope.Get<DiffRenderer>("PART_LeftRenderer");
        rightRenderer = e.NameScope.Get<DiffRenderer>("PART_RightRenderer");
    }

    internal static void CollapseUnchangedSections(List<DiffPiece> pieces, int contextLineCount)
    {
        var i = -1;
        var was = false;
        var last = 0;
        var max = -1;
        var removing = new List<int>();
        foreach (var ele in pieces)
        {
            i++;
            if (ele.Type != ChangeType.Unchanged)
            {
                if (!was)
                {
                    was = true;
                    if (contextLineCount > 0)
                    {
                        var first = Math.Max(last, removing.Count - contextLineCount);
                        removing.RemoveRange(first, removing.Count - first);
                    }
                }

                continue;
            }

            if (was)
            {
                was = false;
                last = removing.Count;
                max = i + contextLineCount;
            }

            if (i < max) continue;
            removing.Add(i);
        }

        removing.Reverse();
        foreach (var j in removing)
        {
            pieces.RemoveAt(j);
        }
    }

    private void UpdateDiff()
    {
        if (IsSideBySide)
        {
            var diff = DiffBuilder.SideBySideDiffBuilder.Instance.BuildDiffModel(OldText ?? "", NewText ?? "");
            if (IgnoreUnchanged)
            {
                CollapseUnchangedSections(diff.OldText.Lines, LinesContext);
                CollapseUnchangedSections(diff.NewText.Lines, LinesContext);
            }

            SetCurrentValue(LeftDiffProperty, diff.OldText.Lines);
            SetCurrentValue(RightDiffProperty, diff.NewText.Lines);
        }
        else
        {
            var diff = DiffBuilder.InlineDiffBuilder.Instance.BuildDiffModel(OldText ?? "", NewText ?? "");
            if (IgnoreUnchanged)
            {
                CollapseUnchangedSections(diff.Lines, LinesContext);
            }

            SetCurrentValue(LeftDiffProperty, diff.Lines);
            SetCurrentValue(RightDiffProperty, null);
        }
    }
}