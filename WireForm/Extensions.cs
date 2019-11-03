using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitObjectActions;
using WireForm.MathUtils;

namespace WireForm
{
    public static class Extensions
    {
        public static void AddConnection(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject circuitObject)
        {
            if (!connections.ContainsKey(circuitObject.StartPoint))
            {
                connections.Add(circuitObject.StartPoint, new List<BoardObject>());
            }
            connections[circuitObject.StartPoint].Add(circuitObject);
        }

        public static void RemoveConnection(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject circuitObject)
        {
            connections[circuitObject.StartPoint].Remove(circuitObject);
        }

        public static void AddRange<T>(this HashSet<T> set1, HashSet<T> set2)
        {
            foreach(var t in set2)
            {
                set1.Add(t);
            }
        }

        public static void RegisterActions(this ContextMenuStrip gateMenu, List<(CircuitActionAttribute attribute, EventHandler action)> actions, Form form)
        {
            gateMenu.Items.Clear();
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                if (action.attribute.RequireRefresh)
                {
                    action.action += (object sender, EventArgs e) =>
                    {
                        //RefreshSelections(state);
                        form.Refresh();
                    };
                }
                gateMenu.Items.Add(action.attribute.Name, null, action.action);
            }
        }

        public static Color BitColor(this BitValue value)
        {
            switch (value)
            {
                case BitValue.Error:
                    return Color.DarkRed;
                case BitValue.Nothing:
                    return Color.DimGray;
                case BitValue.One:
                    return Color.Blue;
                case BitValue.Zero:
                    return Color.DarkBlue;
            }
            throw new NullReferenceException();
        }
    }
}
