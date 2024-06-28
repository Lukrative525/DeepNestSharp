namespace DeepNestSharp.Domain.ViewModels
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using DeepNestLib;
  using DeepNestLib.Placement;
  using DeepNestSharp.Domain.Models;

  public class ZoomPreviewDrawingContext : ObservableCollection<object>, IZoomPreviewDrawingContext
  {
    public double Width { get; private set; }

    public double Height { get; private set; }

    public static ObservableSheetPlacement Shift(ObservableSheetPlacement sheetPlacement, double offsetX, double offsetY)
    {
      Sheet shiftedSheet = new Sheet(sheetPlacement.Sheet.Clone().ShiftToOrigin(), WithChildren.Included);
      Sheet shiftedOriginalSheet = new Sheet(sheetPlacement.OriginalSheet.Clone().Shift(offsetX, offsetY), WithChildren.Included);

      List<PartPlacement> shiftedPartPlacements = new List<PartPlacement>();
      foreach (ObservablePartPlacement partPlacement in sheetPlacement.PartPlacements)
      {
        INfp shiftedPart = partPlacement.Part.Clone().Shift(partPlacement.X + offsetX, partPlacement.Y + offsetY);
        PartPlacement shiftedPartPlacement = new PartPlacement(shiftedPart);
        shiftedPartPlacements.Add(shiftedPartPlacement);
      }

      SheetPlacement shiftedSheetPlacement = new SheetPlacement(sheetPlacement.PlacementType, shiftedSheet, shiftedOriginalSheet, shiftedPartPlacements, sheetPlacement.MergedLength);

      return new ObservableSheetPlacement(shiftedSheetPlacement);
    }

    public void Set(ISheetPlacement sheetPlacement)
    {
      this.For(sheetPlacement);
    }

    public IZoomPreviewDrawingContext For(ISheetPlacement sheetPlacement)
    {
      return this.For(new ObservableSheetPlacement((SheetPlacement)sheetPlacement));
    }

    /// <inheritdoc />
    public IZoomPreviewDrawingContext For(ObservableSheetPlacement sheetPlacement)
    {
      this.Clear();
      ObservableSheetPlacement shiftedSheetPlacement = Shift(sheetPlacement, -sheetPlacement.MinX, -sheetPlacement.MinY);
      this.Width = shiftedSheetPlacement.Sheet.WidthCalculated;
      this.Height = shiftedSheetPlacement.Sheet.HeightCalculated;
      this.Add(shiftedSheetPlacement);
      foreach (IPartPlacement partPlacement in shiftedSheetPlacement.PartPlacements)
      {
        INfp part = partPlacement.Part;
        this.Add(partPlacement);
        foreach (INfp child in part.Children)
        {
          this.AppendChild(new ObservableHole(child.Shift(partPlacement)));
        }
      }

      return this;
    }

    /// <inheritdoc />
    public IZoomPreviewDrawingContext For(INfp part)
    {
      this.For(new ObservableNfp(part));
      return this;
    }

    /// <inheritdoc />
    public IZoomPreviewDrawingContext For(ObservableNfp part)
    {
      this.Clear();
      this.Width = part.WidthCalculated;
      this.Height = part.HeightCalculated;
      this.Add(part);
      foreach (INfp c in part.Children)
      {
        this.AppendChild(c);
      }

      return this;
    }

    private void AppendChild(INfp c)
    {
      if (c is ObservableHole observableHole)
      {
        this.AppendChild(observableHole);
      }
      else if (c is ObservablePoint observablePoint)
      {
        this.AppendChild(observablePoint);
      }
      else if (c is ObservableFrame observableFrame)
      {
        this.AppendChild(observableFrame);
      }
      else
      {
        this.AppendChild(new ObservableHole(c));
      }
    }

    /// <inheritdoc />
    public void AppendChild(ObservableHole child)
    {
      this.Add(child);
      foreach (INfp c in child.Children)
      {
        this.AppendChild(new ObservableHole(c));
      }
    }

    /// <inheritdoc />
    public void AppendChild(ObservablePoint child)
    {
      this.Add(child);
    }

    /// <inheritdoc />
    public void AppendChild(ObservableFrame child)
    {
      this.Add(child);
    }
  }
}