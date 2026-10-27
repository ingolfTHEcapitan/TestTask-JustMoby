using System.Collections.Generic;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.Components.SatteMachine;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Enemy.Components
{
    public class EnemyVisuals : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Health _health;
        [Header("Audio")]
        [SerializeField] private AudioSource _enemyAudioSource;
        [SerializeField] private List<AudioClip> _swordHitSounds;
        [SerializeField] private List<AudioClip> _swordMissSounds;
        [SerializeField] private List<AudioClip> _takeDamageSounds;
        [Header("FX")]
        [SerializeField] private DissolveShader _dissolveShader;

        private readonly int _dieHash = Animator.StringToHash("Die");
        private readonly int _takeDamageHash = Animator.StringToHash("TakeDamage");
        private readonly int _attackHash = Animator.StringToHash("Attack");
        
        private ISoundService _audioService;
        private EffectsService _effectsService;
        private EnemyStateMachine _stateMachine;
        
        private float _tempAnimationSpeed;

        [Inject]
        private void Construct(ISoundService audioService, EffectsService effectsService)
        {
            _audioService = audioService;
            _effectsService = effectsService;
        }

        public void Initialize(EnemyStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            
            _health.OnTakeDamage += PlayDamageAnimation;
            _health.OnTakeDamage += PlayDamageSound;
            _health.OnZeroHealth += PlayDeathAnimation;
            _stateMachine.OnAttackStarted += PlayAttackAnimation;
            _stateMachine.OnAttackMissed += PlayMissSound;
            _stateMachine.OnAttackHit += PlayHitSound;
        }
        
        private void OnDestroy()
        {
            _health.OnTakeDamage -= PlayDamageAnimation;
            _health.OnTakeDamage -= PlayDamageSound;
            _health.OnZeroHealth -= PlayDeathAnimation;
            _stateMachine.OnAttackStarted -= PlayAttackAnimation;
            _stateMachine.OnAttackMissed -= PlayMissSound;
            _stateMachine.OnAttackHit -= PlayHitSound;
        }

        [UsedImplicitly]
        public void OnDeathPose()
        {
            _dissolveShader.PlayDissolveFx();
            _effectsService.PlayEnemyDeathFxAsync(transform.position, transform).Forget(Debug.LogError);
        }

        private void PlayDamageAnimation() => 
            _animator.SetTrigger(_takeDamageHash);

        private void PlayDeathAnimation() => 
            _animator.SetTrigger(_dieHash);

        private void PlayAttackAnimation() => 
            _animator.SetTrigger(_attackHash);
        
        private void PlayHitSound() => 
            _audioService.PlayOneShotRandom(_swordHitSounds, _enemyAudioSource);

        private void PlayMissSound() => 
            _audioService.PlayOneShotRandom(_swordMissSounds, _enemyAudioSource);
        
        private void PlayDamageSound() => 
            _audioService.PlayOneShotRandom(_takeDamageSounds, _enemyAudioSource);
    }
}