using Assets.EOTS;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class PickUpFlag : GoapAction
    {
        /// <summary>
        /// The object used for the effect
        /// </summary>
        private bool _hasFlag;

        /// <summary>
        /// The target of this action
        /// </summary>
        private FlagComponent _flag;
        private Soldier _soldier;

        private void Awake()
        {
            AddPrecondition("hasFlag", false); // we cannot have the flag to pick up the flag
            AddEffect("hasFlag", true); // we will have the flag after we picked it up

            // cache the flag
            _flag = FindObjectOfType<FlagComponent>();
            Target = _flag.gameObject;
        }

        /// <summary>
        /// Resets the action to its default values, so it can be used again.
        /// </summary>
        public override void Reset()
        {
            //print("Reset action");

            _hasFlag = false;
            StartTime = 0;
        }

        /// <summary>
        /// Check if the action has been completed
        /// </summary>
        /// <returns></returns>
        public override bool IsDone()
        {
            return _hasFlag;
        }

        /// <summary>
        /// Checks if the agent need to be in range of the target to complete this action.
        /// </summary>
        /// <returns></returns>
        public override bool RequiresInRange()
        {
            return true; // yes we need to be near the flag to pick it up  
        }

        /// <summary>
        /// Checks if there is a <see cref="ChoppingBlockComponent"/> close to the agent.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            // the flag will "be worked" if there is some1 carrying it. Otherwise it is free to be picked up.
            if (_soldier.Invulnerable == false && _flag.BeingCarried == false && _flag.CanBeCarried)
                Target = _flag.gameObject;

            return _flag.BeingCarried == false;
        }

        /// <summary>
        /// Once the WorkDuration is compelted, adds 5 FireWood to the agent's backpack.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public override bool Perform(GameObject agent)
        {
            if (_soldier.Invulnerable)
                return false;

            if (Target == null)
                return false;

            if (_flag.BeingCarried || _flag.BeingCarried == false) return false;

            var runner = agent.GetComponent<Soldier>();

            _hasFlag = true;
            runner.HasFlag = true;
            _flag.PickUp(runner);

            print("picked up flag");

            return true;
        }
    }
}