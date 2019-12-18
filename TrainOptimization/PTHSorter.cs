using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

class PTHSorter : IComparer<IIndividual<Gene>>
{
    public int Compare(IIndividual<Gene> x, IIndividual<Gene> y)
    {
        int lvRes = 0;

        if (x.RefFitnessValue < y.RefFitnessValue)
        {
            lvRes = -1;
        }
        else if(x.RefFitnessValue > y.RefFitnessValue)
        {
            lvRes = 1;
        }

        return lvRes;
    }
}
