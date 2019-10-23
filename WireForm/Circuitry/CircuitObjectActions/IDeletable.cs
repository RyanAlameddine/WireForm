using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.Circuitry.CircuitObjectActions
{
    public interface IDeletable
    {
        [CircuitAction("Delete", true)]
        void Delete(BoardState state);
    }
}
