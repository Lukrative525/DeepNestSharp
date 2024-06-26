﻿namespace DeepNestLib
{
  using System.Collections.Generic;

  public class Simplify
  {
    // to suit your point format, run search/replace for '.x' and '.y';
    // for 3D version, see 3d branch (configurability would draw significant performance overhead)

    // square distance between 2 points
    public static double GetSqDist(SvgPoint p1, SvgPoint p2)
    {
      var dx = p1.X - p2.X;
      var dy = p1.Y - p2.Y;

      return (dx * dx) + (dy * dy);
    }

    // square distance from a point to a segment
    public static double GetSqSegDist(SvgPoint p, SvgPoint p1, SvgPoint p2)
    {
      var x = p1.X;
      var y = p1.Y;
      var dx = p2.X - x;
      var dy = p2.Y - y;

      if (dx != 0 || dy != 0)
      {
        var t = (((p.X - x) * dx) + ((p.Y - y) * dy)) / ((dx * dx) + (dy * dy));

        if (t > 1)
        {
          x = p2.X;
          y = p2.Y;
        }
        else if (t > 0)
        {
          x += dx * t;
          y += dy * t;
        }
      }

      dx = p.X - x;
      dy = p.Y - y;

      return (dx * dx) + (dy * dy);
    }

    // rest of the code doesn't care about point format

    // basic distance-based simplification
    private static SvgPoint[] SimplifyRadialDist(SvgPoint[] points, double? sqTolerance)
    {
      SvgPoint prevPoint = points[0];
      NoFitPolygon newPoints = new NoFitPolygon();
      newPoints.AddPoint(prevPoint);

      SvgPoint point = null;
      int i = 1;
      for (var len = points.Length; i < len; i++)
      {
        point = points[i];

        if (point.Marked || GetSqDist(point, prevPoint) > sqTolerance)
        {
          newPoints.AddPoint(point);
          prevPoint = point;
        }
      }

      if (prevPoint != point)
      {
        newPoints.AddPoint(point);
      }

      return newPoints.Points;
    }

    public static void SimplifyDPStep(SvgPoint[] points, int first, int last, double? sqTolerance, ref NoFitPolygon simplified)
    {
      var maxSqDist = sqTolerance;
      var index = -1;
      var marked = false;
      for (var i = first + 1; i < last; i++)
      {
        var sqDist = GetSqSegDist(points[i], points[first], points[last]);

        if (sqDist > maxSqDist)
        {
          index = i;
          maxSqDist = sqDist;
        }

        /*if(points[i].marked && maxSqDist <= sqTolerance){
            index = i;
            marked = true;
        }*/
      }

      /*if(!points[index] && maxSqDist > sqTolerance){
          console.log('shit shit shit');
      }*/

      if (maxSqDist > sqTolerance || marked)
      {
        if (index - first > 1)
        {
          SimplifyDPStep(points, first, index, sqTolerance, ref simplified);
        }

        ((IHiddenNfp)simplified).Push(points[index]);
        if (last - index > 1)
        {
          SimplifyDPStep(points, index, last, sqTolerance, ref simplified);
        }
      }
    }

    /// <summary>
    /// Simplification using Ramer-Douglas-Peucker algorithm, reducing points by removing those that are within tolerance of a straight line between the points either side of that removed.
    /// </summary>
    /// <param name="points">Original polygon points.</param>
    /// <param name="sqTolerance">Epsilon parmeter; square distance tolerance.</param>
    /// <returns>Simplified clone.</returns>
    public static SvgPoint[] SimplifyDouglasPeucker(SvgPoint[] points, double? sqTolerance)
    {
      var last = points.Length - 1;

      NoFitPolygon simplified = new NoFitPolygon();
      simplified.AddPoint(points[0]);
      SimplifyDPStep(points, 0, last, sqTolerance, ref simplified);
      ((IHiddenNfp)simplified).Push(points[last]);

      return simplified.Points;
    }

    /// <summary>
    /// both algorithms combined for awesome performance
    /// </summary>
    /// <param name="points"></param>
    /// <param name="tolerance"></param>
    /// <param name="doSimplifyRadialDist">If .f then the simplifyRadialDist algorithym is skipped.</param>
    /// <param name="doSimplifyDouglasPeucker">If .f then the simplifyDouglasPeucker algorithym is skipped.</param>
    /// <returns></returns>
    public static NoFitPolygon SimplifyPolygon(IEnumerable<SvgPoint> points, double? tolerance, bool doSimplifyRadialDist, bool doSimplifyDouglasPeucker)
    {
      SvgPoint[] resultSource = points.DeepClone();
      if (resultSource.Length > 2)
      {
        var sqTolerance = (tolerance != null) ? (tolerance * tolerance) : 1;

        if (doSimplifyRadialDist)
        {
          resultSource = SimplifyRadialDist(resultSource, sqTolerance);
        }

        if (doSimplifyDouglasPeucker)
        {
          resultSource = SimplifyDouglasPeucker(resultSource, sqTolerance);
        }
      }

      NoFitPolygon result = new NoFitPolygon();
      foreach (SvgPoint point in resultSource)
      {
        result.AddPoint(point.Clone());
      }

      return result;
    }
  }
}
