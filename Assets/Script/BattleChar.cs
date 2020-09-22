using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour
{
    public bool isPlayer;
    public string[] movesAvailable;

    public string charName;
    public int currentHP, maxHP, currentMP, maxMP, strength, defence, wpnPower, armPower;
    public bool hasDied;

    public SpriteRenderer theSprite;
    public Sprite deadSprite, aliveSprite;

    void Start()
    {

    }

    void Update()
    {

    }
}
