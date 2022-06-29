using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    Vector2 _targetPosition;
    GameManager gm;
    ShieldUIManager shieldUIManager;
    LevelManager lm;
    [SerializeField] float amplitude = 30000;
    [SerializeField] float period = 5;

    private void Start() {
        gm = FindObjectOfType<GameManager>();
        lm = gm.GetComponent<LevelManager>();
        shieldUIManager = GameObject.Find("Canvas").GetComponentInChildren  <ShieldUIManager>();
    }
    // Update is called once per frame
    void Update()
    {
        float yOffset = (1f/amplitude) * Mathf.Sin(Time.realtimeSinceStartup*period);
        _targetPosition = new Vector2(transform.position.x, transform.position.y + yOffset*400); 
        transform.position = _targetPosition;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.Equals(gm.Player.GetComponent<CapsuleCollider2D>()))
        {
            if (gameObject.tag == "Key")
            {
                KeyPowerup();
            } else if (gameObject.tag == "ExtraShield")
            {
                ShieldPowerUp();
            }
            Destroy(this.gameObject);
        }
    }

    void KeyPowerup()
    {
        lm.unlockAscender();
        
    }
    
    void ShieldPowerUp()
    {
        shieldUIManager.addShield();
    }
}
