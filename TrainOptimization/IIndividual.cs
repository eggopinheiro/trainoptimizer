using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for IIndivudual
/// </summary>
public interface IIndividual<T> : IComparable<IIndividual<T>>
{
    int Count { get; }
    double Fitness { get; set; }
    double RefFitnessValue { get; set; }
    double GBest { get; set; }
    T this[int index] { get; set; }
    bool GenerateIndividual(List<T> pPlanList, int pUniqueId, bool pAllowDeadLockIndividual);
    double GetFitness();
    bool IsValid();
    int GetUniqueId();
    int GetDistanceFrom(IIndividual<T> pIndividual);
    bool ProcessGene(T pGene, DateTime pInitialTime = default(DateTime), bool pUpdate = true);
    List<T> GetGenes(int pStartIndex, int pEndIndex);
    List<T> GetGenes();
    void AddGenes(List<T> pGenes, bool pNeedUpdate = true);
    void AddGeneRef(T pGene);
    void Clear();
    bool Save();
    IIndividual<Gene> Clone();
    void LoadDistRef();
    void Serialize();
    void UnSerialize();
}