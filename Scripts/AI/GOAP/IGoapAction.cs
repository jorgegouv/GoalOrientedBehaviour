using System.Collections.Generic;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP
{
    public interface IGoapAction
    {
        /// <summary>
        /// The set of preconditions that must be fulfilled for this action to take place
        /// </summary>
        HashSet<KeyValuePair<string, object>> Preconditions { get; }

        /// <summary>
        /// The set of consequences that will take effect once this action is completed. 
        /// </summary>
        HashSet<KeyValuePair<string, object>> Effects { get; }

        /// <summary>
        /// The set of consequences that will take effect once this action is completed. 
        /// </summary>
        /// <summary>
        /// Are we in range of the target?
        /// The MoveTo state will set this and it gets reset each time this action is performed.
        /// </summary>
        bool InRange { get; set; }

        /// <summary>
        /// An action often has to perform on an object. This is that object. Can be null.
        /// </summary>
        GameObject Target { get; set; }

        float Cost { get; set; }

        /// <summary>
        /// Resets all the variables used for the action.
        /// </summary>
        void DoReset();

        /// <summary>
        /// Reset any variables that need to be reset before planning happens again.
        /// </summary>
        void Reset();

        /// <summary>
        /// Is the action done?
        /// </summary>
        /// <returns></returns>
        bool IsDone();

        /// <summary>
        /// Procedurally check if this action can run. Not all actions will need this, but some might.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool CheckProceduralPrecondition(GameObject agent);

        /// <summary>
        /// Run the action. Returns True if the action performed successfully or false if something happened and it can no longer perform. In this case the action queue should clear out and the goal cannot be reached.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool Perform(GameObject agent);

        /// <summary>
        /// Does this action need to be within range of a target game object?
        /// If not then the moveTo state will not need to run for this action.
        /// </summary>
        /// <returns></returns>
        bool RequiresInRange();

        /// <summary>
        /// Add a precondition to the preconditions set. No repeats allowed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddPrecondition(string key, object value);

        /// <summary>
        /// Removes the precodition associated with the received key
        /// </summary>
        /// <param name="key"></param>
        void RemovePrecondition(string key);

        /// <summary>
        /// Add an effect ot the effects list of this action. No repeats allowed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddEffect(string key, object value);

        /// <summary>
        /// Remove the effect with the received key from this aciton
        /// </summary>
        /// <param name="key"></param>
        void RemoveEffect(string key);

        /// <summary>
        /// Determines if the action is still being performed
        /// </summary>
        /// <returns></returns>
        bool StillWorking();
    }
}