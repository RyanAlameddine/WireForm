using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.Circuitry.CircuitObjectOperations
{
    public interface IDeletable
    {
        void Delete(BoardState propogator);
    }
}
