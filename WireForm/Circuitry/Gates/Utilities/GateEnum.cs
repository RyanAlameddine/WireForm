using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.Circuitry.Gates.Utilities
{
    public static class GateEnum
    {
        static Type gatesEnum;
        public static Type GatesEnum
        {
            get
            {
                if (gatesEnum == null)
                {
                    gatesEnum = CreateEnum();
                }
                return gatesEnum;
            }
        }

        public static Type[] allGates;
        public static Type[] AllGates
        {
            get
            {
                if (allGates == null)
                {
                    allGates = Assembly.GetExecutingAssembly().GetTypes().Where((x) => x.IsSubclassOf(typeof(Gate))).ToArray();
                }
                return allGates;
            }
        }

        public static Type CreateEnum()
        {
            EnumBuilder eb = GetEnumBuilder();
            for (int i = 0; i < AllGates.Length; i++)
            {
                eb.DefineLiteral(AllGates[i].Name, i);
            }

            Type objectType = eb.CreateType();
            return objectType;
        }

        private static EnumBuilder GetEnumBuilder()
        {
            var typeSignature = "GatesEnumGenerated";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            EnumBuilder eb = moduleBuilder.DefineEnum(typeSignature,
                    TypeAttributes.Public | TypeAttributes.AnsiClass,
                    typeof(int));
            return eb;
        }

        public static Gate NewGate(int gateIndex, Vec2 Position)
        {
            var values = Enum.GetValues(GatesEnum);

            int i = 0;
            string selectedValue = "";
            foreach (var value in values)
            {
                if (i == gateIndex)
                {
                    selectedValue = value.ToString();
                    break;
                }
                i++;
            }

            var gateType = AllGates.First((x) => x.Name == selectedValue);

            var gate = (Gate)Activator.CreateInstance(gateType, Position);

            return gate;
        }
    }
}
