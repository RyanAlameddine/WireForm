using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;
using Wireform.MathUtils.Collision;
using WireformInput.Utils;

namespace WireformInput.States.Text
{
    /// <summary>
    /// The tool state for the text tool. Clicking will create a textbox or edit an existing one.
    /// </summary>
    public class TextToolState : InputState
    {
        /// <summary>
        /// The current held/edited circuitLabel
        /// </summary>
        CircuitLabel circuitLabel;

        /// <summary>
        /// Whether or not the mouse is held down and the label is being dragged around
        /// </summary>
        bool draggingLabel = false;

        public override bool IsClean() => true;

        public override async Task Draw(BoardState currentState, PainterScope painter)
        {
            if(circuitLabel != null)
            {
                BoxCollider selectionBox = circuitLabel.HitBox;
                await painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);
                if (draggingLabel)
                {
                    await circuitLabel.Draw(painter, currentState);
                }
            }
        }

        public override InputReturns MouseLeftDown(StateControls stateControls)
        {
            RemoveBar();
            DeleteIfEmpty(stateControls);
            circuitLabel = default;

            //If mouse clicked a CircuitLabel
            if (new BoxCollider(stateControls.LocalMousePosition.X, stateControls.LocalMousePosition.Y, 0, 0).GetIntersections(stateControls.State, (false, false, true), out _, out var boardObjects, false))
            {
                var label = (CircuitLabel) boardObjects.Where((x) => x is CircuitLabel).FirstOrDefault();
                if (label != default)
                {
                    stateControls.State.Extras.Remove(label);
                    draggingLabel = true;
                    circuitLabel = label;
                    circuitLabel.Text += '|';
                }
            }
            else
            {
                circuitLabel = new CircuitLabel(stateControls.LocalMousePosition);
                draggingLabel = true;
            }

            return (true, this);
        }

        /// <summary>
        /// Deletes the current circuitLabel if the text is empty
        /// </summary>
        private void DeleteIfEmpty(StateControls stateControls)
        {
            if(circuitLabel != null && circuitLabel.Text == "") 
            { 
                circuitLabel.Delete(stateControls.State);
                stateControls.RegisterChange("Deleted unused circuitLabel");
            }
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            if (draggingLabel)
            {
                circuitLabel.StartPoint = stateControls.LocalMousePosition;
                return (true, this);
            }
            return (false, this);
        }

        public override InputReturns MouseLeftUp(StateControls stateControls)
        {
            if (draggingLabel)
            {
                //Create and add a circuitLabel
                stateControls.State.Extras.Add(circuitLabel);
                stateControls.RegisterChange("Circuit label created/moved");
            }

            draggingLabel = false;
            return (true, this);
        }

        public override InputReturns KeyDown(StateControls stateControls)
        {
            //No label selected
            if (circuitLabel == null) return (false, this);

            //Add the text to the selected circuitLabel
            RemoveBar();
            switch (stateControls.PressedKey.Value)
            {
                //backspace
                case (char) 8:
                    if(circuitLabel.Text.Length > 0)
                        circuitLabel.Text = circuitLabel.Text.Substring(0, circuitLabel.Text.Length - 1) + "|";
                    break;
                //enter
                //case '\n':
                //    circuitLabel
                default:
                    circuitLabel.Text += stateControls.PressedKey + "|";
                    break;
            }

            return (true, this);
        }

        public override InputReturns CleanupState(StateControls stateControls)
        {
            RemoveBar();
            DeleteIfEmpty(stateControls);
            return (true, this);
        }

        /// <summary>
        /// Removes the typing bar (|) from the end of the text if applicable
        /// </summary>
        void RemoveBar()
        {
            if (circuitLabel != null && circuitLabel.Text != "")
            {
                circuitLabel.Text = circuitLabel.Text.Substring(0, circuitLabel.Text.Length - 1);
            }
        }
    }
}
