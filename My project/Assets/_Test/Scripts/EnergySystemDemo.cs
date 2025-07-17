using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
        private float _currentEnergy;
        private int _currentRefills = 999;
        private const float MaxEnergy = 999f;
        private bool _isAutoSpinning = false;

        private void Start()
        {
            // Set initial energy to 20%
            _currentEnergy = MaxEnergy * 0.2f;
        
            // Setup slider
            energySlider.minValue = 0f;
            energySlider.maxValue = 1f;
            energySlider.value = 0.2f;
            energySlider.onValueChanged.AddListener(OnSliderChanged);
        
            UpdateUI();
        }

        private void OnSliderChanged(float value)
        {
            _currentEnergy = value * MaxEnergy;
            UpdateUI();
        }

        private void UpdateUI()
        {
            currentEnergyText.text = ((int)_currentEnergy).ToString();
            maxEnergyText.text = MaxEnergy.ToString(CultureInfo.InvariantCulture);
            refillsText.text = _currentRefills.ToString();
        
            energySlider.value = _currentEnergy / MaxEnergy;

            UpdateEnergyState();
        }

        private void Update()
        {
            switch (_isAutoSpinning)
            {
                case true when _currentEnergy >= energyCost:
                    _currentEnergy -= energyCost * Time.deltaTime;
                    UpdateUI();
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
                UpdateUI();
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
                UpdateUI();
            }
        }
    
        private void UpdateEnergyState()
        {
            var hasEnoughEnergy = _currentEnergy >= energyCost;
        
            button.GetComponent<AnimatedButton>().enabled = hasEnoughEnergy;

            // Make button look not interactive
            buttonImage.color = hasEnoughEnergy ? Color.white : Color.gray;
        }
    }
}
