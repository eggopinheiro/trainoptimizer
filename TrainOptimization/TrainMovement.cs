using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class TrainMovement : IEquatable<TrainMovement>, IEnumerable<Gene> 
{
    private int mId = 0;
    private List<Gene> mGenes;

    public TrainMovement()
    {
        mGenes = new List<Gene>();
    }

    public void Add(IEnumerable<Gene> pGenes, int pIndex = -1)
    {
        if (pIndex == -1)
        {
            mGenes.AddRange(pGenes);
        }
        else
        {
            mGenes.InsertRange(pIndex, pGenes);
        }
    }

    public void Add(Gene pGene, int pIndex = -1)
    {
        if (pIndex == -1)
        {
            mGenes.Add(pGene);
        }
        else if (pIndex >= 0)
        {
            mGenes.Insert(pIndex, pGene);
        }
    }

    public void Remove(Gene pGene)
    {
        mGenes.Remove(pGene);
    }

    public void Remove(int pIndex)
    {
        if ((pIndex < mGenes.Count) && (pIndex > -1))
        {
            mGenes.RemoveAt(pIndex);
        }
    }

    public Gene Last
    {
        get
        {
            if (mGenes.Count > 0)
            {
                return mGenes[mGenes.Count-1];
            }
            else
            {
                return null;
            }
        }
    }

    public Gene this[int i]
    {
        get
        {
            if (i < mGenes.Count)
            {
                return mGenes[i];
            }
            else
            {
                return null;
            }
        }

        set
        {
            if ((i < mGenes.Count) && (i >= 0))
            {
                mGenes[i] = value;
            }
        }
    }

    public int Count
    {
        get
        {
            return mGenes.Count;
        }
    }

    public IEnumerator<Gene> GetEnumerator()
    {
        return mGenes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public static bool operator ==(TrainMovement obj1, TrainMovement obj2)
    {
        bool lvRes = false;
        Gene lvGeneObj1 = null;
        Gene lvGeneObj2 = null;

        bool lvNoRef1 = ReferenceEquals(obj1, null);
        bool lvNoRef2 = ReferenceEquals(obj2, null);

        if (lvNoRef1 && lvNoRef2)
        {
            return true;
        }

        if (lvNoRef1)
        {
            return false;
        }

        if (lvNoRef2)
        {
            return false;
        }

        if (obj1.Count == obj2.Count)
        {
            lvRes = true;
            for (int i = 0; i < obj1.Count; i++)
            {
                lvGeneObj1 = obj1[i];
                lvGeneObj2 = obj2[i];

                if ((lvGeneObj1.TrainId != lvGeneObj2.TrainId) || (lvGeneObj1.StopLocation.Location != lvGeneObj2.StopLocation.Location) || (lvGeneObj1.State != lvGeneObj2.State))
                {
                    lvRes = false;
                    break;
                }
            }
        }

        return lvRes;
    }

    public static bool operator !=(TrainMovement obj1, TrainMovement obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(TrainMovement other)
    {
        bool lvRes = false;

        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if(this == other)
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;
        TrainMovement lvTrainMovement = obj as TrainMovement;

        if (lvTrainMovement != null)
        {
            lvRes = Equals(lvTrainMovement);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        if (mId == 0)
        {
            LoadId();
        }

        return mId;
    }

    public void LoadId()
    {
        Gene lvGene = null;

        if (mGenes.Count > 0)
        {
            lvGene = mGenes[0];

            if (lvGene.StopLocation != null)
            {
                mId = new { lvGene.TrainId, lvGene.StopLocation.Location, lvGene.State }.GetHashCode();
            }
            else
            {
                mId = new { lvGene.TrainId, lvGene.State }.GetHashCode();
            }
        }
    }

    public int GetID()
    {
        if (mId == 0)
        {
            LoadId();
        }

        return mId;
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (Gene lvGene in mGenes)
        {
            if (lvRes.Length > 0)
            {
                lvRes.Append(" => ");
            }
            lvRes.Append(lvGene.TrainId);
            lvRes.Append(", Nome: ");
            lvRes.Append(lvGene.TrainName);
            lvRes.Append(", Data: ");
            lvRes.Append(lvGene.Time);
            if (lvGene.StopLocation != null)
            {
                lvRes.Append(", Stop Location: ");
                lvRes.Append(lvGene.StopLocation.Location);
            }
            else
            {
                lvRes.Append(", Local: ");
                lvRes.Append(lvGene.SegmentInstance.Location);
                lvRes.Append(".");
                lvRes.Append(lvGene.SegmentInstance.SegmentValue);
            }
            lvRes.Append(", Status: ");
            if (lvGene.State == Gene.STATE.OUT)
            {
                lvRes.Append(" Saida");
            }
            else if (lvGene.State == Gene.STATE.IN)
            {
                lvRes.Append(" Chegada");
            }
            else
            {
                lvRes.Append(" Indefinido");
            }
            lvRes.Append(", Track: ");
            lvRes.Append(lvGene.Track);
            lvRes.Append(", Destino: ");
            lvRes.Append(lvGene.End);
            lvRes.Append(", Headway: ");
            lvRes.Append(lvGene.HeadWayTime);
        }

        return lvRes.ToString();
    }
}
