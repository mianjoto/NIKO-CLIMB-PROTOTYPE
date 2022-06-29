using System;
using UnityEngine;

public class OnEnemyDiedArgs : EventArgs {
    private readonly GameObject _enemy;

    public OnEnemyDiedArgs(GameObject enemy)
    {
        _enemy = enemy;
    }

    public GameObject enemyObject
    {
        get { return _enemy; }
    }
}