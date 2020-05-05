using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform
{
    public static class GlobalSettings
    {
        /// <summary>
        /// The max amount of propogation calls before the propogation algorithm gives up
        /// and assumes oscillation.
        /// </summary>
        public static int PropogationRepetitionOverflow = 1000;
    }
}
