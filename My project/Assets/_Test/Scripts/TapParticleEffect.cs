using UnityEngine;
using UnityEngine.EventSystems;

namespace _Test.Scripts
{
    public class TapParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem; 
        private Camera _worldCamera;
        
        private void Start()
        {
            _worldCamera = Camera.main;
        }
        
        public void PlayParticlesAtClickPosition(BaseEventData eventData)
        {
            if (particleSystem == null || _worldCamera == null) return;
            
            var screenPos = Vector2.zero;
            
            if (eventData is PointerEventData pointerData)
            {
                screenPos = pointerData.position;
            }
            
            var worldPos = _worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y));
            
            particleSystem.transform.position = worldPos;
            particleSystem.Play();
        }
    }
}