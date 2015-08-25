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

using OsmSharp.Math.StateMachines;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Automata
{
    /// <summary>
    /// Class representing a finite-state machine consuming objects representing events.
    /// </summary>
    public class FiniteStateMachine<EventType>
    {
        /// <summary>
        /// Keeps a list of already consumed event since the latest reset.
        /// </summary>
        private IList<EventType> _consumedEvents;

        /// <summary>
        /// Keeps the current state of this machine.
        /// </summary>
        private FiniteStateMachineState<EventType> _currentState;

        /// <summary>
        /// Keeps the initial state of this machine.
        /// </summary>
        private FiniteStateMachineState<EventType> _initialState;

        /// <summary>
        /// Creates a new finite state machine.
        /// </summary>
        public FiniteStateMachine()
        {
            // create the consumed events list.
            _consumedEvents = new List<EventType>();

            // set state.
            var initialState = this.BuildStates();
            _initialState = initialState;
            _currentState = initialState;
        }

        /// <summary>
        /// Creates a new finite state machine.
        /// </summary>
        public FiniteStateMachine(FiniteStateMachineState<EventType> initialState)
        {
            // create the consumed events list.
            _consumedEvents = new List<EventType>();

            // set state.
            _initialState = initialState;
            _currentState = initialState;
        }

        /// <summary>
        /// Builds the initial states if none was given.
        /// </summary>
        /// <returns></returns>
        protected virtual FiniteStateMachineState<EventType> BuildStates()
        {
            throw new NotSupportedException("Cannot create this FSM without an explicit initial state.");
        }

        #region Consumption/Reset

        /// <summary>
        /// Consumes the given event.
        /// </summary>
        /// <param name="even">The event.</param>
        /// <returns>True if the machine is in a final state.</returns>
        public bool Consume(EventType even)
        {
            var oldState = _currentState;

            // add to the consumed events.
            _consumedEvents.Add(even);

            // if the type matches on of the outgoing transitions; change state; else revert to initial.
            bool succes = false;
            bool final = false;
            foreach (var transition in _currentState.Outgoing)
            {
                if (transition.Match(this, even))
                {
                    succes = true;
                    _currentState = transition.TargetState;
                    this.NotifyStateTransition(even, _currentState);

                    transition.NotifySuccessfull(even);
                    break;
                }
            }

            // revert if unsuccesfull.
            if (!succes)
            {
                if (!_currentState.ConsumeAll)
                {
                    this.NotifyReset(even, _currentState);
                    this.Reset();
                }
            }
            else
            {
                if (_currentState.Final)
                {
                    final = true;
                    this.NotifyFinalState(_consumedEvents);
                    this.Reset();
                }
            }

            this.NotifyConsumption(even, _currentState, oldState);

            return final;
        }

        #endregion

        /// <summary>
        /// Resets this machine.
        /// </summary>
        public void Reset()
        {
            _consumedEvents.Clear();
            _currentState = _initialState;
        }

        #region Events

        /// <summary>
        /// Delegate containing an event object and it's associated state.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        public delegate void EventStateDelegate(EventType even, FiniteStateMachineState<EventType> state);

        /// <summary>
        /// Delegate containing an event object and it's associated state.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="newState"></param>
        /// <param name="oldState"></param>
        public delegate void EventStatesDelegate(EventType even, FiniteStateMachineState<EventType> newState, 
            FiniteStateMachineState<EventType> oldState);
        
        /// <summary>
        /// Delegate containing an event object list.
        /// </summary>
        /// <param name="events"></param>
        public delegate void EventsDelegate(IList<EventType> events);

        /// <summary>
        /// Event raised when an event is consumed.
        /// </summary>
        public event EventStatesDelegate ConsumptionEvent;

        /// <summary>
        /// Notify listeners an event was consumed.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="newState"></param>
        /// <param name="oldState"></param>
        private void NotifyConsumption(EventType even, 
            FiniteStateMachineState<EventType> newState, FiniteStateMachineState<EventType> oldState)
        {
            if (ConsumptionEvent != null)
            {
                ConsumptionEvent(even, newState, oldState);
            }
            this.RaiseConsumptionEvent(even, newState, oldState);
        }

        /// <summary>
        /// Raizes the comsumption event.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="newState"></param>
        /// <param name="oldState"></param>
        protected virtual void RaiseConsumptionEvent(EventType even, FiniteStateMachineState<EventType> 
            newState, FiniteStateMachineState<EventType> oldState)
        {

        }

        /// <summary>
        /// Event raised when a final state has been reached.
        /// </summary>
        public event EventsDelegate FinalStateEvent;

        /// <summary>
        /// Notify listeners when a final state has been reached.
        /// </summary>
        /// <param name="events"></param>
        private void NotifyFinalState(IList<EventType> events)
        {
            if (FinalStateEvent != null)
            {
                FinalStateEvent(events);
            }
            this.RaiseFinalStateEvent(events);
        }

        /// <summary>
        /// Raizes the final state event.
        /// </summary>
        /// <param name="events"></param>
        protected virtual void RaiseFinalStateEvent(IList<EventType> events)
        {

        }

        /// <summary>
        /// Event raised when a reset occured.
        /// </summary>
        public event EventStateDelegate ResetEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        private void NotifyReset(EventType even, FiniteStateMachineState<EventType> state)
        {
            if (ResetEvent != null)
            {
                ResetEvent(even,state);
            }
            this.RaiseResetEvent(even, state);
        }

        /// <summary>
        /// Raises the reset event.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        protected virtual void RaiseResetEvent(EventType even, FiniteStateMachineState<EventType> state)
        {

        }

        /// <summary>
        /// Event raised when a state transition occured.
        /// </summary>
        public event EventStateDelegate StateTransitionEvent;

        /// <summary>
        /// Notify listeners when a state transition occured.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        private void NotifyStateTransition(EventType even, FiniteStateMachineState<EventType> state)
        {
            if (StateTransitionEvent != null)
            {
                StateTransitionEvent(even, state);
            }
            this.RaiseStateTransitionEvent(even, state);
        }

        /// <summary>
        /// Raizes the state transition event.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        protected virtual void RaiseStateTransitionEvent(EventType even, FiniteStateMachineState<EventType> state)
        {

        }

        #endregion

        /// <summary>
        /// Returns a description of this machine.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_currentState != null)
            {
                return string.Format("{0}:{1}", this.GetType().Name, _currentState.ToString());
            }
            else
            {
                return string.Format("{0}:{1}", this.GetType().Name, "NO STATE");
            }
        }
    }
}