using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for IFitness
/// </summary>
public interface IFitness<T>
{
    double GetFitness(IIndividual<T> pIndividual);
    int FitnessCallNum { get; }
    string Type { get; set; }
}