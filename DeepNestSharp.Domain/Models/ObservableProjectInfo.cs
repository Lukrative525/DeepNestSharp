﻿namespace DeepNestSharp.Ui.Models
{
  using System;
  using CommunityToolkit.Mvvm.ComponentModel;
  using DeepNestLib;
  using DeepNestLib.NestProject;
  using DeepNestSharp.Domain.Models;
  using DeepNestSharp.Domain.ViewModels;
  using Light.GuardClauses;

  public class ObservableProjectInfo : ObservableObject, IProjectInfo
  {
    private readonly IMainViewModel mainViewModel;
    private readonly IProjectInfo wrappedProjectInfo;
    private ObservableCollection<IDetailLoadInfo, DetailLoadInfo, ObservableDetailLoadInfo>? detailLoadInfos;
    private ObservableCollection<ISheetLoadInfo, SheetLoadInfo, ObservableSheetLoadInfo>? sheetLoadInfos;
    private ObservableSvgNestConfig? observableConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableProjectInfo"/> class.
    /// </summary>
    /// <param name="projectInfo">The ProjectInfo to wrap.</param>
    public ObservableProjectInfo(IMainViewModel mainViewModel)
    {
      this.mainViewModel = mainViewModel;
      this.wrappedProjectInfo = new ProjectInfo(mainViewModel.SvgNestConfigViewModel.SvgNestConfig);
    }

    public event EventHandler? IsDirtyChanged;

    public IList<IDetailLoadInfo, DetailLoadInfo> DetailLoadInfos
    {
      get
      {
        if (this.detailLoadInfos == null || this.detailLoadInfos.Count == 0)
        {
          this.detailLoadInfos = new ObservableCollection<IDetailLoadInfo, DetailLoadInfo, ObservableDetailLoadInfo>(this.wrappedProjectInfo.DetailLoadInfos, x => new ObservableDetailLoadInfo(x));
          this.detailLoadInfos.IsDirtyChanged += this.DetailLoadInfos_IsDirtyChanged;
        }

        return this.detailLoadInfos;
      }
    }

    public IList<ISheetLoadInfo, SheetLoadInfo> SheetLoadInfos
    {
      get
      {
        if (this.sheetLoadInfos == null || this.sheetLoadInfos.Count == 0)
        {
          this.sheetLoadInfos = new ObservableCollection<ISheetLoadInfo, SheetLoadInfo, ObservableSheetLoadInfo>(this.wrappedProjectInfo.SheetLoadInfos, x => new ObservableSheetLoadInfo(x));
        }

        return this.sheetLoadInfos;
      }
    }

    public ISvgNestConfig Config
    {
      get
      {
        if (this.observableConfig == null)
        {
          this.observableConfig = (ObservableSvgNestConfig)this.mainViewModel.SvgNestConfigViewModel.SvgNestConfig;
        }

        return this.observableConfig;
      }
    }

    private void DetailLoadInfos_IsDirtyChanged(object? sender, EventArgs e)
    {
      this.IsDirtyChanged?.Invoke(this, e);
    }

    public void Load(ISvgNestConfig config, string filePath)
    {
      // This is a fudge; need a better way of injecting Config; loathe to go Locator but AvalonDock's pushing that way.
      config.MustBe(this.mainViewModel.SvgNestConfigViewModel.SvgNestConfig);
      this.DetailLoadInfos.Clear();
      this.SheetLoadInfos.Clear();
      this.wrappedProjectInfo.Load(config, filePath);
      this.OnPropertyChanged(nameof(this.DetailLoadInfos));
      this.OnPropertyChanged(nameof(this.SheetLoadInfos));
      this.mainViewModel.SvgNestConfigViewModel.RaiseNotifyUpdatePropertyGrid();
    }

    public void Load(ProjectInfo source)
    {
      this.wrappedProjectInfo.Load(source);
    }

    public string ToJson()
    {
      return this.wrappedProjectInfo.ToJson();
    }

    public void SaveState()
    {
      this.detailLoadInfos?.SaveState();
    }
  }
}
