﻿namespace DeepNestLib.GeneticAlgorithm
{
  using GeneticSharp.Domain.Chromosomes;
  using GeneticSharp.Domain.Randomizations;

  public class NestChromosome : ChromosomeBase
  {
    public NestChromosome(int numberOfParts)
      : base(numberOfParts)
    => this.CreateGenes();

    public override IChromosome CreateNew()
    {
      return new NestChromosome(this.Length);
    }

    public override Gene GenerateGene(int geneIndex)
    {
      return new Gene(RandomizationProvider.Current.GetInt(0, this.Length * 50));
    }
  }
}
