using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Circuitry.Utils
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GateAttribute : Attribute
    {
        public readonly string path = "";
        public readonly string gateName = "";

        /// <summary>
        /// Attribute which marks the Gate as one which should be loaded into the creation menu.
        /// This overload will set the gateName to the name of the type, and the path to the root
        /// </summary>
        public GateAttribute()
        {

        }

        /// <summary>
        /// Attribute which marks the Gate as one which should be loaded into the creation menu. 
        /// This overload will set the gateName to the name of the type.
        /// </summary>
        /// <param name="categoryPath">
        /// The category path separated by / characters.
        /// Eg. Logic/Extra/
        /// </param>
        public GateAttribute(string categoryPath)
        {
            this.path = categoryPath;
            if (categoryPath.Length == 0) return;
            if (path[path.Length - 1] != '/') path += "/";
            if (path[0] == '/') path = path.Substring(1, path.Length - 1);
        }

        /// <summary>
        /// Attribute which marks the Gate as one which should be loaded into the creation menu
        /// </summary>
        /// <param name="categoryPath">
        /// The category path separated by / characters.
        /// Eg. Logic/Extra/
        /// </param>
        /// <param name="gateName">The Gate's name (if it is different from the name of the type itself)</param>
        public GateAttribute(string categoryPath, string gateName) : this(categoryPath)
        {
            this.gateName = gateName;
        }
    }
}
