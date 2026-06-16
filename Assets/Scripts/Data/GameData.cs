using System;
using System.Collections.Generic;

[System.Serializable]
public class GameDataBase
{
    public string _id;
}

[System.Serializable]
public class EnemyData : GameDataBase
{
    public string _Name;
    public string _displayName;
    public float _health;
    public float _moveSpeed;
    public float _stopDistance;
    public float _attackDamage;
    public float _attackInterval;
    public float _visualScale;
}