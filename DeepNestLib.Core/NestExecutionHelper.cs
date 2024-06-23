﻿namespace DeepNestLib
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using DeepNestLib.IO;
  using DeepNestLib.NestProject;

  public class NestExecutionHelper
  {
    public void InitialiseNest(NestingContext context, IList<ISheetLoadInfo> sheetLoadInfos, IList<IDetailLoadInfo> detailLoadInfos, IProgressDisplayer progressDisplayer)
    {
      progressDisplayer.IsVisibleSecondaryProgressBar = false;
      context.Reset();
      int src = 0;
      foreach (ISheetLoadInfo item in sheetLoadInfos)
      {
        src = context.GetNextSheetSource();
        if (item.SheetType == SheetTypeEnum.Arbitrary)
        {
          IRawDetail det = this.LoadRawDetail(new FileInfo(item.Path));
          INfp loadedNfp;
          if (det.TryConvertToNfp(src, out loadedNfp))
          {
            for (int i = 0; i < item.Quantity; i++)
            {
              Sheet ns = Sheet.NewSheet(context.Sheets.Count + 1, loadedNfp);
              context.Sheets.Add(ns);
              ns.Source = src;
            }
          }
          else
          {
            progressDisplayer.DisplayMessageBox($"Failed to import {det.Name}.", "Load Error", MessageBoxIcon.Stop);
          }
        }
        else
        {
          for (int i = 0; i < item.Quantity; i++)
          {
            Sheet ns = Sheet.NewSheet(context.Sheets.Count + 1, item.Width, item.Height);
            context.Sheets.Add(ns);
            ns.Source = src;
          }
        }
      }

      context.ReorderSheets();
      src = 0;
      foreach (IDetailLoadInfo item in detailLoadInfos.Where(o => o.IsIncluded))
      {
        progressDisplayer.DisplayTransientMessage($"Preload {item.Path}...");
        IRawDetail det = this.LoadRawDetail(new FileInfo(item.Path));

        this.AddToPolygons(context, src, det, item.Quantity, progressDisplayer, isPriority: item.IsPriority, isMultiplied: item.IsMultiplied, strictAngles: item.StrictAngle);

        src++;
      }

      progressDisplayer.DisplayTransientMessage(string.Empty);
    }

    public void AddToPolygons(NestingContext context, int src, IRawDetail det, int quantity, IProgressDisplayer progressDisplayer, bool isIncluded = true, bool isPriority = false, bool isMultiplied = false, AnglesEnum strictAngles = AnglesEnum.AsPreviewed)
    {
      DetailLoadInfo item = new DetailLoadInfo() { Quantity = quantity, IsIncluded = isIncluded, IsPriority = isPriority, IsMultiplied = isMultiplied, StrictAngle = strictAngles };
      this.AddToPolygons(context, src, det, item, progressDisplayer);
    }

    public void AddToPolygons(NestingContext context, int src, IRawDetail det, DetailLoadInfo item, IProgressDisplayer progressDisplayer)
    {
      INfp loadedNfp;
      if (det.TryConvertToNfp(src, out loadedNfp))
      {
        loadedNfp.IsPriority = item.IsPriority;
        loadedNfp.StrictAngle = item.StrictAngle;
        var quantity = item.Quantity * (item.IsMultiplied ? SvgNest.Config.Multiplier : 1);
        for (int i = 0; i < quantity; i++)
        {
          context.Polygons.Add(loadedNfp.Clone());
        }
      }
      else
      {
        progressDisplayer.DisplayMessageBox($"Failed to import {det.Name}.", "Load Error", MessageBoxIcon.Stop);
      }
    }

    public IRawDetail LoadRawDetail(FileInfo f)
    {
      IRawDetail det = null;
      if (f.Extension == ".svg")
      {
        det = SvgParser.LoadSvg(f.FullName);
      }

      if (f.Extension == ".dxf")
      {
        det = DxfParser.LoadDxfFile(f.FullName).Result;
      }

      return det;
    }
  }
}
