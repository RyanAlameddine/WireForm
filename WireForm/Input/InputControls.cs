using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.Input
{
    class InputControls
    {
        public BoardState State { get; }
        public Vec2 MousePosition { get; }
        public Action Refresh { get; }
        public Keys Key { get; }
        public Keys Modifiers { get; }

        public Action<string> RegisterChange { get; }
        public Action Reverse { get; }
        public Action Advance { get; }

        /// <summary>
        /// null if no update is required
        /// </summary>
        public List<(CircuitActionAttribute attribute, EventHandler action)> CircuitActionsOutput { get; set; } = null;

        /// <summary>
        /// null if no update is required
        /// </summary>
        public List<CircuitProp> CircuitPropertiesOutput { get; set; } = null;

        public InputControls(BoardState state, Vec2 mousePosition, Keys key, Keys modifiers, Action refresh, Action<string> registerChange, Action reverse, Action advance)
        {
            this.State = state;
            this.MousePosition = mousePosition;
            this.Key = key;
            this.Modifiers = modifiers;
            this.Refresh = refresh;
            this.RegisterChange = registerChange;
            this.Reverse = reverse;
            this.Advance = advance;
        }
    }
}
