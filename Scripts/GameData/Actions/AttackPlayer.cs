using System.Collections;
using System.Linq;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;
using Assets.EOTS;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class AttackPlayer : GoapAction
    {
        private bool _attacked;
        private bool _onCooldown;
        private Soldier _target;
        private Soldier _soldier;
        private FlagComponent _flag;

        public override void Reset()
        {
            _attacked = false;
        }

        public override bool IsDone()
        {
            return _attacked;
        }

        public override bool RequiresInRange()
        {
            return true;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            // can be changed to several strategies. in this case, we will attack the closest agent

            if (_soldier.Invulnerable == false && _flag.BeingCarried == false && _flag.CanBeCarried)
                Target = _flag.gameObject;

            return _flag.BeingCarried == false;
        }
         
        public override bool Perform(GameObject agent)
        {
            if (Target == null || _target.Invulnerable || _onCooldown || Vector3.Distance(agent.transform.position, _target.transform.position) > 1.5f)
                return false;

            
            _target.Died();
            _target = null;
            StartCoroutine(StartCooldown());

            _attacked = true;
            return true;
        }


        private IEnumerator StartCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(20f);
            _onCooldown = false;
        }
    }
}