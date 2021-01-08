using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.Scripts.HSM.Abstracts;
using Assets.Scripts.HSM.Concretes;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP.HSM.Actions
{
    /// <summary>
    /// Active action of the Acting State of the <see cref="HierarchicalStateMachine"/>.
    /// </summary>
    public class PlanAction : IAction
    {
        /// <summary>
        /// The <see cref="GoapPlanner "/> that will be used to plan the actions
        /// </summary>
        private readonly GoapPlanner _planner;
        /// <summary>
        /// The <see cref="GoapAgent"/> data provider
        /// </summary>
        private readonly IGoap _dataProvider;
        /// <summary>
        /// The <see cref="GoapAgent"/> that is planning
        /// </summary>
        private readonly GoapAgent _agent;

        public PlanAction(GoapPlanner planner, IGoap dataProvider, GoapAgent agent)
        {
            _planner = planner;
            _dataProvider = dataProvider;
            _agent = agent;
        }


        /// <summary>
        /// Implementation of <see cref="IAction"/> interface. Calls the Plan method of the <see cref="GoapPlanner"/>.
        /// </summary>
        public void Execute()
        {
            _agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            
            // get the world state and the goal we want to plan for
            var worldState = _dataProvider.GetWorldState();
            var goal = _dataProvider.CreateGoalState();

            // Plan
            var plan = _planner.Plan(_agent.gameObject, _agent.GetAvailableActions(), worldState, goal);
            if (plan != null)// we have a plan, hooray!
            {
                _agent.SetCurrentActions(plan);
                _dataProvider.PlanFound(goal, plan);
            }
            else// ugh, we couldn't get a plan
                _dataProvider.PlanFailed(goal);
        }
    }
}
