﻿namespace DeepNestSharp.Ui.Behaviors
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using DeepNestSharp.Domain.Models;
  using DeepNestSharp.Domain.ViewModels;
  using Microsoft.Xaml.Behaviors;

  public class PolygonMouseHover : Behavior<FrameworkElement>
  {
    private IMainViewModel? mainViewModel;
    private ObservablePartPlacement? partPlacement;

    protected override void OnAttached()
    {
      base.OnAttached();
      if (this.AssociatedObject.GetVisualParent<Window>() is Window window &&
          this.AssociatedObject.DataContext is ObservablePartPlacement partPlacement &&
          window.DataContext is IMainViewModel mainViewModel)
      {
        this.mainViewModel = mainViewModel;
        this.partPlacement = partPlacement;

        this.AssociatedObject.MouseEnter += this.AssociatedObject_MouseEnter;
        this.AssociatedObject.MouseLeave += this.AssociatedObject_MouseLeave;
      }
    }

    private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.AssociatedObject.GetVisualParent<Canvas>() is Canvas canvas &&
          this.mainViewModel != null &&
          !this.mainViewModel.PreviewViewModel.IsDragging)
      {
        canvas.Focus();
        if (this.mainViewModel != null)
        {
          this.mainViewModel.PreviewViewModel.HoverPartPlacement = this.partPlacement;
          System.Diagnostics.Debug.WriteLine($"Hover {this.partPlacement?.Id ?? -1}");
        }
      }
    }

    private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mainViewModel != null &&
          !this.mainViewModel.PreviewViewModel.IsDragging)
      {
        this.mainViewModel.PreviewViewModel.HoverPartPlacement = null;
      }
    }
  }
}