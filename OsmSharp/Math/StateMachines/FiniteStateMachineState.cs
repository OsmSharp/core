// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Math.StateMachines
{
    /// <summary>
    /// Represents a state in a finite-state machine.
    /// </summary>
    public sealed class FiniteStateMachineState<EventType>
    {
        /// <summary>
        /// The unique id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The list of possible outgoing transition.
        /// </summary>
        public IList<FiniteStateMachineTransition<EventType>> Outgoing { get; private set; }

        /// <summary>
        /// Boolean representing if the state is final.
        /// </summary>
        public bool Final { get; set; }

        /// <summary>
        /// Boolean representing if this state consumes event even if there is no outgoing transition.
        /// </summary>
        public bool ConsumeAll { get; set; }

        /// <summary>
        /// Returns a description of this state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("State[{0}]: Final: {1}",
                this.Id,
                this.Final);
        }

        #region Generate States

        /// <summary>
        /// Generates an amount of states.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<FiniteStateMachineState<EventType>> Generate(int count)
        {
            List<FiniteStateMachineState<EventType>> states = new List<FiniteStateMachineState<EventType>>();
            for (int idx = 0; idx < count; idx++)
            {
                states.Add(new FiniteStateMachineState<EventType>()
                {
                    Id = idx,
                    Outgoing = new List<FiniteStateMachineTransition<EventType>>(),
                    Final = false,
                    ConsumeAll = false
                });
            }
            return states;
        }

        #endregion
    }
}
