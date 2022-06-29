using UnityEngine;

public class PlaySoundOnTriggerEnter : MonoBehaviour
{
    [SerializeField] CapsuleCollider2D _playerCollider;
    [SerializeField, Tooltip("The specific audiosource for this object")]
    AudioSource _audioSource; 
    [SerializeField, Tooltip("Whether to stop the sound on exit of collision")]
    bool stopOnExit;
    [SerializeField, Tooltip("Whether to mute the sound on exit of collision")]
    bool muteOnExit;

    private void Awake() {
        _audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<CapsuleCollider2D>().Equals(_playerCollider))
        {
            _audioSource.mute = false;
            _audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<CapsuleCollider2D>().Equals(_playerCollider))
        {
            if (stopOnExit)
            {
                _audioSource.Stop();
            }
            if (muteOnExit)
            {
                _audioSource.mute = true;
            }
        }
    }

}
