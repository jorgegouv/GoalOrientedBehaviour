using System.Collections;
using System.Collections.Generic;
using Assets.EOTS;
using UnityEngine;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class GuardAction : GoapAction
    {
        private bool _invulnerable;
        private bool _onCooldown;
        private FlagComponent _flag;
        // Start is called before the first frame update
        public override void Reset()
        {
            _invulnerable = false;
        }

        // Update is called once per frame
        public override bool IsDone()
        {
            return _invulnerable;
        }
        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (_soldier.Invulnerable == false && _flag.BeingCarried == false && _flag.CanBeCarried)
                Target = _flag.gameObject;

            return _flag.BeingCarried == false;
        }
        public override bool Perform(GameObject agent)
        {
            _soldier.Bubble();
            StartCoroutine(StartCooldown());
            _invulnerable = true;

            return true;
        }
        public override bool RequiresInRange()
        {
            throw new System.NotImplementedException();
        }
        private Soldier _soldier;
        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            AddEffect("invulnerable", true);
        }
        private IEnumerator StartCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(30f);
            _onCooldown = false;
        }


    }
}