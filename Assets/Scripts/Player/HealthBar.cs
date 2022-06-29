using DG.Tweening;

namespace UnityEngine.UI {
    
    public class HealthBar : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject target;
        private GameObject healthBarInstance;
        [SerializeField] float healthBarOffset;
        [SerializeField] float loseHealthTime = 1f;
        public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        private float animationPercentageCutoff = 30;
        public float newHealth;
        private float currentHealth;
        public Slider slider;
        private Image healthFill;
        public Gradient gradient;

        

        private void Awake() {
            slider = GetComponent<Slider>();

            // Sets the fill color to a gradient. I learned this from this tutorial: https://medium.com/nerd-for-tech/adding-a-player-health-bar-d59d629c1311
            healthFill = GetComponentsInChildren<Image>()[1];
                
        }

        private void Update() {
            AnimateHealth();
    
        }

        void AnimateHealth()
        {
            slider.DOValue(newHealth, loseHealthTime, snapping: false).SetEase(Ease.InQuad);
            healthFill.color = gradient.Evaluate(slider.normalizedValue);
        }
        
        public void SetInitialHealth(float maxHealth)
        {   
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            newHealth = maxHealth;
            healthFill.color = gradient.Evaluate(1f);
        }

        public void SetHealth(float newHealth)
        {
            this.newHealth = newHealth;
        }

    }

}
