using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;

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

        /// <summary>
        /// if saveToNew is "", pressing the save button will be treated as SaveAs
        /// </summary>
        private string savePath = "";

        public BoardStack()
        {
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

        public void Load(string path)
        {
            //openFileDialog.Filter = "Json|*.json";
            //openFileDialog.Title  = "Load your wireForm";
            //openFileDialog.ShowDialog();

            //var fileName = openFileDialog.FileName;
            if (path == "")
            {
                return;
            }


            SaveManager.Load(File.ReadAllText(path), out currentState);
            currentNode = new BoardStackNode(null, null, currentState.Copy(), "Created Board");
            savePath = path;
            Propogate();
        }

        public void Clear()
        {
            currentNode = new BoardStackNode(null, null, new BoardState(), "Created Board");
            currentState = currentNode.State;
            savePath = "";
        }

        /// <summary>
        /// IMPORTANT: Returns false if auto-saving failed. If false, please run SaveAs with path
        /// </summary>
        public bool Save()
        {
            if (savePath == "")
            {
                return false;
            }

            SaveAs(savePath);
            return true;
        }

        public void SaveAs(string path)
        {
            //saveFileDialog.Filter = "Json|*.json";
            //saveFileDialog.Title = "Save your wireForm";
            //saveFileDialog.ShowDialog();
            //var fileName = saveFileDialog.FileName;
            //if (fileName == "")
            //{
            //    return;
            //}
            savePath = path;

            SaveManager.Save(path, currentState);
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