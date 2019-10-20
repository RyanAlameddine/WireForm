using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;

namespace WireForm
{
    public class StateHistoryManager
    {
        public Stack<BoardState> states = new Stack<BoardState>();

        public StateHistoryManager(BoardState startingState)
        {
            states.Push(startingState.Copy());
        }
    }
}
