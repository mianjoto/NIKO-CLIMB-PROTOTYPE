using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public KeyCode punchKey = KeyCode.Mouse0; 
    public Animator animator;
    [SerializeField] protected CooldownTimer punchTimer;
    public LayerMask enemyLayers;
    private Vector2 playerVelocity;
    private float forceMultiplier = 5f;
    [SerializeField] float punchDamage;
    public float punchCooldown = 1f, punchRange = 1f, punchForce = 10f;
    [SerializeField] AudioClip punchImpactSound;

    GameManager gm;

    // Hitboxes and shield
    public Transform punchHitbox;
    public GameObject Shield;

    private void Awake() {
        gm = FindObjectOfType<GameManager>();    
    }

    void Start() {
        Shield.SetActive(false);
    }


    void Update() {
        if (!PauseMenu.isPaused)
        {
            // Check for punches
            if (Input.GetKeyDown(punchKey) && punchTimer.cooldownComplete)
            {
                // Play an attack animation
                animator.SetTrigger("Punch");
                punchTimer.ResetCooldown();

                // Punch() will run on the animation event when the punch touches the enemy.
            }
            if (Input.GetKeyDown(gm.blockKey) && !isPlaying(animator, "Punch")) // Only block if not punching
            {
                Block();
            }
            if (Input.GetKeyUp(gm.blockKey))
            {
                Shield.SetActive(false);
            }
        }

    }    
    
    void Punch()
    {
        // Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchHitbox.position, punchRange, enemyLayers);
        if (hitEnemies.Length != 0)
        {
            AudioManager.Instance.PlayOneShot(punchImpactSound);
        }

        // Push and damage enemies
        playerVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        playerVelocity = new Vector2(playerVelocity.x*0.5f, playerVelocity.y*0.5f); // halve the velocity of the player
        foreach (Collider2D enemy in hitEnemies) {
            // damage enemy
            // move enemy
            Rigidbody2D enemyrb = enemy.GetComponent<Rigidbody2D>();
            enemyrb.velocity = new Vector2(playerVelocity.x*punchForce, playerVelocity.y*punchForce);
            enemy.GetComponent<HealthManager>().TakeDamage(gm.punchDamage);
        }



    }


    void Block()
    {
        Debug.Log(Shield.GetComponent<ShieldManager>().canBlock);
        if (Shield.GetComponent<ShieldManager>().canBlock)
        {
            Debug.Log("ACTIVATED SHIELD");
            Shield.SetActive(true);
        }
    }

    bool isPlaying(Animator anim, string stateName)
    {
    int baseLayer = 0;
    if (anim.GetCurrentAnimatorStateInfo(baseLayer).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(baseLayer).normalizedTime < 1.0f)
        return true;
    else
        return false;
    }
}


