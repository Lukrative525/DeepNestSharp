﻿namespace DeepNestSharp.Domain.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using DeepNestLib;
  using DeepNestLib.GeneticAlgorithm;
  using DeepNestLib.NestProject;
  using DeepNestLib.Placement;
  using IxMilia.Dxf;

  public class ObservableNfp : ObservablePropertyObject, INfp
  {
    private readonly INfp item;

    public ObservableNfp(INfp nfp)
    {
      this.item = nfp;
    }

    /// <inheritdoc cref="INfp" />
    public INfp SourceItem => this.item;

    /// <inheritdoc/>
    public override bool IsDirty => true;

    [Description("The gross outer area, not accounting for any holes.")]
    [Category("Dimensions")]
    /// <inheritdoc/>
    public double Area => this.item.Area;

    /// <inheritdoc/>
    [Description("The children (aka holes) nested inside the part.")]
    [Category("Definition")]
    public IList<INfp> Children
    {
      get => this.item.Children;
      set => this.SetProperty(nameof(this.Children), () => this.item.Children, v => this.item.Children = v, value);
    }

    /// <inheritdoc/>
    public bool Fitted => this.item.Fitted;

    [Description("The overall height of the part.")]
    [Category("Dimensions")]
    /// <inheritdoc/>
    public double HeightCalculated => this.item.HeightCalculated;

    /// <inheritdoc/>
    [Description("The Id of the part.")]
    [Category("Description")]
    public int Id
    {
      get => this.item.Id;
      set => this.SetProperty(nameof(this.Id), () => this.item.Id, v => this.item.Id = v, value);
    }

    /// <inheritdoc/>
    public bool IsClosed => this.item.IsClosed;

    /// <inheritdoc/>
    public bool IsExact => !this.item.Points.Any(o => !o.Exact);

    /// <inheritdoc/>
    [Browsable(false)]
    public bool IsPriority
    {
      get => this.item.IsPriority;
      set => this.SetProperty(nameof(this.IsPriority), () => this.item.IsPriority, v => this.item.IsPriority = v, value);
    }

    /// <inheritdoc/>
    [Browsable(false)]
    public int Length => this.item.Length;

    /// <inheritdoc/>
    [Description("The MaxX of part's points.")]
    [Category("Placement")]
    public double MaxX => this.item.MaxX;

    /// <inheritdoc/>
    [Description("The MaxY of part's points.")]
    [Category("Placement")]
    public double MaxY => this.item.MaxY;

    /// <inheritdoc/>
    [Description("The MinX of part's points.")]
    [Category("Placement")]
    public double MinX => this.item.MinX;

    /// <inheritdoc/>
    [Description("The MinY of part's points.")]
    [Category("Placement")]
    public double MinY => this.item.MinY;

    /// <inheritdoc/>
    [Description("The name of file loaded as the part.")]
    [Category("Description")]
    public string Name
    {
      get => this.item.Name;
      set => this.SetProperty(nameof(this.Name), () => this.item.Name, v => this.item.Name = v, value);
    }

    public double NetArea => this.item.NetArea;

    /// <inheritdoc/>
    [Description("The X offset (Set and used by the export process).")]
    [Category("Placement")]
    public double? OffsetX
    {
      get => this.item.OffsetX;
      set => this.SetProperty(nameof(this.OffsetX), () => this.item.OffsetX, v => this.item.OffsetX = v, value);
    }

    /// <inheritdoc/>
    [Description("The Y offset (Set and used by the export process).")]
    [Category("Placement")]
    public double? OffsetY
    {
      get => this.item.OffsetY;
      set => this.SetProperty(nameof(this.OffsetY), () => this.item.OffsetY, v => this.item.OffsetY = v, value);
    }

    /// <inheritdoc/>
    public bool Overlaps(INfp other) => this.item.Overlaps(other);

    /// <inheritdoc/>
    [Description("An index noting the order in the placement sequence at which this part got inserted.")]
    [Category("Placement")]
    public int PlacementOrder
    {
      get => this.item.PlacementOrder;
      set => this.SetProperty(nameof(this.PlacementOrder), () => this.item.PlacementOrder, v => this.item.PlacementOrder = v, value);
    }

    /// <inheritdoc/>
    [Description("The points that make up the outer edge of the part.")]
    [Category("Definition")]
    SvgPoint[] IPolygon.Points => this.item.Points;

    /// <inheritdoc/>
    [Description("The degrees of rotation from the original imported part.")]
    [Category("Placement")]
    public double Rotation
    {
      get => this.item.Rotation;
    }

    /// <inheritdoc/>
    public INfp Sheet
    {
      get => this.item.Sheet;
      set => this.SetProperty(nameof(this.Sheet), () => this.item.Sheet, v => this.item.Sheet = v, value);
    }

    /// <inheritdoc/>
    public int Source
    {
      get => this.item.Source;
      set => this.SetProperty(nameof(this.Source), () => this.item.Source, v => this.item.Source = v, value);
    }

    /// <inheritdoc/>
    [Description("Denotes whether any restrictions on angle of placement have been imposed.")]
    [Category("Placement")]
    public AnglesEnum StrictAngle
    {
      get => this.item.StrictAngle;
      set => this.SetProperty(nameof(this.StrictAngle), () => this.item.StrictAngle, v => this.item.StrictAngle = v, value);
    }

    [Description("The overall width of the part.")]
    [Category("Dimensions")]
    /// <inheritdoc/>
    public double WidthCalculated => this.item.WidthCalculated;

    /// <inheritdoc/>
    [Description("The X offset of the part from the origin.")]
    [Category("Placement")]
    public double X
    {
      get => this.item.X;
      set => this.SetProperty(nameof(this.X), () => this.item.X, v => this.item.X = v, value);
    }

    /// <inheritdoc/>
    [Description("The Y offset of the part from the origin.")]
    [Category("Placement")]
    public double Y
    {
      get => this.item.Y;
      set => this.SetProperty(nameof(this.Y), () => this.item.Y, v => this.item.Y = v, value);
    }

    //public MainViewModel MainViewModel { get; }

    /// <inheritdoc/>
    public SvgPoint this[int ind] => this.item[ind];

    /// <inheritdoc/>
    public void AddPoint(SvgPoint point)
    {
      this.item.AddPoint(point);
    }

    /// <inheritdoc/>
    public void Clean()
    {
      this.item.Clean();
    }

    /// <inheritdoc/>
    public INfp Clone()
    {
      return this.item.Clone();
    }

    /// <inheritdoc/>
    public INfp CloneExact()
    {
      return this.item.CloneExact();
    }

    /// <inheritdoc/>
    public INfp CloneTree()
    {
      return this.item.CloneTree();
    }

    /// <inheritdoc/>
    public INfp CloneTop()
    {
      return this.item.CloneTop();
    }

    /// <inheritdoc/>
    public NoFitPolygon GetHull()
    {
      return this.item.GetHull();
    }

    /// <inheritdoc/>
    public void ReplacePoints(IEnumerable<SvgPoint> points)
    {
      this.item.ReplacePoints(points);
    }

    /// <inheritdoc/>
    public void ReplacePoints(INfp replacementNfp)
    {
      this.item.ReplacePoints(replacementNfp);
    }

    /// <inheritdoc/>
    public void Reverse()
    {
      this.item.Reverse();
    }

    /// <inheritdoc/>
    public INfp Rotate(double degrees, WithChildren withChildren)
    {
      return this.item.Rotate(degrees, withChildren);
    }

    /// <inheritdoc/>
    public INfp Slice(int v)
    {
      return this.item.Slice(v);
    }

    /// <inheritdoc/>
    public string Stringify()
    {
      return this.item.Stringify();
    }

    public Chromosome ToChromosome()
    {
      return this.item.ToChromosome();
    }

    public Chromosome ToChromosome(double rotation)
    {
      return this.item.ToChromosome(rotation);
    }

    /// <inheritdoc/>
    public string ToJson()
    {
      return this.item.ToJson();
    }

    /// <inheritdoc/>
    public string ToShortString()
    {
      return this.item.ToShortString();
    }

    /// <inheritdoc/>
    public string ToOpenScadPolygon()
    {
      return this.item.ToOpenScadPolygon();
    }

    public INfp Shift(IPartPlacement shift)
    {
      return this.item.Shift(shift);
    }

    public INfp Shift(double x, double y)
    {
      return this.item.Shift(x, y);
    }

    public INfp ShiftToOrigin()
    {
      return this.item.ShiftToOrigin();
    }

    bool IEquatable<IPolygon>.Equals(IPolygon other)
    {
      return ((IEquatable<IPolygon>)this.item).Equals(other);
    }

    public void EnsureIsClosed()
    {
      this.item.EnsureIsClosed();
    }

    public DxfFile ToDxfFile()
    {
      return this.item.ToDxfFile();
    }
  }
}
