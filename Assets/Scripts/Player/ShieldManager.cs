using System;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    GameManager gm;
    ShieldUIManager shieldUIManager;
    GameObject player;
    float shieldOffset = 1f;
    public event EventHandler OnShieldAbsorbHit;

    [SerializeField] AudioClip shieldHitSound;
    [SerializeField] AudioClip shieldDieSound;
    public bool canBlock=true;

    private void Awake() {
        gm = FindObjectOfType<GameManager>();
        player = FindObjectOfType<GameManager>().Player;
        shieldUIManager = GameObject.Find("Canvas").GetComponentInChildren<ShieldUIManager>();
    }

    private void Start() {
    }

    void Update()
    {
        FollowPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Projectile")
        {
            OnShieldAbsorbHit?.Invoke(this, EventArgs.Empty);
            bool hasAtLeastOneShield = shieldUIManager.findValidShieldIndex() == -1;
            Debug.Log("hasAtLeastOneShield=" + hasAtLeastOneShield);
            if (hasAtLeastOneShield)
            {
                AudioManager.Instance.PlayOneShot(shieldDieSound);
                this.gameObject.SetActive(false);
                Debug.Log("CANNOTBLOCK");
                canBlock = false;
                return;
            }
            canBlock = true;
            AudioManager.Instance.PlayOneShot(shieldHitSound);
        }
    }

    void FollowPlayer()
    {
        float playerLength = player.GetComponent<SpriteRenderer>().bounds.extents.x;
        float flipSign = 1;
        if (player.GetComponent<PlayerMovement>().facingLeft)
        {
            flipSign = -1;
        }
        Vector2 shieldPos = new Vector2(player.transform.position.x + flipSign * (playerLength/2 + shieldOffset), player.transform.position.y);
        this.transform.position = shieldPos;
    }

}
