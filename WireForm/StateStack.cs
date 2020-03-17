using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;

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

        /// <summary>
        /// if saveToNew is "", pressing the save button will be treated as SaveAs
        /// </summary>
        private string savePath = "";

        public StateStack()
        {
            currentState = new BoardState();
            currentNode = new StateStackNode(null, null, currentState.Copy(), "Created Board");
        }

        /// <summary>
        /// Registers a change to the current board state.
        /// </summary>
        public void RegisterChange(string message)
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

        public void Load(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "Json|*.json";
            openFileDialog.Title  = "Load your wireForm";
            openFileDialog.ShowDialog();

            var fileName = openFileDialog.FileName;
            if(fileName == "")
            {
                return;
            }


            SaveManager.Load(File.ReadAllText(fileName), out currentState);
            currentNode = new StateStackNode(null, null, currentState.Copy(), "Created Board");
            savePath = fileName;
            Propogate();
        }

        public void Clear()
        {
            currentNode = new StateStackNode(null, null, new BoardState(), "Created Board");
            currentState = currentNode.State;
            savePath = "";
        }

        public void Save(SaveFileDialog saveFileDialog)
        {
            if (savePath == "")
            {
                SaveAs(saveFileDialog);
                if (savePath == "") return;
            }

            SaveManager.Save(savePath, currentState);
        }

        public void SaveAs(SaveFileDialog saveFileDialog)
        {
            saveFileDialog.Filter = "Json|*.json";
            saveFileDialog.Title = "Save your wireForm";
            saveFileDialog.ShowDialog();
            var fileName = saveFileDialog.FileName;
            if (fileName == "")
            {
                return;
            }
            savePath = fileName;

            Save(saveFileDialog);
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