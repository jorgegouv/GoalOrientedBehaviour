using Assets.Scripts.HSM.Abstracts;
using Assets.Scripts.HSM.Concretes;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP.HSM.Conditions
{
    /// <summary>
    /// <see cref="ICondition"/> used by the <see cref="HierarchicalStateMachine"/>. Returns true of the <see cref="GoapAgent"/> of this state machine is ready to perform an action
    /// </summary>
    public class ReadyToPerformActionCondition : ICondition
    {
        /// <summary>
        /// Returns true of the <see cref="GoapAgent"/> of this state machine is ready to perform an action
        /// </summary>
        /// <param name="watch"></param>
        /// <returns></returns>
        public bool Test(object watch)
        {
            var agent = (GoapAgent) watch;

            return agent.NeedToMove == false && agent.NeedNewPlan == false;
        }
    }
}
