using _Project.Scripts.Services.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace _Project.Scripts.UI.Common
{
	public class ButtonSoundEffect : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
	{
		[SerializeField] private AudioClip _onMouseClickSound;
		[SerializeField] private AudioClip _onMouseEnterSound;
		[SerializeField] private AudioSource _audioSource;
		
		private ISoundService _audioService;

		[Inject]
		private void Construct(ISoundService audioService) => 
			_audioService = audioService;

		public void Initialize(AudioSource audioSource) => 
			_audioSource = audioSource;

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_onMouseClickSound == null)
			{
				Debug.LogWarning("[ButtonSoundEffect] OnMouseClickSound is null");
				return;
			}
				
			_audioService.PlayOneShot(_onMouseClickSound, _audioSource);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_onMouseEnterSound == null)
			{
				Debug.LogWarning("[ButtonSoundEffect] OnMouseEnterSound is null");
				return;
			}
			
			_audioService.PlayOneShot(_onMouseEnterSound, _audioSource);
		}
	}
}