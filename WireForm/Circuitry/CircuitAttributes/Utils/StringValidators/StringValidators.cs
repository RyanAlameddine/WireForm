using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Circuitry.CircuitAttributes.Utils.StringValidators
{
    public interface IStringValidator
    {
        public bool Validate(string str);
    }

    public static class StringValidators
    {
        /// <summary>
        /// Valid if the input string is not null or empty
        /// </summary>
        public class NotNullOrEmpty : IStringValidator
        {
            public bool Validate(string str) => !string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// Valid if the input string can be parsed to an int
        /// </summary>
        public class IsInt : IStringValidator
        {
            public bool Validate(string str) => int.TryParse(str, out _);
        }
        /// <summary>
        /// Valid if the input string can be parsed to a float
        /// </summary>
        public class IsFloat : IStringValidator
        {
            public bool Validate(string str) => float.TryParse(str, out _);
        }

        /// <summary>
        /// Valid if the input string is not null
        /// </summary>
        public class NotNull : IStringValidator
        {
            public bool Validate(string str) => str != null;
        }
    }
}
