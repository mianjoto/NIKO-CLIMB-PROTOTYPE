using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject player, projectile, skullPrefab;
    GameManager gm;
    [SerializeField] HealthManager healthBar;

    public float shootingRange;
    public float lineOfSight;
    float nextFireTime;
    float distanceFromPlayer;
    bool facingRight;


    public event EventHandler<OnEnemyDiedArgs> OnEnemyDied;
    [SerializeField] AudioClip enemyDeathSound;

    #region TEST VARS
    public EnemyState state;
    public bool _inLOS;
    public bool _inShootingRange;
    #endregion


    ProjectileEmitter emitter;
    Vector2 playerPosition;
    Vector2 enemyPosition;

    [SerializeField]
    float moveSpeed = 1f, fireRate = 1f;


    void Awake() {
        nextFireTime = Time.time;
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        if (!player) player = GameObject.FindWithTag("Player");
        emitter = FindObjectOfType<ProjectileEmitter>();
        state = EnemyState.Patrol;
    }


    void FixedUpdate()
    {
        if (gm.playerIsDead) {return;}
        state = GetState();
        switch (state)
        {
            case EnemyState.Die:
                Die();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Move:
                MoveToPlayer();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
        }

        // Face the player
        if (enemyPosition.x < playerPosition.x && !facingRight) {
            transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        } else if (enemyPosition.x > playerPosition.x && facingRight) {
            transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }

    EnemyState GetState()
    {
        if (healthBar.health <= 0) return EnemyState.Die;

        playerPosition = player.transform.position;
        enemyPosition = transform.position;
        _inShootingRange = inShootingRange();
        _inLOS = inLOS();
        if (inShootingRange())
        {
            return EnemyState.Attack;
        }
        else if (inLOS() && !inShootingRange())
        {
            return EnemyState.Move;
        }
        else
        {
            return EnemyState.Patrol;
        }
        
    }

    void Die()
    {
        //Stop shooting
        CancelInvoke("Shoot");
        AudioManager.Instance.PlayOneShot(enemyDeathSound, volumeScale: 0.5f);
        // Drop a skull behind
        float skullOffset = 0.5f;
        Vector2 skullPosition = new Vector2(transform.position.x, transform.position.y + skullOffset);
        GameObject skullInstance = Instantiate(skullPrefab, skullPosition, Quaternion.identity);
        OnEnemyDied?.Invoke(this, new OnEnemyDiedArgs(this.gameObject));
        Destroy(healthBar.Instance); // Delete the enemy's health bar
        Destroy(healthBar); // TODO REDUNDANT Delete the enemy's health bar
        Destroy(gameObject); 
        
    }

    void Patrol()
    {
        // Stop shooting
        CancelInvoke("Shoot");
        // TODO patrol logic here
    }

    void MoveToPlayer()
    {
        // transform.position = Vector2.MoveTowards(new Vector2(enemyPosition.x, enemyPosition.y), new Vector2(playerPosition.x, enemyPosition.y), moveSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(enemyPosition, playerPosition, moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        if (emitter == null)
            return;
        if (nextFireTime < Time.time) 
        {  
            emitter.Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }



    bool inShootingRange() {
        distanceFromPlayer = Vector2.Distance(playerPosition, enemyPosition);
        return distanceFromPlayer < shootingRange;
    }

    bool inLOS() {
        distanceFromPlayer = Vector2.Distance(playerPosition, enemyPosition);
        return distanceFromPlayer < lineOfSight;
    }

    
}

public enum EnemyState
{
    Patrol,
    Attack,
    Move,
    Die
}