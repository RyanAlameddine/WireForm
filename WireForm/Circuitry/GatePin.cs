using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry
{
    public class GatePin : DrawableObject
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
                var (xMult, yMult, flipXY) = Parent.Direction.GetMultiplier();
                Vec2 pos = value;
                float x = pos.X * xMult;
                float y = pos.Y * yMult;
                if (flipXY)
                    pos = new Vec2(y, x);
                else
                    pos = new Vec2(x, y);

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

        public override async Task Draw(PainterScope scope, BoardState state)
        {
            await scope.FillEllipseC(Values.BitColors()[0], LocalPoint, new Vec2(.4f, .4f));
        }
    }
}
