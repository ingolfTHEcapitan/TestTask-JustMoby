using System.Collections.Generic;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Components
{
    public class PlayerVisuals : MonoBehaviour
    {
        private const float DeathSoundDelay = 0.2f;
        
        [Header("Components")]
        [SerializeField] private Weapon.Weapon _weapon;
        [SerializeField] private Health _health;

        [Header("Audio")]
        [SerializeField] private AudioSource _weaponAudioSource;
        [SerializeField] private AudioSource _playerAudioSource;

        [SerializeField] private AudioClip _weaponShootSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] private AudioClip _healSound;
        [SerializeField] private List<AudioClip> _takeDamageSounds;
        
        private AudioService _audioService;
        private EffectsService _effectsService;

        [Inject]
        private void Construct(AudioService audioService, EffectsService effectsService)
        {
            _audioService = audioService;
            _effectsService = effectsService;
        }
        
        public void Initialize()
        {
            _weapon.OnBulletHit += PlayBulletHitFx;
            _weapon.OnShoot += PlayWeaponShootSound;
            _health.OnZeroHealth += PlayDeathSound;
            _health.OnTakeDamage += PlayDamageSound;
            _health.OnTakeHeal += PlayHealSound;
        }

        private void OnDestroy()
        {
            _weapon.OnBulletHit -= PlayBulletHitFx;
            _weapon.OnShoot -= PlayWeaponShootSound;
            _health.OnZeroHealth -= PlayDeathSound;
            _health.OnTakeDamage -= PlayDamageSound;
            _health.OnTakeHeal -= PlayHealSound;
        }

        private void PlayBulletHitFx(Vector3 hitPosition, Transform targetContainer) => 
            _effectsService.PlayHitFxAsync(hitPosition, targetContainer).Forget(Debug.LogError);

        private void PlayWeaponShootSound() => 
            _audioService.PlayOneShot(_weaponShootSound, _weaponAudioSource);

        private void PlayDeathSound() => 
            _audioService.PlayDelayed(_deathSound, _playerAudioSource, DeathSoundDelay);

        private void PlayDamageSound() => 
            _audioService.PlayOneShotRandom(_takeDamageSounds, _playerAudioSource);

        private void PlayHealSound() => 
            _audioService.PlayOneShot(_healSound, _playerAudioSource);
    }
}