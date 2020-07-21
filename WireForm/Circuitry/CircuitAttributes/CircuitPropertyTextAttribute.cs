using System;
using System.Collections.Generic;
using System.Reflection;
using Wireform.Circuitry.CircuitAttributes.Utils.StringValidators;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Properties are properties which can be altered to change information
    /// about a certain object, often found in a properties menu on the side of the screen.
    /// This attribute should be placed on a property which has a getter and a setter and is an string.
    /// If you would like an attribute which works with text instead of a dropdown, see <see cref="CircuitPropertyDropdownAttribute"/>
    /// </summary>
    public sealed class CircuitPropertyTextAttribute : CircuitPropertyBase
    {
        readonly IStringValidator validator;

        /// <param name="RequireReconnect">true if the Circuit Property requires the gate to be reconnected (connections reset) on edit</param>
        /// <param name="ValidationType">An <see cref="IStringValidator"/> type that will handle validation of string input, see <see cref="StringValidators"/></param>
        public CircuitPropertyTextAttribute(bool RequireReconnect, Type ValidationType) : base(RequireReconnect)
        {
            validator = (IStringValidator) Activator.CreateInstance(ValidationType);
        }

        /// <param name="ValidationType">An <see cref="IStringValidator"/> type that will handle validation of string input, see <see cref="StringValidators"/></param>
        public CircuitPropertyTextAttribute(Type ValidationType) : base(false)
        {
            validator = (IStringValidator) Activator.CreateInstance(ValidationType);
        }

        public override CircuitProp ToProp(PropertyInfo property, BoardObject target)
        {
            return new CircuitProp(
                () => (string) property.GetValue(target),
                (value, connections) => 
                    {
                        if(validator.Validate(value.ToString())) property.SetValue(target, value);
                    },
                false, target, (0,-1), Array.Empty<string>(), RequireReconnect, property.Name);

        }
    }
}