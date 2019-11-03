using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.Circuitry.Gates.Utilities;

namespace WireForm
{
    public sealed class StateStack
    {
        private StateStackNode currentNode;
        private BoardState currentState;
        public BoardState CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public StateStack()
        {
            currentState = new BoardState();
            currentNode = new StateStackNode(null, null, currentState.Copy(), "Created Board");
        }

        /// <summary>
        /// Registers a change to the current board state.
        /// </summary>
        public void RegisterChange(BoardState state, string message)
        {
            Debug.WriteLine(message);
            currentNode.Next = new StateStackNode(null, currentNode, currentState.Copy(), message);
            currentNode = currentNode.Next;
            Propogate();
        }

        /// <summary>
        /// Advance forward in the state stack
        /// </summary>
        public void Advance()
        {
            if(currentNode.Next != null)
            {
                currentNode = currentNode.Next;
                currentState = currentNode.State.Copy();
                Propogate();
            }
        }

        /// <summary>
        /// Move backwards through the state stack
        /// </summary>
        public void Reverse()
        {
            if (currentNode.Previous != null)
            {
                currentNode = currentNode.Previous;
                currentState = currentNode.State.Copy();
                Propogate();
            }
        }

        public void Load(string v)
        {
            SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), v)), out currentState);
            currentNode = new StateStackNode(null, null, currentState.Copy(), "Created Board");
        }

        public void Save(string v)
        {
            SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), v), currentState);
        }

        public void Propogate()
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach (Gate gate in CurrentState.gates)
            {
                if (gate.Inputs.Length == 0)
                {
                    sources.Enqueue(gate);
                }
            }
            FlowPropagator.Propogate(currentState, sources);
        }

        private class StateStackNode
        {
            public StateStackNode Next;
            public StateStackNode Previous;

            public BoardState State;
            public string Message;

            public StateStackNode(StateStackNode next, StateStackNode previous, BoardState state, string message)
            {
                Next = next;
                Previous = previous;

                State = state;
                Message = message;
            }
        }
    }
}