using System.Collections.Generic;
using Assets.EOTS;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP
{
    /// <summary>
    /// Collect the world data for this Agent that will be used for GOAP planning. 
    /// Any agent that wants to use GOAP must implement this interface. It provides information to the GOAP planner so it can plan what actions to use.
    ///
    ///  It also provides an interface for the planner to give feedback to the Agent and report success/failure.
    /// </summary>
    public interface IGoap 
    {
        /// <summary>
        /// The starting state of the Agent and the world. Supply what states are needed for actions to run.
        /// </summary>
        /// <returns></returns>
        HashSet<KeyValuePair<string,object>> GetWorldState ();

        /// <summary>
        /// Give the planner a new goal so it can figure out the actions needed to fulfill it.
        /// </summary>
        /// <returns></returns>
        HashSet<KeyValuePair<string,object>> CreateGoalState ();

        /// <summary>
        /// No sequence of actions could be found for the supplied goal. You will need to try another goal
        /// </summary>
        /// <param name="failedGoal"></param>
        void PlanFailed (HashSet<KeyValuePair<string,object>> failedGoal);

        /// <summary>
        /// A plan was found for the supplied goal. These are the actions the Agent will perform, in order.
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="actions"></param>
        void PlanFound (HashSet<KeyValuePair<string,object>> goal, Queue<IGoapAction> actions);

        /// <summary>
        /// All actions are complete and the goal was reached. Hooray!
        /// </summary>
        void ActionsFinished ();

        /// <summary>
        /// Finihed the current action. There are still more actions in the plan.
        /// </summary>
        void CurrentActionFinished(IGoapAction currentAction, IGoapAction nextAction);

        /// <summary>
        /// This function is called once a action is completed and there is no more actions in the plan
        /// </summary>
        /// <param name="currentAction"></param>
        void CurrentActionFinished(IGoapAction currentAction);

        /// <summary>
        /// One of the actions caused the plan to abort. That action is returned.
        /// </summary>
        /// <param name="aborter"></param>
        void PlanAborted (IGoapAction aborter);

        /// <summary>
        /// Called during Update.Move the agent towards the target in order for the next action to be able to perform.
        /// Return true if the Agent is at the target and the next action can perform.False if it is not there yet.
        /// </summary>
        /// <param name="nextAction"></param>
        /// <returns></returns>
        bool MoveAgent(IGoapAction nextAction);


        void Died();
    }
}

