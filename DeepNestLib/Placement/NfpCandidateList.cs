﻿namespace DeepNestLib
{
  using System.IO;
  using System.Text.Json;
  using System.Text.Json.Serialization;
#if NCRUNCH
  using System.Text;
#endif

  public class NfpCandidateList : INfpCandidateList
  {
    public const string FileDialogFilter = "DeepNest Nfp Candidates (*.dnnfps)|*.dnnfps|All files (*.*)|*.*";

    [JsonConstructor]
    public NfpCandidateList()
    {
    }

    public NfpCandidateList(INfp[] items, ISheet sheet, INfp part)
    {
      Items = items;
      Sheet = new Sheet(sheet, WithChildren.Included);
      Part = new NFP(part, WithChildren.Included);
    }

    // inner NFP
    public NfpCandidateList(INfpHelper nfpHelper, ISheet sheet, INfp part, double clipperScale)
    {
      Items = nfpHelper.GetInnerNfp(sheet, part, MinkowskiCache.Cache, clipperScale); // ?? new INfp[0];
      Sheet = sheet;
      Part = part;
    }

    [JsonInclude]
    public ISheet Sheet { get; private set; }

    [JsonInclude]
    public INfp Part { get; private set; }

    [JsonIgnore]
    public int NumberOfNfps => Items.Length;

    [JsonInclude]
    public INfp[] Items { get; private set; }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    public INfp this[int index] { get => Items[index]; }

    public static NfpCandidateList FromJson(string json)
    {
      var options = new JsonSerializerOptions();
      options.Converters.Add(new SheetJsonConverter());
      options.Converters.Add(new NfpJsonConverter());
      return JsonSerializer.Deserialize<NfpCandidateList>(json, options);
    }

    public static INfpCandidateList LoadFromFile(string filePath)
    {
      using (StreamReader inputFile = new StreamReader(filePath))
      {
        return FromJson(inputFile.ReadToEnd());
      }
    }

    public string ToJson()
    {
      var options = new JsonSerializerOptions();
      options.Converters.Add(new SheetJsonConverter());
      options.Converters.Add(new NfpJsonConverter());
      return JsonSerializer.Serialize(this, options);
    }
  }
}