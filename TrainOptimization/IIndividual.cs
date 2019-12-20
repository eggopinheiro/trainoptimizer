using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for IIndivudual
/// </summary>
public interface IIndividual<T> : IComparable<IIndividual<T>>, IEnumerable<TrainMovement>
{
    int Count { get; }
    double Fitness { get; set; }
    double RefFitnessValue { get; set; }
    double GBest { get; set; }
    T this[int index] { get; set; }
    bool GenerateIndividual(IEnumerable<T> pPlanList, int pUniqueId, bool pAllowDeadLockIndividual = false, bool pSortedByTime = false);
    double GetFitness();
    bool IsValid();
    int GetUniqueId();
    int GetDistanceFrom(IIndividual<T> pIndividual);
    IEnumerable<Gene> MoveTrain(T pTrainMov, out Gene[] pUsedHeadway, ref bool pHasStopped, DateTime pInitialTime = default(DateTime), bool pUpdate = true, DateTime pForcedDepTime = default(DateTime), bool pNoStopBeforeSwitch = false);
    IEnumerable<T> GetElements(int pStartIndex, int pEndIndex);
    void AddElements(IEnumerable<T> pElemetns, bool pNeedUpdate = true);
    void AddElementRef(T pElement);
    bool hasArrived(Int64 pTrainId);
    void Clear();
    bool Save();
    IIndividual<T> Clone();
    void LoadDistRef();
    void Serialize();
    void UnSerialize();
}