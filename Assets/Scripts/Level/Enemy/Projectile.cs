using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 startingPosition;
    Vector2 targetPosition;
    Vector2 velocity;
    public GameObject player;
    CapsuleCollider2D playerCollider;
    GameManager gm;
    float projectileDamage;

    BoxCollider2D hitbox;
    Rigidbody2D rb;

    [SerializeField]
    float projectileSpeed = 3f;



    // Start is called before the first frame update
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        hitbox = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        projectileDamage = gm.enemyBulletDamage;

    }

    private void Start() {
        rb.velocity = -transform.right * projectileSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gm.playerIsDead) { return; }
        if (other.gameObject.GetComponent<CapsuleCollider2D>().Equals(playerCollider))
        {
            player.GetComponent<HealthManager>().TakeDamage(projectileDamage);
        }

    // TODO EVERYTHING HERE LOL
        // Ignore collision if colliding with the enemy that sent the bullet or if collides with ascender
        if (!other.Equals(GameObject.FindWithTag("Enemy").GetComponent<CapsuleCollider2D>()) &&
            !other.Equals(GameObject.FindWithTag("Ascender")))
        {
            Destroy(gameObject);
        }

    }
}
