using Assets.EOTS;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.GameData.Soldiers
{
    public interface ISoldier
    {
        Transform MyTransform { get; set; }
        Teams MyTeam { get; set; }
        bool Invulnerable { get; set; }
        bool HasFlag { get; set; }


        void Died();

    }
}