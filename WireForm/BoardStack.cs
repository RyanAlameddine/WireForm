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
        public BoardState CurrentState { get; private set; }

        private readonly ISaver saver;
        string locationIdentifier = "";

        public BoardStack(ISaver saver)
        {
            this.saver = saver;
            CurrentState = new BoardState();
            currentNode = new BoardStackNode(null, null, CurrentState.Copy(), "Created Board");
        }

        /// <summary>
        /// Registers a change to the current board state.
        /// </summary>
        public void RegisterChange(string message)
        {
            Debug.WriteLine(message);
            currentNode.Next = new BoardStackNode(null, currentNode, CurrentState.Copy(), message);
            currentNode = currentNode.Next;
            CurrentState.Propogate();
        }

        /// <summary>
        /// Advance forward in the board stack
        /// </summary>
        public void Advance()
        {
            if(currentNode.Next != null)
            {
                currentNode = currentNode.Next;
                CurrentState = currentNode.State.Copy();
                CurrentState.Propogate();
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
                CurrentState = currentNode.State.Copy();
                CurrentState.Propogate();
            }
        }

        public void Load()
        {
            string json = saver.GetJson();
            if (json.Length == 0) return;
            SaveManager.Load(json, out var newState);
            CurrentState = newState;
            currentNode = new BoardStackNode(null, null, CurrentState.Copy(), "Created Board");
            CurrentState.Propogate();
        }

        public void Clear()
        {
            currentNode = new BoardStackNode(null, null, new BoardState(), "Created Board");
            CurrentState = currentNode.State;
            locationIdentifier = "";
        }

        public void Save()
        {
            locationIdentifier = saver.WriteJson(SaveManager.Serialize(CurrentState), locationIdentifier);
            if (locationIdentifier.Length == 0) return;
        }

        public void SaveAs()
        {
            locationIdentifier = saver.WriteJson(SaveManager.Serialize(CurrentState), "");
            if (locationIdentifier.Length == 0) return;
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