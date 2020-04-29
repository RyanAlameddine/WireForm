using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Hides a circuit property or action
    /// This is useful when, for example, a base class contains a circuit property or action whic you 
    /// do not want to appear in the menus for your derivation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HideCircuitAttributesAttribute : Attribute
    {

    }
}
