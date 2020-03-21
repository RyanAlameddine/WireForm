using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Input.States.Selection
{
    /// <summary>
    /// The state where the Selection tool is selected and the program sits idle.
    /// Also controls the right click action which loads CircuitActions.
    /// </summary>
    class SelectionToolState : SelectionStateBase
    {
        public SelectionToolState(List<CircuitObject> selections)
        {
            this.selections = selections;
        }

        public override InputReturns MouseDown(Form1 form, Vec2 mousePosition, MouseButtons mouseButton)
        {
            return mouseButton switch
            {
                MouseButtons.Left  => LeftClick(form, mousePosition),
                MouseButtons.Right => RightClick(form, mousePosition),
                _                  => (false, this),
            };
        }

        private InputReturns LeftClick(Form1 form, Vec2 mousePosition)
        {
            Vec2 mousePointGridded = LocalGridPoint(mousePosition);
            //true if you click a gate
            if (new BoxCollider(mousePointGridded.X, mousePointGridded.Y, 0, 0).GetIntersections(form.stateStack.CurrentState, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = null;
                //Select first selected object
                foreach (var v in circuitObjects)
                {
                    clickedcircuitObject = v;
                }
                return (true, new MovingSelectionState(mousePosition, selections));
            }
            return (true, new SelectingState(mousePosition, selections));
        }


        private InputReturns RightClick(Form1 form, Vec2 mousePosition)
        {
            Vec2 mousePointGridded = LocalGridPoint(mousePosition);
            //true if you click a gate
            if (new BoxCollider(mousePointGridded.X, mousePointGridded.Y, 0, 0).GetIntersections(form.stateStack.CurrentState, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = null;
                //Select first selected object
                foreach (var v in circuitObjects)
                {
                    clickedcircuitObject = v;
                }

                var actions = CircuitActionAttribute.GetActions(clickedcircuitObject, form.stateStack, form.drawingPanel);

                ///Add refreshing to the actions if necessary
                form.GateMenu.Items.Clear();
                for (int i = 0; i < actions.Count; i++)
                {
                    form.GateMenu.Items.Add(actions[i].attribute.Name, null, actions[i].action);
                }

                form.GateMenu.Show(form, (Point)mousePosition);

                return (true, this);
            }
            return (false, this);
        }
    }
}
