﻿namespace DeepNestLib
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
#if NCRUNCH
  using System.Text;
#endif
  using ClipperLib;

  public static class OpenScadExtensions
  {
    public static string ToOpenScadPolygon(this List<List<IntPoint>> clipperSheetNfp)
    {
      return ToOpenScadPolygon(clipperSheetNfp.Select(z => z.ToArray()).ToArray());
    }

    public static string ToOpenScadPolygon(this IntPoint[][] clipperSheetNfp)
    {
      StringBuilder resultBuilder = new StringBuilder();
      for (var polygonIdx = 0; polygonIdx < clipperSheetNfp.Length; polygonIdx++)
      {
        resultBuilder.AppendLine("polygon ([");
        foreach (IntPoint p in clipperSheetNfp[polygonIdx])
        {
          resultBuilder.AppendLine($"[{p.X:0.######},{p.Y:0.######}],");
        }

        resultBuilder.AppendLine("]);");
      }

      return resultBuilder.ToString();
    }

    public static string ToOpenScadPolygon(this IEnumerable<INfp> nfps)
    {
      StringBuilder openScadBuilder = new StringBuilder();
      foreach (INfp item in nfps)
      {
        openScadBuilder.AppendLine(item.ToOpenScadPolygon());
      }

      return openScadBuilder.ToString();
    }
  }
}