using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using DiffPlex.Model;

namespace DiffPlex.Avalonia.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        DiffView.OldText = TestData.DuplicateText(TestData.OldText, 100);
        DiffView.NewText = TestData.DuplicateText(TestData.NewText, 100);
    }
}