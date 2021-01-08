using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.Scripts.HSM.Abstracts;
using Assets.Scripts.HSM.Concretes;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP.HSM.Conditions
{
    /// <summary>
    /// <see cref="ICondition"/> used by the <see cref="HierarchicalStateMachine"/>. Returns true of the <see cref="GoapAgent"/> of this state machine needs to move
    /// </summary>
    public class NeedToMoveCondition : ICondition
    {
        /// <summary>
        /// Returns true of the <see cref="GoapAgent"/> of this state machine needs to move
        /// </summary>
        /// <param name="watch"></param>
        /// <returns></returns>
        public bool Test(object watch)
        {
            return ((GoapAgent)watch).NeedToMove;
        }
    }
}
