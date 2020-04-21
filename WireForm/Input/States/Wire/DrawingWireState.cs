﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;

namespace Wireform.Input.States.Wire
{
    /// <summary>
    /// State for drawing new wires holding the left click button
    /// </summary>
    class DrawingWireState : InputState
    {
        /// <summary>
        /// The line from the start point to the branch point (if the wire is not being drawn straight along one axis)
        /// </summary>
        private readonly WireLine primaryLine;

        /// <summary>
        /// The line from the branch point to the end point
        /// </summary>
        private readonly WireLine secondaryLine;

        /// <summary>
        /// true if the primary wire is being drawn horizontally
        /// </summary>
        private bool isHorizontal;

        public DrawingWireState(Vec2 griddedMousePosition)
        {
            primaryLine   = new WireLine(griddedMousePosition, griddedMousePosition, true);
            secondaryLine = new WireLine(griddedMousePosition, griddedMousePosition, true);
        }

        public override void Draw(BoardState currentState, PainterScope painter)
        {
            WirePainter.DrawWireLine(painter, currentState, primaryLine  );
            WirePainter.DrawWireLine(painter, currentState, secondaryLine);
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            Vec2 endPosition = stateControls.GriddedMousePosition;

            //If the target position has not changed return
            if (secondaryLine.EndPoint == endPosition) return (false, this);

            //The position where the primary and secondary wires split
            Vec2 branchPosition;
            if (isHorizontal) branchPosition = new Vec2(endPosition.X, primaryLine.StartPoint.Y);
            else              branchPosition = new Vec2(primaryLine.StartPoint.X, endPosition.Y);

            primaryLine  .EndPoint   = branchPosition;
            secondaryLine.StartPoint = branchPosition;
            secondaryLine.EndPoint   = endPosition;

            primaryLine.IsHorizontal = isHorizontal;
            secondaryLine.IsHorizontal = !isHorizontal;

            //If the primary line has lost relevance, the assumed direction should be changed
            if (primaryLine.StartPoint == primaryLine.EndPoint) isHorizontal = !isHorizontal;

            return (true, this);
        }

        public override InputReturns MouseLeftUp(StateControls stateControls)
        {
            primaryLine  .InsertAndAttach(stateControls.State.wires, stateControls.State.Connections);
            secondaryLine.InsertAndAttach(stateControls.State.wires, stateControls.State.Connections);

            stateControls.RegisterChange($"Created wire from {primaryLine.StartPoint}-{secondaryLine.EndPoint}");
            return (true, new WireToolState());
        }
    }
}
