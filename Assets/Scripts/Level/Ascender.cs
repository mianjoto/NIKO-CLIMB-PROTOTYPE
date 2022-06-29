using UnityEngine;

public class Ascender : MonoBehaviour
{
    public GameManager gm;
    [SerializeField] CapsuleCollider2D playerCollider;
    [SerializeField] bool _isNearAscender;
    [SerializeField] bool _isLocked;
    [SerializeField] bool _isUsed;
    [SerializeField] AudioClip _lockedSound;
    [SerializeField] AudioClip ascendingSoundEffect;
    [SerializeField] AudioClip _usedSound;

    void Awake() {
        gm = FindObjectOfType<GameManager>();
        playerCollider = gm.Player.GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        _isNearAscender = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isNearAscender && Input.GetKeyDown(gm.interactKey)) {
            if (!_isLocked && !_isUsed)
            {   
                AudioManager.Instance.PlayOneShot(ascendingSoundEffect);
                gm.AscendFloor();
            }
            else if (_isLocked)
            {
                AudioManager.Instance.PlayOneShot(_lockedSound, volumeScale: 0.5f);
            }
            else if (_isUsed)
            {
                Debug.Log("playingSOund");
                AudioManager.Instance.PlayOneShot(_usedSound);
            }
            else {}
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<CapsuleCollider2D>().Equals(playerCollider)) {
            _isNearAscender = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.GetComponent<CapsuleCollider2D>().Equals(playerCollider)) {
            _isNearAscender = false;
        }
    }
}
