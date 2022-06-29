using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    GameManager gm;
    HealthManager hm;
    bool isBurning;

    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        hm = gm.Player.GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.playerIsDead)
        {
            transform.Translate(Vector2.up * Time.deltaTime * gm.fireClimbSpeed, Space.World);
            if (isBurning)
            {
                hm.TakeDamage(gm.fireTickDamage);
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<CapsuleCollider2D>().Equals(gm.Player.GetComponent<CapsuleCollider2D>()))
        {
            isBurning = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<CapsuleCollider2D>().Equals(gm.Player.GetComponent<CapsuleCollider2D>()))
        {
            isBurning = false;
        }
    }


}
