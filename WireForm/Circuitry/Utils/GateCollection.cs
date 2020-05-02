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
        private static Dictionary<string, Func<Vec2, Gate>> constructors;
        /// <summary>
        /// Map from string (gate type name eg. "Splitter") to function which takes in a Vec2 (position) and 
        /// returns a new Gate object
        /// </summary>
        public static IReadOnlyDictionary<string, Func<Vec2, Gate>> GateConstructors
        {
            get
            {
                if (constructors == null) LoadConstructors();
                return constructors;
            }
        }

        /// <summary>
        /// Returns a new gate from the GateConstructors dictionary at the desired position
        /// </summary>
        public static Gate CreateGate(string name, Vec2 position)
        {
            return constructors[name](position);
        }

        /// <summary>
        /// Loads all Gate types from the executing assembly
        /// </summary>
        private static void LoadConstructors()
        {
            constructors = new Dictionary<string, Func<Vec2, Gate>>();
            var gateTypes = Assembly.GetExecutingAssembly().GetTypes().Where((x) => x.IsSubclassOf(typeof(Gate)) && !x.IsAbstract);
            foreach(var type in gateTypes)
            {
                var name = type.Name;

                try
                {
                    Gate constructor(Vec2 position) => (Gate)Activator.CreateInstance(type, position, default);
                    constructors.Add(name, constructor);
                } catch (System.MissingMethodException)
                {
                    throw new MissingMethodException("All Gates must have a constructor which takes in only Vec2 (for position), and Direction");
                }
            }
        }
    }
}
