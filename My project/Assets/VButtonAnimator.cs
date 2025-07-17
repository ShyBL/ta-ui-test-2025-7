using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class VButtonAnimator : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite autoSpinSprite;
    
    [Header("Auto Spin Settings")]
    [SerializeField] private float holdThreshold = 3f;
    
    [Header("Press Animation Settings")]
    [SerializeField] private float pressAnimationDistance = 10f;
    [SerializeField] private float pressAnimationDuration = 0.1f;
    [SerializeField] private Ease pressEase = Ease.OutQuad;
    
    [Header("Pop Up Animation Settings")]
    [SerializeField] private float popAnimationDistance = 20f;
    [SerializeField] private float popAnimationDuration = 0.3f;
    [SerializeField] private Ease popEase = Ease.OutBounce;
    
    private Image buttonImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Sequence currentAnimation;
    private float pressStartTime;
    private int pressCount = 0;
    
    private bool isPointerDown = false;
    private bool popTweenPending = false;
    private bool shouldPopUp = false;
    private bool shouldSwitchSprite = false;
    private bool isAnimating = false;
    private bool isAutoSpin = false;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        
        if (buttonImage == null) buttonImage = GetComponent<Image>();
    }
    
    // Called by EventTrigger Component
    public void OnPointerDown(BaseEventData eventData)
    {
        if (isAnimating || isAutoSpin || isPointerDown) return;
        isPointerDown = true;
        pressStartTime = Time.time;
        shouldPopUp = false;
        shouldSwitchSprite = false;
        StartPressDownTween();
    }

    // Called by EventTrigger Component
    public void OnPointerUp(BaseEventData eventData)
    {
        if (!isPointerDown || isAnimating || isAutoSpin) return;
        isPointerDown = false;
        float heldTime = Time.time - pressStartTime;
        if (heldTime >= holdThreshold)
        {
            // After press down finishes, switch sprite to auto spin
            shouldSwitchSprite = true;
        }
        else
        {
            // After press down finishes, play pop up tween
            shouldPopUp = true;
        }
    }

    private void StartPressDownTween()
    {
        isAnimating = true;
        buttonImage.color = Color.gray;
        currentAnimation?.Kill();
        currentAnimation = DOTween.Sequence();
        currentAnimation.Append(rectTransform.DOAnchorPosY(originalPosition.y - pressAnimationDistance, pressAnimationDuration)
            .SetEase(pressEase));
        
        currentAnimation.OnComplete(() => {
            isAnimating = false;
            buttonImage.color = Color.white;
            
            // After press down finishes, check what to do next
            if (shouldPopUp)
            {
                shouldPopUp = false;
                StartPopUpTween();
            }
            else if (shouldSwitchSprite)
            {
                shouldSwitchSprite = false;
                buttonImage.sprite = autoSpinSprite;
            }
        });
    }
    private void StartPopUpTween()
    {
        if (isAnimating) return;
        isAnimating = true;
        buttonImage.color = Color.gray;
        
        currentAnimation?.Kill();
        currentAnimation = DOTween.Sequence();
        currentAnimation.Append(rectTransform.DOAnchorPosY(originalPosition.y + popAnimationDistance, popAnimationDuration)
            .SetEase(popEase));
        
        currentAnimation.OnComplete(() => {
            isAnimating = false;
            buttonImage.color = Color.white;
        });
    }
    
    private void StartAutoSpin()
    {
        isAutoSpin = true;
        buttonImage.sprite = autoSpinSprite;
    }
    
    public void StopAutoSpin()
    {
        if (!isAutoSpin) return;
        
        isAutoSpin = false;
        
        // Return to original position and state
        rectTransform.DOAnchorPosY(originalPosition.y, 0.3f);
        
        buttonImage.sprite = normalSprite;
        buttonImage.color = Color.white;
    }
    
    private void OnDestroy()
    {
        currentAnimation?.Kill();
    }
}