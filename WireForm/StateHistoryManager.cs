using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;

namespace WireForm
{
    public sealed class StateHistoryManager
    {
        public StateHistoryManager()
        {
            currentState = new BoardState();
        }

        private BoardState currentState;
        public BoardState CurrentState {
            get
            {
                return currentState;
            }
        }
        private Stack<BoardState> states = new Stack<BoardState>();

        public void Load(string v)
        {
            SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), v)), out currentState);
        }
    }
}
