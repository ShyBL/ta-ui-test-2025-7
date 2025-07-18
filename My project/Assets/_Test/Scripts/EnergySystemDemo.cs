using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace _Test.Scripts
{
    public class EnergySystemDemo : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider energySlider;
        [SerializeField] private TextMeshProUGUI currentEnergyText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI refillsText;
        [SerializeField] private GameObject button;
        [SerializeField] private Image buttonImage;
        
        [Header("Settings")]
        [SerializeField] private float energyCost = 10f;
        [SerializeField] private float sliderAnimationDuration = 0.3f;
        [SerializeField] private Ease sliderEaseType = Ease.OutBack;
    
        private float _currentEnergy = 999f;
        private int _currentRefills = 999;
        private const float MaxEnergy = 999f;
        private bool _isAutoSpinning = false;
        private bool _isSliderAnimating = false;
        private Tween _currentSliderTween;

        private void Start()
        {
            energySlider.onValueChanged.AddListener(OnSliderChanged);
            UpdateUI();
        }
        
        private void Update()
        {
            switch (_isAutoSpinning)
            {
                case true when _currentEnergy >= energyCost:
                    _currentEnergy -= energyCost * Time.deltaTime;
                    _currentEnergy = Mathf.Max(_currentEnergy, 0f);
                    TweenEnergySlider();
                    break;
                case true when _currentEnergy < energyCost:
                    _isAutoSpinning = false;
                    break;
            }
        }
        
        public void WasteEnergy()
        {
            if (_currentEnergy >= energyCost)
            {
                _currentEnergy -= energyCost;
                TweenEnergySlider();
            }
        }
    
        public void AutoSpinWasteEnergy(bool enable)
        {
            _isAutoSpinning = enable;
        }
    
        public void UseRefill()
        {
            if (_currentRefills > 0)
            {
                _currentRefills--;
                _currentEnergy = MaxEnergy;
                TweenEnergySlider();
            }
        }
        
        private void TweenEnergySlider()
        {
            _currentSliderTween?.Kill();
            
            var targetValue = _currentEnergy / MaxEnergy;
            
            // Temporarily disable the slider change listener to prevent conflicts
            _isSliderAnimating = true;

            _currentSliderTween = energySlider.DOValue(targetValue, sliderAnimationDuration).SetEase(sliderEaseType);
            _currentSliderTween.OnUpdate(UpdateEnergyText);// Update text during animation for smooth number changes
            _currentSliderTween.OnComplete(() => {
                _isSliderAnimating = false;
                UpdateUI();
            });
        }
        
        private void OnSliderChanged(float value)
        {
            if (_isSliderAnimating) return;
            _currentEnergy = value * MaxEnergy;
            UpdateUI();
        }

        private void UpdateUI()
        {
            UpdateEnergyText();
            refillsText.text = _currentRefills.ToString();
            UpdateEnergyState();
        }
        
        private void UpdateEnergyText()
        {
            currentEnergyText.text = ((int)_currentEnergy).ToString();
            maxEnergyText.text = MaxEnergy.ToString(CultureInfo.InvariantCulture);
        }
    
        private void UpdateEnergyState()
        {
            var hasEnoughEnergy = _currentEnergy >= energyCost;
        
            button.GetComponent<AnimatedButton>().enabled = hasEnoughEnergy;

            // Make button look not interactive
            buttonImage.color = hasEnoughEnergy ? Color.white : Color.gray;
        }
        
        private void OnDestroy()
        {
            _currentSliderTween?.Kill();
        }
    }
}