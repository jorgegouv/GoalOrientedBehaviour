using System.Linq;
using Assets.EOTS;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class ScoreFlag : GoapAction
    {
        /// <summary>
        /// The object used for the effect
        /// </summary>
        private bool _scored;

        /// <summary>
        /// Target of this action
        /// </summary>
        private Transform _myTeamBase;

        private Base _droppingBase;
        private FlagComponent _flag;

        private Soldier _soldier;

        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            _flag = FindObjectOfType<FlagComponent>();
            AddPrecondition("hasFlag", true); // we must have the flag to drop it at the base
            AddEffect("scored", true); // we will have dropped the flag once we finish
            AddEffect("hasFlag", false); // we will no longer have the flag after we drop it

        }

        public override void Reset()
        {
            //print("Reset action");
            _scored = false;
            StartTime = 0;
        }

        public override bool IsDone()
        {
            return _scored;
        }

        public override bool RequiresInRange()
        {
            return true; // you must be in range to drop the flag
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (_soldier.Invulnerable == false && _flag.BeingCarried == false && _flag.CanBeCarried)
                Target = _flag.gameObject;

            return _flag.BeingCarried == false;
        }

        public override bool Perform(GameObject agent)
        {

            if (_soldier.HasFlag == false)
                return false; // lost the flag somewhere

            _flag.Score(_droppingBase.MyTeam);
            _soldier.HasFlag = false;
            _scored = true; // you have dropped the flag

            //print("scored flag");

            return true;
        }
    }
}