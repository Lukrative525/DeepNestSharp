﻿namespace DeepNestSharp.Ui.ViewModels
{
  using DeepNestLib;
  using DeepNestLib.NestProject;
  using DeepNestSharp.Ui.Docking;
  using DeepNestSharp.Ui.Models;

  public class SvgNestConfigViewModel : ToolViewModel
  {
    private readonly MainViewModel mainViewModel;
    private int selectedDetailLoadInfoIndex;
    private IDetailLoadInfo selectedDetailLoadInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgNestConfigViewModel"/> class.
    /// </summary>
    /// <param name="mainViewModel">MainViewModel singleton; the primary context; access this via the activeDocument property.</param>
    public SvgNestConfigViewModel(MainViewModel mainViewModel)
      : base(nameof(SvgNestConfigViewModel))
    {
      this.mainViewModel = mainViewModel;
    }

    public ISvgNestConfig SvgNestConfig { get; } = new ObservableSvgNestConfig(SvgNest.Config);
  }
}