using System;
using System.Collections;
using System.Collections.Generic;
using Wireform.Circuitry.Data;
using Wireform.Utils;

namespace Wireform.Circuitry.CircuitAttributes.Utils
{
    /// <summary>
    /// A collection which is able to process lists of [CircuitAction]
    /// </summary>
    public class CircuitActionCollection : IReadOnlyCollection<CircuitAct>
    {
        private readonly HashSet<CircuitAct> actions;
        private Action<string> registerChange;

        public static CircuitActionCollection Empty { get => new CircuitActionCollection(new HashSet<CircuitAct>(), null); }

        internal CircuitActionCollection(HashSet<CircuitAct> actions, Action<string> registerChange)
        {
            this.actions = actions;
            this.registerChange = registerChange;
        }

        /// <summary>
        /// Invokes hotkey on all circuitActions applicible
        /// </summary>
        /// <returns>true if the drawing panel should be refreshed</returns>
        public bool InvokeHotkeyActions(BoardState currentState, char hotkey, Modifier modifiers)
        {
            if (actions == null) return false;
            bool toRefresh = false;
            //Execute matches
            foreach (var action in actions)
            {
                if (action.Hotkey == hotkey && action.Modifiers == modifiers)
                {
                    toRefresh = true;
                    action.Invoke(currentState);
                }
            }

            string hotkeyStr = hotkey.GetHotkeyString(modifiers);

            if (toRefresh) registerChange($"Executed hotkey {hotkeyStr} on selection(s)");
            return toRefresh;
        }

        /// <summary>
        /// Invokes the given action
        /// </summary>
        /// <returns>true if the drawing panel should be refreshed</returns>
        public bool InvokeAction(BoardState currentState, CircuitAct act)
        {
            act.Invoke(currentState);
            registerChange($"Executed action {act.Name} on selection(s)");
            return true;
        }

        /// <summary>
        /// Invokes action at the given index and calls the refresh function if neccessary
        /// </summary>
        public void InvokeActionAndRefresh(BoardState currentState, Action refresh, CircuitAct act)
        {
            act.Invoke(currentState);
            registerChange($"Executed action {act.Name} on selection(s)");
            refresh();
        }

        /// <summary>
        /// Combines two CircuitActionCollections into the current collection.
        /// </summary>
        public void UnionWith(CircuitActionCollection other)
        {
            if (other == null) return;
            //Adds circuit actions
            actions.UnionWith(other);

            //Chooses the defined registerChange function
            registerChange = registerChange == default ? other.registerChange : registerChange;
        }

        //Generated Code

        public int Count => actions.Count;

        public IEnumerator<CircuitAct> GetEnumerator()
        {
            return ((IReadOnlyCollection<CircuitAct>)actions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyCollection<CircuitAct>)actions).GetEnumerator();
        }
    }
}
