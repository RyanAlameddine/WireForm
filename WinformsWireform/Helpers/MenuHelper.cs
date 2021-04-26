using System.Windows.Forms;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;
using WireformInput;

namespace WinformsWireform.Helpers
{
    internal static class MenuHelper
    {
        public static void CreateGateMenuFromRoot(ToolStripMenuItem rootItem, InputStateManager manager)
        {
            var gates = GateCollection.GatePaths;

            //Create gate menu heirarchy
            foreach (var gatePath in gates)
            {
                var currentMenuItem = rootItem;
                var path = gatePath.Split('/');
                for (int i = 0; i < path.Length - 1; i++)
                {
                    //Checks if currentMenuItem already has category child which matches path[i]
                    var children = currentMenuItem.DropDownItems;
                    bool found = false;
                    foreach (var child in children) 
                        if (child is ToolStripMenuItem item) 
                            if (item.Text == path[i])
                            {
                                currentMenuItem = item; 
                                found = true; 
                                break;
                            }
                    if (found) continue;

                    var newItem = new ToolStripMenuItem(path[i]);
                    currentMenuItem.DropDownItems.Add(newItem);
                    currentMenuItem = newItem;
                }

                //Add gate to category menu
                var gateItem = new ToolStripMenuItem(path[path.Length - 1], null,
                    (s, e) =>
                    {
                        //Place created gate onto board
                        Gate newGate = GateCollection.CreateGate(gatePath, Vec2.Zero);
                        manager.PlaceNewGate(newGate);
                    });
                currentMenuItem.DropDownItems.Add(gateItem);
            }
        }
    }
}
