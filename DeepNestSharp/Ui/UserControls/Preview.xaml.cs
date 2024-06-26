﻿namespace DeepNestSharp.Ui.UserControls
{
  using System.Windows;
  using System.Windows.Controls;
  using DeepNestLib;
  using DeepNestSharp.Domain.ViewModels;
  using DeepNestSharp.Ui.Behaviors;

  /// <summary>
  /// Interaction logic for Preview.xaml.
  /// </summary>
  public partial class Preview : UserControl
  {
    public Preview()
    {
      this.InitializeComponent();
      this.Loaded += this.Preview_Loaded;
    }

    private void Preview_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.DataContext is PreviewViewModel viewModel &&
          sender is Preview preview &&
          preview.GetVisualParent<Window>() is Window window &&
          preview.GetChildOfType<ScrollViewer>() is ScrollViewer scrollViewer)
      {
        scrollViewer.SizeChanged += this.ScrollViewer_SizeChanged;
        //scrollViewer.LayoutUpdated += this.ScrollViewer_LayoutUpdated;
        window.StateChanged += this.Window_StateChanged;
        SetViewport(viewModel, scrollViewer);
      }
    }

    private void Window_StateChanged(object? sender, System.EventArgs e)
    {
      if (this.DataContext is PreviewViewModel viewModel &&
          sender is Window window &&
          window.GetChildOfType<Preview>() is Preview preview &&
          preview.GetChildOfType<ScrollViewer>() is ScrollViewer scrollViewer)
      {
        switch (window.WindowState)
        {
          case WindowState.Maximized:
            SetViewport(viewModel, scrollViewer);
            break;
        }
      }
    }

    private void ScrollViewer_LayoutUpdated(object? sender, System.EventArgs e)
    {
      if (this.DataContext is PreviewViewModel viewModel &&
          sender is ScrollViewer scrollViewer)
      {
        SetViewport(viewModel, scrollViewer);
      }
    }

    private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.DataContext is PreviewViewModel viewModel &&
          sender is ScrollViewer scrollViewer)
      {
        SetViewport(viewModel, scrollViewer);
      }
    }

    private static void SetViewport(PreviewViewModel viewModel, ScrollViewer scrollViewer)
    {
      viewModel.Viewport = new SvgPoint(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight);
      viewModel.Actual = new SvgPoint(scrollViewer.ActualWidth, scrollViewer.ActualHeight);
    }
  }
}