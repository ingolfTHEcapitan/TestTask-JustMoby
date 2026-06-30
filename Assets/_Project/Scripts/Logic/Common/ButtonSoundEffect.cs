using _Project.Scripts.Services.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace _Project.Scripts.Logic.Common
{
	public class ButtonSoundEffect : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
	{
		[SerializeField] private AudioClip _pointerDown;
		[SerializeField] private AudioClip _pointerEnter;
		[SerializeField] private AudioSource _audioSource;
		
		private IAudioService _audioService;

		[Inject]
		public void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}

		public void Initialize(AudioSource audioSource) => 
			_audioSource = audioSource;

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_pointerDown != null) 
				_audioService.PlayOneShot(_pointerDown, _audioSource);
			
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_pointerEnter != null) 
				_audioService.PlayOneShot(_pointerEnter, _audioSource);
		}
	}
}