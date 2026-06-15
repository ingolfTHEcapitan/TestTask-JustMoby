using UnityEngine;

namespace _Project.Scripts.Services.LoadingCurtain.UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private float _indicatorRotationSpeed = 100f;

        private void Awake() => 
            DontDestroyOnLoad(gameObject);

        public void Show() => 
            gameObject.SetActive(true);

        public void Hide() => 
            gameObject.SetActive(false);

        private void Update()
        {
            _loadingIndicator.transform.Rotate(Vector3.forward, Time.deltaTime * _indicatorRotationSpeed);
        }
    }
}