using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    GameManager gm;

    #region HEALTHBAR
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject healthBarPrefab;
    public GameObject Instance;
    [SerializeField] float healthBarOffset = 2.2f;
    HealthBar healthBarScript;
    [SerializeField] public float health;
    Vector3 targetPosition;
    #endregion

    #region AUDIO
    [SerializeField] AudioClip takeDamageSound;
    [SerializeField, Range(0, 1)] private float takeDamageSoundVolume = 0.3f;
    [SerializeField] protected CooldownTimer damageSoundTimer;
    [SerializeField] private float damageSoundCooldownLength = 0.3f;
    #endregion

    private void Start() {
        gm = FindObjectOfType<GameManager>();
        health = gm.maxPlayerHealth; 

        // Instantiate health bar with canvas as parent
        canvas = GameObject.Find("Canvas");
        Instance = Instantiate(healthBarPrefab);
        Instance.name = gameObject.name + " Health Bar";
        Instance.transform.SetParent(canvas.transform, false);
        
        // Set HP bar values
        Instance.GetComponent<Slider>().minValue = 0;
        Instance.GetComponent<Slider>().maxValue = health;
        Instance.GetComponent<Slider>().value = health;
        
        healthBarScript = Instance.GetComponent<UnityEngine.UI.HealthBar>();
        healthBarScript.target = gameObject;
        healthBarScript.SetInitialHealth(health);

        damageSoundTimer.cooldownLength = damageSoundCooldownLength;
     
    }

    private void Update() {
       health = healthBarScript.slider.value;
    }

    void LateUpdate()
    {
        if (Instance == null)
            return;
        HealthBarFollowTarget();
    }


    void HealthBarFollowTarget() 
    {
        // Move healthbar to target
        targetPosition = new Vector3(transform.position.x, transform.position.y + healthBarOffset, transform.position.z);
        Instance.transform.position = targetPosition;
    }

    public void TakeDamage(float damageTaken)
    {
        healthBarScript.SetHealth(health -= damageTaken);
        if (damageSoundTimer.cooldownComplete)
        {
            AudioManager.Instance.PlayOneShot(takeDamageSound, takeDamageSoundVolume);
            damageSoundTimer.ResetCooldown();
        }
        
    }

    
}
