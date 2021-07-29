using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wireform.MathUtils;

namespace Wireform.Circuitry.Utils
{
    /// <summary>
    /// Loads all Gate classes and allows you to create instances of said classes
    /// </summary>
    public static class GateCollection
    {
        private static SortedDictionary<string, Func<Vec2, Gate>> constructors;
        /// <summary>
        /// Gate path category path followed by name separated by '/', eg. "Utils/Splitter"
        /// </summary>
        public static IEnumerable<string> GatePaths
        {
            get => constructors.Keys;
        }

        static GateCollection()
        {
            LoadConstructors();
        }
        
        /// <summary>
        /// Returns a new gate from the GateConstructors dictionary at the desired path
        /// </summary>
        public static Gate CreateGate(string path, Vec2 position)
        {
            return constructors[path](position);
        }

        /// <summary>
        /// Loads all Gate types from the executing assembly
        /// </summary>
        private static void LoadConstructors()
        {
            constructors = new SortedDictionary<string, Func<Vec2, Gate>>();
            //Gets all types which extend Gate and are not abstract
            var gateTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where((x) => x.IsSubclassOf(typeof(Gate)) && !x.IsAbstract);
            foreach(var type in gateTypes)
            {
                //Check for [Gate]
                GateAttribute attribute = type.GetCustomAttribute<GateAttribute>(false);
                if (attribute == null) continue;

                //type name only if gateName is ""
                var name = attribute.gateName.Length != 0 ? attribute.gateName : type.Name;
                var fullPath = attribute.path + name;

                try
                {
                    Gate constructor(Vec2 position) => (Gate)Activator.CreateInstance(type, position, default);
                    constructors.Add(fullPath, constructor);
                } catch (MissingMethodException)
                {
                    throw new MissingMethodException("All Gates must have a constructor which takes in only Vec2 (for position), and Direction");
                }
            }
        }
    }
}
