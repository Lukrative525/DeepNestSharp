﻿namespace DeepNestLib.Placement
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  public class MinkowskiDictionaryJsonConverter : JsonConverter<MinkowskiDictionary>
  {
    public override bool CanConvert(Type typeToConvert)
    {
      return typeToConvert == typeof(MinkowskiDictionary);
    }

    public override MinkowskiDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      List<KeyValuePair<MinkowskiKey, INfp>> kvpList = JsonSerializer.Deserialize<List<KeyValuePair<MinkowskiKey, INfp>>>(ref reader, options);
      var json = JsonSerializer.Serialize(kvpList, options);
      MinkowskiDictionary result = new MinkowskiDictionary();
      foreach (KeyValuePair<MinkowskiKey, INfp> kvp in kvpList)
      {
        // System.Diagnostics.Debug.Print(string.Join(",", kvp.Key.Item7));
        result.Add(kvp.Key, kvp.Value, false);
      }

      return result;
    }

    public override void Write(Utf8JsonWriter writer, MinkowskiDictionary dictionary, JsonSerializerOptions options)
    {
      List<KeyValuePair<MinkowskiKey, INfp>> kvpList = dictionary.ToList();
      JsonSerializer.Serialize(writer, kvpList, options);
    }
  }
}
