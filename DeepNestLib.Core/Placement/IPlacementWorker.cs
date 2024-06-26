﻿namespace DeepNestLib
{
  using System.Collections.Generic;
  using DeepNestLib.Placement;

  public interface IPlacementWorker
  {
    SheetPlacement AddPlacement(INfp processingPart, List<IPartPlacement> placements, INfp part, PartPlacement position, PlacementTypeEnum placementType, ISheet sheet, ISheet originalSheet, double mergedLength);

    void VerboseLog(string message);
  }
}