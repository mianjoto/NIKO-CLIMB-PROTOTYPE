using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    GameManager _gm;
    HealthManager _hm;
    float _playerHealth;
    public event System.EventHandler OnPlayerDeath; 

    private void Start()
    {
        _gm = FindObjectOfType<GameManager>();
        _hm = GetComponent<HealthManager>();
    }

    void Update()
    {
        _playerHealth = _hm.health;
        if (_playerHealth == 0)
        {
            OnPlayerDeath?.Invoke(this, System.EventArgs.Empty);
        }
    }
    
}
