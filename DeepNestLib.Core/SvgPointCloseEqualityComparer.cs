﻿namespace DeepNestLib
{
  using System;
  using System.Collections.Generic;

  public class SvgPointCloseEqualityComparer : IEqualityComparer<SvgPoint>
  {
    public bool Equals(SvgPoint x, SvgPoint y)
    {
      double precision = 0.0001;
      if (this.CloseEqual(x.X, y.X, precision) &&
          this.CloseEqual(x.Y, y.Y, precision) &&
          x.Exact == y.Exact &&
          x.Marked == y.Marked)
      {
        return true;
      }

      return false;
    }

    private bool CloseEqual(double x, double y, double precision)
    {
      return Math.Abs(x - y) <= precision;
    }

    public int GetHashCode(SvgPoint obj)
    {
      throw new NotImplementedException();
    }
  }
}