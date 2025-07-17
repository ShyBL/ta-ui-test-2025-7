using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Test.Scripts
{
    [System.Serializable]
    public class PointerDataUnityEvent : UnityEvent<BaseEventData> { }
    
    [RequireComponent(typeof(RectTransform))]
    public class AnimatedButton : MonoBehaviour
    {
        [Header("Required Components")]
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Sprite autoSpinSprite;
        [SerializeField] private Image buttonImage;

        [Header("Auto Spin Settings")]
        [SerializeField] private float holdThreshold = 3f;

        [Header("Press Animation Settings")]
        [SerializeField] private float pressAnimationDistance = 10f;

        [SerializeField] private float pressAnimationDuration = 0.1f;
        [SerializeField] private Ease pressEase = Ease.OutQuad;

        [Header("Pop Up Animation Settings")]
        [SerializeField]private float popAnimationDistance = 20f;
        [SerializeField] private float popAnimationDuration = 0.3f;
        [SerializeField] private Ease popEase = Ease.OutBounce;
        
        [Header("Button Events")]
        public PointerDataUnityEvent buttonPressed;
        public UnityEvent buttonHeld;
        
        private RectTransform _rectTransform;
        private Vector3 _originalPosition;
        private Sequence _currentAnimation;
        private float _pressStartTime;
        private bool _isPointerDown = false;
        private bool _shouldPopUp = false;
        private bool _isAutoSpin = false;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalPosition = _rectTransform.anchoredPosition;
            if (buttonImage == null) buttonImage = GetComponent<Image>();
        }

        // Called by EventTrigger Component
        public void OnPointerDown(BaseEventData eventData)
        {
            if (_isAutoSpin || _isPointerDown) return;
            _isPointerDown = true;
            _pressStartTime = Time.time;
            _shouldPopUp = false;
            StartPressDownTween();
            buttonPressed.Invoke(eventData);
        }

        // Called by EventTrigger Component
        public void OnPointerUp(BaseEventData eventData)
        {
            // Avoid Double-Clicking or if state changed to AutoSpin this or next frame
            if (!_isPointerDown || _isAutoSpin) return;
            _isPointerDown = false;
            var startTime = Time.time - _pressStartTime;
            
            //_shouldPopUp = !(startTime >= holdThreshold);
            if (startTime >= holdThreshold)
            {
                _shouldPopUp = false;
                buttonHeld.Invoke();
            }
            else
            {
                _shouldPopUp = true;
            }
        }

        private void StartPressDownTween()
        {
            buttonImage.color = Color.gray;
            _currentAnimation?.Kill();
            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_rectTransform.DOAnchorPosY(_originalPosition.y - pressAnimationDistance,
                pressAnimationDuration).SetEase(pressEase));
            _currentAnimation.OnComplete(() =>
            {
                // After press down finishes, check what to do next
                if (_shouldPopUp)
                {
                    _shouldPopUp = false;
                    
                    buttonImage.sprite = pressedSprite;
                    
                    StartPopUpTween();
                }
                else
                {
                    buttonImage.sprite = autoSpinSprite;
                }
            });
        }

        private void StartPopUpTween()
        {
            _currentAnimation?.Kill();
            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_rectTransform.DOAnchorPosY(_originalPosition.y, popAnimationDuration)
                .SetEase(popEase));
            _currentAnimation.OnComplete(() =>
            {
                buttonImage.color = Color.white;
                buttonImage.sprite = normalSprite;
                if (_isAutoSpin)
                {
                }
            });
        }

        private void StartAutoSpin()
        {
            _isAutoSpin = true;
            buttonImage.sprite = autoSpinSprite;
        }

        public void StopAutoSpin()
        {
            if (!_isAutoSpin) return;
            _isAutoSpin = false;

            // Return to original position and state
            _rectTransform.DOAnchorPosY(_originalPosition.y, 0.3f);
            buttonImage.sprite = normalSprite;
            buttonImage.color = Color.white;
        }

        private void OnDestroy()
        {
            _currentAnimation?.Kill();
        }
    }
}