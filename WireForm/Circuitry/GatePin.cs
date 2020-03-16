using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry
{
    public class GatePin : BoardObject
    {
        [JsonIgnore]
        Vec2 startPoint;
        [JsonIgnore]
        public override Vec2 StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                startPoint = value;

                if (Parent == null)
                {
                    //Debug.WriteLine("Null parent in gatepin initialization - If this occurs while loading, this is not an error");
                    return;
                }

                localPoint = startPoint - Parent.StartPoint;
            }
        }
        Vec2 localPoint;
        public Vec2 LocalPoint
        {
            get
            {
                return localPoint;
            }
            set
            {
                localPoint = value;

                if (Parent == null)
                {
                    //Debug.WriteLine("Null parent in gatepin initialization - If this occurs while loading, this is not an error");
                    return;
                }

                startPoint = MathHelper.Plus(value, Parent.StartPoint);

            }
        }
        public BitArray Values { get; set; }
        public Gate Parent { get; set; }

        public GatePin(Gate Parent, Vec2 LocalStart)
        {
            this.Parent = Parent;
            LocalPoint = LocalStart;

            Values = new BitArray(1);
        }
        /// <summary>
        /// Refreshes the StartPoint and LocalPoint variables to ensure that they are synced
        /// </summary>
        public void RefreshLocation()
        {
            StartPoint = localPoint + Parent.StartPoint;
        }
    }
}
