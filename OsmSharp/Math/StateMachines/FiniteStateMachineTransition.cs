// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Math.Automata;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.StateMachines
{
    /// <summary>
    /// Represents a transition in a finite-state machine.
    /// </summary>
    public sealed class FiniteStateMachineTransition<EventType>
    {
        /// <summary>
        /// The source state.
        /// </summary>
        public FiniteStateMachineState<EventType> SourceState { get; private set; }

        /// <summary>
        /// The target state.
        /// </summary>
        public FiniteStateMachineState<EventType> TargetState { get; private set; }

        /// <summary>
        /// The list of events this transition repsonds to.
        /// </summary>
        public List<FiniteStateMachineTransitionCondition<EventType>> TransitionConditions { get; private set; }

        /// <summary>
        /// Boolean indicating not to respond to the listed events.
        /// </summary>
        public bool Inverted { get; private set; }

        /// <summary>
        /// The delegate code.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public delegate void TransitionFinishedDelegate(object message);

        /// <summary>
        /// Checks the actual condition.
        /// </summary>
        public TransitionFinishedDelegate Finished { get; private set; }

        /// <summary>
        /// Returns true if the given event triggers a response in this transition.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal bool Match(FiniteStateMachine<EventType> machine, object message)
        {
            // get the value of the match.
            bool val = false;
            foreach (var condition in this.TransitionConditions)
            {
                if (condition.Check(machine, message))
                {
                    val = true;
                    break;
                }
            }

            // revert the value if needed.
            if (this.Inverted)
            {
                return !val;
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// Notifies this transition that it was successfull.
        /// </summary>
        /// <param name="message"></param>
        internal void NotifySuccessfull(object message)
        {
            if(this.Finished != null)
            {
                this.Finished(message);
            }
        }

        #region Generation

        /// <summary>
        /// Generates a non-inverted transition.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="eventTypes"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, params Type[] eventTypes)
        {
            return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventTypes);
        }

        /// <summary>
        /// Generates a non-inverted transition with an extra check-delegate!
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="eventType"></param>
        /// <param name="checkDelegate"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition<EventType> Generate(
            List<FiniteStateMachineState<EventType>> states, int start, int end, Type eventType,
            OsmSharp.Math.StateMachines.FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate)
        {
            return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventType, checkDelegate, null);
        }

        /// <summary>
        /// Generates a non-inverted transition with an extra check-delegate!
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="eventType"></param>
        /// <param name="checkDelegate"></param>
        /// <param name="finishedDelegate"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition<EventType> Generate(
            List<FiniteStateMachineState<EventType>> states, int start, int end, Type eventType,
            OsmSharp.Math.StateMachines.FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate, 
            TransitionFinishedDelegate finishedDelegate)
        {
            return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventType, checkDelegate, finishedDelegate);
        }

        /// <summary>
        /// Generates a transition with an extra check-delegate!
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inverted"></param>
        /// <param name="eventType"></param>
        /// <param name="checkDelegate"></param>
        /// <param name="finishedDelegate"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition<EventType> Generate(
            List<FiniteStateMachineState<EventType>> states, int start, int end, bool inverted, Type eventType,
            OsmSharp.Math.StateMachines.FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate,
            TransitionFinishedDelegate finishedDelegate)
        {
            var conditions = new List<FiniteStateMachineTransitionCondition<EventType>>();
            conditions.Add(new FiniteStateMachineTransitionCondition<EventType>()
            {
                EventTypeObject = eventType,
                CheckDelegate = checkDelegate
            });

            var trans = new FiniteStateMachineTransition<EventType>()
            {
                SourceState = states[start],
                TargetState = states[end],
                TransitionConditions = conditions,
                Finished = finishedDelegate,
                Inverted = inverted
            };
            states[start].Outgoing.Add(trans);
            return trans;
        }

        /// <summary>
        /// Generates a transition.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inverted"></param>
        /// <param name="eventTypes"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, bool inverted, 
            params Type[] eventTypes)
        {
            List<FiniteStateMachineTransitionCondition<EventType>> conditions = new List<FiniteStateMachineTransitionCondition<EventType>>();
            foreach (Type type in eventTypes)
            {
                conditions.Add(new FiniteStateMachineTransitionCondition<EventType>()
                {
                    EventTypeObject = type
                });
            }

            FiniteStateMachineTransition<EventType> trans = new FiniteStateMachineTransition<EventType>()
            {
                SourceState = states[start],
                TargetState = states[end],
                TransitionConditions = conditions,
                Inverted = inverted
            };
            states[start].Outgoing.Add(trans);
            return trans;
        }

        #endregion
    }
}
