using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry
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
                Vec2 multiplier = Parent.Direction.GetMultiplier();
                Vec2 pos = value;
                if(multiplier.Y == -1)
                {
                    pos = new Vec2(pos.Y, pos.X * multiplier.X);
                }
                else
                {
                    pos = new Vec2(pos.X * multiplier.X, pos.Y);
                }

                startPoint = pos + Parent.StartPoint;

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
            LocalPoint = localPoint;
        }
    }
}
