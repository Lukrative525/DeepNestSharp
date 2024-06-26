﻿namespace DeepNestLib
{
  using DeepNestLib.Placement;
  using System.Collections.Generic;
  using System.Text.Json;

  public class WindowUnk : IWindowUnk
  {
    public WindowUnk()
    {
      this.db = new DbCache(this);
    }

    public Dictionary<string, List<INfp>> nfpCache { get; } = new Dictionary<string, List<INfp>>();

    private IDbCache db { get; }

    public INfp[] Find(DbCacheKey obj, bool inner = false)
    {
      return this.db.Find(obj, inner);
    }

    public bool Has(DbCacheKey dbCacheKey)
    {
      return this.db.Has(dbCacheKey);
    }

    public void Insert(DbCacheKey obj, bool inner = false)
    {
      this.db.Insert(obj, inner);
    }

    public string ToJson(bool writeIndented = false)
    {
      JsonSerializerOptions options = new JsonSerializerOptions();
      options.Converters.Add(new SheetJsonConverter());
      options.Converters.Add(new NfpJsonConverter());
      options.WriteIndented = writeIndented;
      return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    internal static WindowUnk FromJson(string json)
    {
      JsonSerializerOptions options = new JsonSerializerOptions();
      options.Converters.Add(new WindowUnkJsonConverter());
      options.Converters.Add(new SheetJsonConverter());
      options.Converters.Add(new NfpJsonConverter());
      return System.Text.Json.JsonSerializer.Deserialize<WindowUnk>(json, options);
    }
  }
}
