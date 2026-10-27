using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.Components;
using _Project.Scripts.Logic.Enemy.Components.SatteMachine;
using _Project.Scripts.UI.Common.HealthBar;
using UnityEngine;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyContainer: MonoBehaviour
    {
        [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }
        [field: SerializeField] public EnemyVisuals Visuals { get; private set; }
        [field: SerializeField] public EnemyDeath EnemyDeath { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public EnemyRotateToPlayer RotateToPlayer { get; private set; }
        [field: SerializeField] public HealthBarView HealthBarView { get; private set; }
    }
}