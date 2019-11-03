using System;
using System.Collections.Generic;
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
        private BoardState tempState;

        /// <summary>
        /// The temporary state of the board.
        /// These edits will not be saved when an action is registered
        /// </summary>
        public BoardState CurrentStateTemporary
        {
            get
            {
                return tempState;
            }
        }

        public StateStack()
        {
            currentState = new BoardState();
            currentNode = new StateStackNode(null, null, null, null);
        }

        /// <summary>
        /// Registers and executes a change to the current board state.
        /// IMPORTANT: The forward will be executed after registration
        /// </summary>
        /// <param name="advance">An action which enacts the change onto the boardstate</param>
        /// <param name="reverse">An action which undos the change done by the advance</param>
        public void RegisterChange(Action<BoardState> advance, Action<BoardState> reverse)
        {
            currentNode.Next = new StateStackNode(null, currentNode, advance, reverse);
            Advance();
        }

        /// <summary>
        /// Advance forward in the state stack
        /// </summary>
        public void Advance()
        {
            if(currentNode.Next != null)
            {
                currentNode.Next.Advance(currentState);
                currentNode = currentNode.Next;
                tempState = currentState.Copy();
            }
        }

        /// <summary>
        /// Move backwards through the state stack
        /// </summary>
        public void Reverse()
        {
            if (currentNode.Previous != null)
            {
                currentNode.Reverse(currentState);
                currentNode = currentNode.Previous;
                tempState = currentState.Copy();
            }
        }

        public void Load(string v)
        {
            SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), v)), out currentState);
            currentNode = new StateStackNode(null, null, null, null);
            tempState = currentState.Copy();
        }

        public void Save(string v)
        {
            SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), v), currentState);
        }

        private class StateStackNode
        {
            public StateStackNode Next;
            public StateStackNode Previous;

            public Action<BoardState> Advance;
            public Action<BoardState> Reverse;

            public StateStackNode(StateStackNode next, StateStackNode previous, Action<BoardState> advance, Action<BoardState> reverse)
            {
                Next = next;
                Previous = previous;

                Advance = advance;
                Reverse = reverse;
            }
        }
    }
}