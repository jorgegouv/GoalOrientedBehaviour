using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP
{
    /// <summary>
    /// Defines an general abstract action to be used for the <see cref="GoapPlanner"/>
    /// </summary>
    public abstract class GoapAction : MonoBehaviour, IGoapAction
    {
        /// <summary>
        /// The set of preconditions that must be fulfilled for this action to take place
        /// </summary>
        private HashSet<KeyValuePair<string, object>> _preconditions = new HashSet<KeyValuePair<string, object>>();
        /// <summary>
        /// The set of preconditions that must be fulfilled for this action to take place
        /// </summary>
        public HashSet<KeyValuePair<string, object>> Preconditions { get { return _preconditions; } }
        /// <summary>
        /// The set of consequences that will take effect once this action is completed. 
        /// </summary>
        private HashSet<KeyValuePair<string, object>> _effects = new HashSet<KeyValuePair<string, object>>();
        /// <summary>
        /// The set of consequences that will take effect once this action is completed. 
        /// </summary>
        public HashSet<KeyValuePair<string, object>> Effects { get { return _effects; } }
        /// <summary>
        /// The set of consequences that will take effect once this action is completed. 
        /// </summary>
        
        /// <summary>
        /// Are we in range of the target?
        /// The MoveTo state will set this and it gets reset each time this action is performed.
        /// </summary>
        public bool InRange { get; set; }

        /// <summary>
        /// How long it takes to complete this action
        /// </summary>
        public float Duration = 0; // bug asdasdasd

        /// <summary>
        /// The time this action has started execution
        /// </summary>
        protected float StartTime;

        [SerializeField] private float _cost = 1f;

        /// <summary>
        /// An action often has to perform on an object. This is that object. Can be null.
        /// </summary>
        public GameObject Target { get; set; }

        /// <summary>
        /// The cost of performing the action. 
        /// Figure out a weight that suits the action.
        /// Changing it will affect what actions are chosen during planning.
        /// </summary>
        public float Cost
        {
            get => _cost;
            set => _cost = value;
        }

        /// <summary>
        /// Resets all the variables used for the action.
        /// </summary>
        public void DoReset()
        {
            InRange = false;
            Target = null;
            Reset(); // calls the reset specified in the concrete action
        }

        /// <summary>
        /// Reset any variables that need to be reset before planning happens again.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Is the action done?
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDone();

        /// <summary>
        /// Procedurally check if this action can run. Not all actions will need this, but some might.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public abstract bool CheckProceduralPrecondition(GameObject agent);
        
        /// <summary>
        /// Run the action. Returns True if the action performed successfully or false if something happened and it can no longer perform. In this case the action queue should clear out and the goal cannot be reached.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public abstract bool Perform(GameObject agent);

        /// <summary>
        /// Does this action need to be within range of a target game object?
        /// If not then the moveTo state will not need to run for this action.
        /// </summary>
        /// <returns></returns>
        public abstract bool RequiresInRange();

        /// <summary>
        /// Add a precondition to the preconditions set. No repeats allowed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddPrecondition(string key, object value)
        {
            if (_preconditions.Any(pair => pair.Key == key))
            {
                Debug.LogError("This condition was already added before!");
                return; // if there is already a pair with this key, you cannot add a new one
            }

            _preconditions.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Removes the precodition associated with the received key
        /// </summary>
        /// <param name="key"></param>
        public void RemovePrecondition(string key)
        {
            var remove = _preconditions.FirstOrDefault(pair => pair.Key == key);
            // if the remove is equal to the Default, there is no precondition with this key
            if (default(KeyValuePair<string, object>).Equals(remove) == false)
                _preconditions.Remove(remove);
        }

        /// <summary>
        /// Add an effect ot the effects list of this action. No repeats allowed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddEffect(string key, object value)
        {
            if (_effects.Any(pair => pair.Key == key))
            {
                Debug.LogError("This effect has already been added before.");
                return;
            }
            _effects.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Remove the effect with the received key from this aciton
        /// </summary>
        /// <param name="key"></param>
        public void RemoveEffect(string key)
        {
            var remove = _effects.FirstOrDefault(pair => pair.Key == key);

            if (default(KeyValuePair<string, object>).Equals(remove) == false)
                _effects.Remove(remove);
        }

        /// <summary>
        /// Determines if the action is still being performed
        /// </summary>
        /// <returns></returns>
        public bool StillWorking()
        {
            return Time.time - StartTime < Duration;
        }
    }
}