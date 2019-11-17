using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm
{
    public partial class Form1 : Form
    {
        Painter painter = new Painter();
        InputHandler inputHandler = new InputHandler();
        StateStack stateStack = new StateStack();

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolBox.SelectedIndex = 0;
            gateBox.DataSource = Enum.GetValues(GateEnum.GatesEnum);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseDown(stateStack, (Vec2) e.Location, this, e.Button, GateMenu, ModifierKeys.HasFlag(Keys.Shift), null);

            if (toRefresh)
            {
                SettingsUpdate();
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            inputHandler.MouseUp(stateStack, e.Button);

            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseMove((Vec2) e.Location, stateStack.CurrentState);

            if(toRefresh) Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            GraphicsManager.Paint(e.Graphics, painter, new Vec2(Width, Height), inputHandler.intersectionBoxes, inputHandler.selections, inputHandler.mouseBox, inputHandler.resetBoxes, stateStack.CurrentState);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
            {
                stateStack.Save("lines.json");
            }
            if(e.KeyChar == 'l')
            {
                stateStack.Load("lines.json");
            }
            if(e.KeyChar == '+' || e.KeyChar == '=')
            {
                GraphicsManager.SizeScale *= 1.1f;
            }
            if(e.KeyChar == '-')
            {
                GraphicsManager.SizeScale *= .9f;
            }
            if(e.KeyChar == 'p')
            {
                Queue<Gate> sources = new Queue<Gate>();
                foreach (Gate gate in stateStack.CurrentState.gates)
                {
                    if (gate.Inputs.Length == 0)
                    {
                        sources.Enqueue(gate);
                    }
                }
                FlowPropagator.Propogate(stateStack.CurrentState, sources);
            }
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(inputHandler.KeyDown(stateStack, e, this))
            {
                Refresh();
            }
        }


        HashSet<CircuitObject> oldSelections;
        List<CircuitProp> circuitProperties;
        private void SettingsUpdate()
        {
            if (!inputHandler.selections.Equals(oldSelections))
            {
                oldSelections = new HashSet<CircuitObject>(inputHandler.selections);
                CircuitObjSettingsBox.Items.Clear();
                
                foreach(CircuitObject obj in oldSelections)
                {
                    circuitProperties = CircuitPropertyAttribute.GetProperties(obj, stateStack, this);
                    foreach (var property in circuitProperties)
                    {
                        CircuitObjSettingsBox.Items.Add(property.Name);
                    }
                }
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta/40;
            GraphicsManager.SizeScale += delta;
            if(GraphicsManager.SizeScale > 70)
            {
                GraphicsManager.SizeScale = 70;
            }
            Refresh();
        }

        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputHandler.tool = (Tool) toolBox.SelectedIndex;
            gateBox.Visible                = inputHandler.tool == Tool.GateController;
            CircuitObjSettingsBox.Visible  = inputHandler.tool == Tool.GateController;
            CircuitObjSettingsText.Visible = inputHandler.tool == Tool.GateController;
            gatePicBox.Visible             = inputHandler.tool == Tool.GateController;

            inputHandler.selections.Clear();
            Refresh();
        }

        private void GateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gatePicBox.Refresh();
        }

        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            Gate newGate = GateEnum.NewGate(gateBox.SelectedIndex, new Vec2(4, 2.5f));
            var temp = GraphicsManager.SizeScale;
            GraphicsManager.SizeScale = 15;
            newGate.Draw(e.Graphics);

            GraphicsManager.SizeScale = temp;
        }

        private void GatePicBox_MouseClick(object sender, MouseEventArgs e)
        {
            //Enum.TryParse<Gates>(gateBox.SelectedValue.ToString(), out var gate);
            inputHandler.MouseDown(stateStack, (Vec2)e.Location, this, e.Button, GateMenu, false, gateBox.SelectedIndex);
            Refresh();
        }

        private void CircuitObjSettingsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CircuitObjSettingsBox.SelectedIndex == -1) { return; }

            CircuitObjSettingsText.Text = circuitProperties[CircuitObjSettingsBox.SelectedIndex].Get().ToString();
        }

        private void CircuitObjSettingsText_Validating(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("Validating");
        }

        private void CircuitObjSettingsText_Validated(object sender, EventArgs e)
        {
            Debug.WriteLine("Validated");
            circuitProperties[CircuitObjSettingsBox.SelectedIndex].Set(CircuitObjSettingsText.Text);
        }
    }
}
