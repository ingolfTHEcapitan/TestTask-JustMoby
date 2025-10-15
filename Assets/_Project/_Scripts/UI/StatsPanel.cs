using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class StatsPanel: MonoBehaviour
    {
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private Button _closeButton;
        
        private bool _isOpen;

        private void Awake() => 
            _closeButton.onClick.AddListener(Hide);

        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab)) 
                Show();
        }

        private void OnDestroy() => 
            _closeButton.onClick.RemoveListener(Hide);

        private void Show()
        {
            if (_isOpen)
                return;
            
            _isOpen = true;
            
            _statsPanel.SetActive(true);
            CursorController.ShowCursor();
        }

        private void Hide()
        {
            _isOpen = false;
            _statsPanel.SetActive(false);
            CursorController.HideCursor();
        }
    }
}