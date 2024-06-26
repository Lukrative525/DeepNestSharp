﻿namespace DeepNestLib.Placement
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.IO;
  using System.Linq;
  using System.Text.Json.Serialization;
  using ClipperLib;
  using DeepNestLib;
  using DeepNestLib.Geometry;
  using Minkowski;

  public class MinkowskiSum : IMinkowskiSumService
  {
    private static volatile object minkowskiSyncLock = new object();

    [JsonConstructor]
    public MinkowskiSum()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinkowskiSum"/> class.
    /// Private because sharing/reusing the cache is dangerous.
    /// Replacing static global dependencies with factories to facilitate Unit Tests.
    /// </summary>
    private MinkowskiSum(bool useMinkowskiCache, INestStateMinkowski state)
    {
      this.UseMinkowskiCache = useMinkowskiCache;
      this.State = state;
    }

    [JsonInclude]
    public MinkowskiDictionary MinkowskiCache { get; private set; } = new MinkowskiDictionary();

    public Action<string> VerboseLogAction { private get; set; }

    public INestStateMinkowski State { private get; set; }

    [JsonIgnore]
    internal bool UseMinkowskiCache { private get; set; }

    /// <summary>
    /// Create a new instance with a self contained cache.
    /// </summary>
    /// <param name="config">Singleton config for the nest.</param>
    /// <param name="nestState">Shared NestState (instead of NestState.Default).</param>
    /// <returns><see cref="IMinkowskiSumService"/>.</returns>
    public static IMinkowskiSumService CreateInstance(ISvgNestConfig config, INestStateMinkowski nestState) => new MinkowskiSum(config.UseMinkowskiCache, nestState);

    /// <summary>
    /// Create a new instance with a self contained cache.
    /// </summary>
    /// <param name="useMinkowskiCache">A value indicating whether to cache the results.</param>
    /// <param name="nestState">Shared NestState (instead of NestState.Default).</param>
    /// <returns><see cref="IMinkowskiSumService"/>.</returns>
    public static IMinkowskiSumService CreateInstance(bool useMinkowskiCache, INestStateMinkowski nestState) => new MinkowskiSum(useMinkowskiCache, nestState);

    INfp[] IMinkowskiSumService.DllImportExecute(INfp path, INfp pattern, MinkowskiSumCleaning minkowskiSumCleaning)
    {
      NoFitPolygon b = new NoFitPolygon(pattern, WithChildren.Included);
      Dictionary<string, List<PointF>> dic1 = new Dictionary<string, List<PointF>>();
      Dictionary<string, List<double>> dic2 = new Dictionary<string, List<double>>();
      dic2.Add("A", new List<double>());
      foreach (SvgPoint item in path.Points)
      {
        List<double> target = dic2["A"];
        target.Add(item.X);
        target.Add(item.Y);
      }

      dic2.Add("B", new List<double>());
      foreach (SvgPoint item in b.Points)
      {
        List<double> target = dic2["B"];
        target.Add(item.X);
        target.Add(item.Y);
      }

      List<double> hdat = new List<double>();

      foreach (INfp item in path.Children)
      {
        foreach (SvgPoint pitem in item.Points)
        {
          hdat.Add(pitem.X);
          hdat.Add(pitem.Y);
        }
      }

      List<double> aa = dic2["A"];
      List<double> bb = dic2["B"];
      var arr1 = path.Children.Select(z => z.Points.Count() * 2).ToArray();

      MinkowskiKey key = new MinkowskiKey(aa.Count, aa, path.Children.Count, arr1, hdat, bb.Count, bb);
      INfp ret;
      lock (minkowskiSyncLock)
      {
        INfp cacheRetrieval;
        if (!this.MinkowskiCache.TryGetValue(key, out cacheRetrieval))
        {
          this.VerboseLogAction?.Invoke($"{path.ToShortString()}-{b.ToShortString()} {key} not found in {nameof(MinkowskiSum)}.{nameof(this.MinkowskiCache)} so calculating...");
#if x64
          // System.Diagnostics.Debug.Print($"{state.CallCounter}.Minkowski_x64");
          long[] longs = arr1.Select(o => (long)o).ToArray();
          MinkowskiWrapper.setData(key.ALength, key.APoints, key.AChildrenLength, longs, key.Hdat, key.BLength, key.BPoints);
#else
          // System.Diagnostics.Debug.Print($"{callCounter}.Minkowski_x86/AnyCpu");
          MinkowskiWrapper.setData(key.ALength, key.APoints, key.AChildrenLength, arr1, key.Hdat, key.BLength, key.BPoints);
#endif
          MinkowskiWrapper.calculateNFP();

          this.State.IncrementDllCallCounter();

          int[] sizes;
          int[] sizes1;
          int[] sizes2;
          double[] dat1;
          double[] hdat1;
          sizes = new int[2];
          MinkowskiWrapper.getSizes1(sizes);
          sizes1 = new int[sizes[0]];
          sizes2 = new int[sizes[1]];
          MinkowskiWrapper.getSizes2(sizes1, sizes2);
          dat1 = new double[sizes1.Sum()];
          hdat1 = new double[sizes2.Sum()];

          MinkowskiWrapper.getResults(dat1, hdat1);

#if DEBUG
          if (sizes1.Count() > 1)
          {
            throw new ArgumentException("sizes1 cnt >1");
          }
#endif

          // convert back to answer here
          List<PointF> apts = new List<PointF>();

          List<List<double>> holesval = new List<List<double>>();

          for (var i = 0; i < dat1.Length; i += 2)
          {
            var x1 = (float)dat1[i];
            var y1 = (float)dat1[i + 1];
            apts.Add(new PointF(x1, y1));
          }

          var index = 0;
          for (var i = 0; i < sizes2.Length; i++)
          {
            holesval.Add(new List<double>());
            for (var j = 0; j < sizes2[i]; j++)
            {
              holesval.Last().Add(hdat1[index]);
              index++;
            }
          }

          List<List<PointF>> holesout = new List<List<PointF>>();
          foreach (List<double> item in holesval)
          {
            holesout.Add(new List<PointF>());
            for (var i = 0; i < item.Count; i += 2)
            {
              var x = (float)item[i];
              var y = (float)item[i + 1];
              holesout.Last().Add(new PointF(x, y));
            }
          }

          ret = new NoFitPolygon();
          foreach (PointF item in apts)
          {
            ret.AddPoint(new SvgPoint(item.X, item.Y));
          }

          foreach (List<PointF> item in holesout)
          {
            ret.Children.Add(new NoFitPolygon());
            foreach (PointF hitem in item)
            {
              ret.Children.Last().AddPoint(new SvgPoint(hitem.X, hitem.Y));
            }
          }

          if (cacheRetrieval == null)
          {
            if (this.MinkowskiCache.Values.Any(o => o.Equals(ret)))
            {
              if (ret.Points.Length == 0)
              {
                System.Diagnostics.Debugger.Break();
                if (SvgNest.Config.ExportExecutions)
                {
                  KeyValuePair<MinkowskiKey, INfp> matchKvp = this.MinkowskiCache.ToList().First(o => o.Value.Equals(ret));
                  File.WriteAllText(@"C:\Temp\MinkowskiSum\MatchKey.json", matchKvp.Key.ToJson());
                  File.WriteAllText(@"C:\Temp\MinkowskiSum\MatchValue.json", matchKvp.Value.ToJson());
                  var nameSuffix = "Sum2";
                  File.WriteAllText($"C:\\Temp\\MinkowskiSum\\Minkowski{nameSuffix}.dnpoly", this.MinkowskiCache.ToJson());
                  File.WriteAllText($"C:\\Temp\\MinkowskiSum\\Minkowski{nameSuffix}A.dnpoly", path.ToJson());
                  File.WriteAllText($"C:\\Temp\\MinkowskiSum\\Minkowski{nameSuffix}B.dnpoly", b.ToJson());
                  File.WriteAllText($"C:\\Temp\\MinkowskiSum\\Minkowski{nameSuffix}Ret.dnpoly", ret.ToJson());
                }
              }
            }

            if (this.UseMinkowskiCache)
            {
              this.VerboseLogAction?.Invoke($"Add {path.ToShortString()}-{b.ToShortString()} {key} to {nameof(MinkowskiSum)}.{nameof(this.MinkowskiCache)}...");
              this.MinkowskiCache.Add(key, ret);
            }
          }
          else if (!ret.Equals(cacheRetrieval))
          {
            System.Diagnostics.Debug.Print("Ret already exists in cache but key didn't find it!");
          }
        }
        else
        {
          this.VerboseLogAction?.Invoke($"{path.ToShortString()}-{b.ToShortString()} {key} found in {nameof(MinkowskiSum)}.{nameof(this.MinkowskiCache)}...");
          ret = new NoFitPolygon(cacheRetrieval, WithChildren.Included);
        }
      }

      if (minkowskiSumCleaning == MinkowskiSumCleaning.Cleaned)
      {
        this.VerboseLogAction?.Invoke("Clean MinkowskiSum...");
        ret.Clean();
      }

      return new INfp[] { ret };
    }

    NoFitPolygon IMinkowskiSumService.ClipperExecuteOuterNfp(SvgPoint[] pattern, SvgPoint[] path, MinkowskiSumPick minkowskiSumPick)
    {
      NoFitPolygon result = this.ClipperExecuteOuterNfp(pattern, path, minkowskiSumPick);
      result.EnsureIsClosed();
      return result;
    }

    private NoFitPolygon ClipperExecuteOuterNfp(SvgPoint[] pattern, SvgPoint[] path, MinkowskiSumPick minkowskiSumPick)
    {
      var scaler = 10000000;
      List<IntPoint> patternClipper = DeepNestClipper.ScaleUpPath(pattern, scaler);
      List<IntPoint> pathClipper = DeepNestClipper.ScaleUpPath(path, -scaler);

      List<List<IntPoint>> solution = Clipper.MinkowskiSum(patternClipper, pathClipper, true);
      NoFitPolygon clipperNfp = null;

      double? largestArea = null;
      for (var i = 0; i < solution.Count(); i++)
      {
        NoFitPolygon n = solution[i].ToArray().ToNestCoordinates(scaler);
        var sarea = -GeometryUtil.PolygonArea(n);
        if (largestArea == null ||
            minkowskiSumPick == MinkowskiSumPick.Largest && largestArea < sarea ||
            minkowskiSumPick == MinkowskiSumPick.Smallest && sarea > 0 && largestArea > sarea)
        {
          clipperNfp = n;
          largestArea = sarea;
        }
      }

      for (var i = 0; i < clipperNfp.Length; i++)
      {
        clipperNfp[i].X += path[0].X;
        clipperNfp[i].Y += path[0].Y;
      }

      //clipperNfp.EnsureIsClosed();
      return clipperNfp;
    }

    /// <summary>
    /// Uses Clipper to calculate the InnerNfp of pattern inside part.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="path"></param>
    /// <param name="withChildren"></param>
    /// <param name="takeOnlyBiggestArea"></param>
    /// <returns></returns>
    INfp[] IMinkowskiSumService.NewMinkowskiSum(IList<SvgPoint> pattern, INfp path, WithChildren withChildren, bool takeOnlyBiggestArea)
    {
      this.State.IncrementClipperCallCounter();
      var scaler = 10000000;
      List<IntPoint> patternScaledUp = DeepNestClipper.ScaleUpPath(pattern, scaler);
      List<List<IntPoint>> solution;
      if (withChildren == WithChildren.Included)
      {
        List<List<IntPoint>> pathScaledUpList = NfpHelper.NfpToClipperCoordinates(path, -scaler);
        // var options = new System.Text.Json.JsonSerializerOptions();
        // options.IncludeFields = true;
        // var json = System.Text.Json.JsonSerializer.Serialize(patternScaledUp, options);
        // File.WriteAllText(@"C:\Temp\patternScaledUp.json", json);
        // json = System.Text.Json.JsonSerializer.Serialize(pathScaledUpList, options);
        // File.WriteAllText(@"C:\Temp\pathScaledUpList.json", json);

        solution = Clipper.MinkowskiSum(patternScaledUp, pathScaledUpList, false);
      }
      else
      {
        throw new NotImplementedException("Fel88 added this but it was uncalled so I havn't validated it's substitutable.");
        solution = Clipper.MinkowskiSum(patternScaledUp, DeepNestClipper.ScaleUpPath(path.Points, -scaler), true);
      }

      NoFitPolygon clipperNfp = null;

      double? largestArea = null;
      var largestIndex = -1;

      for (var i = 0; i < solution.Count(); i++)
      {
        NoFitPolygon n = solution[i].ToArray().ToNestCoordinates(scaler);
        var sarea = Math.Abs(GeometryUtil.PolygonArea(n));
        if (largestArea == null || largestArea < sarea)
        {
          clipperNfp = n;
          largestArea = sarea;
          largestIndex = i;
        }
      }

      if (!takeOnlyBiggestArea)
      {
        for (var j = 0; j < solution.Count; j++)
        {
          if (j == largestIndex)
          {
            continue;
          }

          NoFitPolygon n = solution[j].ToArray().ToNestCoordinates(scaler);
          clipperNfp.Children.Add(n);
        }
      }

      for (var i = 0; i < clipperNfp.Length; i++)
      {
        clipperNfp[i].X *= -1;
        clipperNfp[i].Y *= -1;
        clipperNfp[i].X += pattern.First().X;
        clipperNfp[i].Y += pattern.First().Y;
      }

      if (clipperNfp.Children != null)
      {
        foreach (INfp nFP in clipperNfp.Children)
        {
          for (var j = 0; j < nFP.Length; j++)
          {
            nFP.Points[j].X *= -1;
            nFP.Points[j].Y *= -1;
            nFP.Points[j].X += pattern.First().X;
            nFP.Points[j].Y += pattern.First().Y;
          }
        }
      }

      clipperNfp.EnsureIsClosed();
      NoFitPolygon[] res = new[] { clipperNfp };
      return res;
    }
  }

}
