using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;

namespace Wireform
{
    public sealed class BoardStack
    {
        private BoardStackNode currentNode;
        private BoardState currentState;
        public BoardState CurrentState
        {
            get
            {
                return currentState;
            }
        }

        private ISaveable saveable;
        string locationIdentifier = "";

        public BoardStack(ISaveable saveable)
        {
            this.saveable = saveable;
            currentState = new BoardState();
            currentNode = new BoardStackNode(null, null, currentState.Copy(), "Created Board");
        }

        /// <summary>
        /// Registers a change to the current board state.
        /// </summary>
        public void RegisterChange(string message)
        {
            Debug.WriteLine(message);
            currentNode.Next = new BoardStackNode(null, currentNode, currentState.Copy(), message);
            currentNode = currentNode.Next;
            Propogate();
        }

        /// <summary>
        /// Advance forward in the board stack
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
        /// Move backwards through the board stack
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

        public void Load()
        {
            string json = saveable.GetJson(out locationIdentifier);
            if (json == "") return;
            SaveManager.Load(json, out currentState);
            currentNode = new BoardStackNode(null, null, currentState.Copy(), "Created Board");
            Propogate();
        }

        public void Clear()
        {
            currentNode = new BoardStackNode(null, null, new BoardState(), "Created Board");
            currentState = currentNode.State;
            locationIdentifier = "";
        }

        public void Save()
        {
            locationIdentifier = saveable.WriteJson(SaveManager.Serialize(currentState), locationIdentifier);
            if (locationIdentifier == "") return;
        }

        public void SaveAs()
        {
            locationIdentifier = saveable.WriteJson(SaveManager.Serialize(currentState), "");
            if (locationIdentifier == "") return;
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

        private class BoardStackNode
        {
            public BoardStackNode Next;
            public BoardStackNode Previous;

            public BoardState State;
            public string Message;

            public BoardStackNode(BoardStackNode next, BoardStackNode previous, BoardState state, string message)
            {
                Next = next;
                Previous = previous;

                State = state;
                Message = message;
            }
        }
    }
}