using Assets.EOTS;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class DropFlag : GoapAction
    {
        private bool _flagDropped;
        private Soldier _soldier;
        private FlagComponent _flag;
        private bool _hasFlag;

        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            _flag = FindObjectOfType<FlagComponent>();
            AddPrecondition("hasFlag", true); // we must have the flag to drop it at the base
            AddEffect("hasFlag", false); // we will no longer have the flag after we drop it

        }

        public override void Reset()
        {
            _flagDropped = false;
            StartTime = 0;
        }

        public override bool IsDone()
        {
            return _flagDropped;
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
                return false;

            _flag.Drop();
            _soldier.HasFlag = false;
            _flagDropped = false;

            return true;
        }

        public override bool RequiresInRange()
        {
            return false;
        }
    }
}