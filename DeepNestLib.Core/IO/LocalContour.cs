﻿namespace DeepNestLib.IO
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;

  public class LocalContour<TSourceEntity> : ILocalContour
  {
    public LocalContour(List<PointF> points, HashSet<TSourceEntity> entities)
    {
      this.Entities = entities;
      this.Points = points;
    }

    public double Len
    {
      get
      {
        double len = 0;
        for (var i = 1; i <= this.Points.Count; i++)
        {
          PointF p1 = this.Points[i - 1];
          PointF p2 = this.Points[i % this.Points.Count];
          len += Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        return len;
      }
    }

    public List<PointF> Points { get; } = new List<PointF>();

    public HashSet<TSourceEntity> Entities { get; } = new HashSet<TSourceEntity>();

    public bool IsChild { get; internal set; } = true;
  }
}
