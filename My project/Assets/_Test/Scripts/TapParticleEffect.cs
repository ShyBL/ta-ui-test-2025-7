using UnityEngine;
using UnityEngine.EventSystems;

namespace _Test.Scripts
{
    public class TapParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem; 
        private Camera worldCamera;
        
        private void Start()
        {
            worldCamera = Camera.main;
        }
        
        public void PlayParticlesAtClickPosition(BaseEventData eventData)
        {
            if (particleSystem == null || worldCamera == null) return;
            
            Vector2 screenPos = Vector2.zero;
            if (eventData is PointerEventData pointerData)
            {
                screenPos = pointerData.position;
            }
            
            Vector3 worldPos = worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
            
            particleSystem.transform.position = worldPos;
            particleSystem.Play();
        }
    }
}