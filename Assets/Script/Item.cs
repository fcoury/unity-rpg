using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon;
    public bool isArmor;

    [Header("Item Details")]
    public string itemName;
    public string description;
    public int value;
    public Sprite itemSprite;

    [Header("Affected Details")]
    public int amountToChange;
    public bool affectHP, affectMP, affectStr;

    [Header("Weapon/Armor Details")]
    public int weaponStrength;
    public int armorStrength;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Use(int charToUseOn)
    {
        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];
        if (isItem)
        {
            if (affectHP)
            {
                selectedChar.currentHP += amountToChange;
                selectedChar.currentHP = Mathf.Min(selectedChar.currentHP, selectedChar.maxHP);
            }
            if (affectMP)
            {
                selectedChar.currentMP += amountToChange;
                selectedChar.currentMP = Mathf.Min(selectedChar.currentMP, selectedChar.maxMP);
            }
            if (affectStr)
            {
                selectedChar.strength += amountToChange;
            }
        }

        if (isWeapon)
        {
            if (selectedChar.equippedWpn != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedWpn);
            }

            selectedChar.equippedWpn = itemName;
            selectedChar.wpnPwr = weaponStrength;
        }

        if (isArmor)
        {
            if (selectedChar.equippedArmr != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedArmr);
            }

            selectedChar.equippedArmr = itemName;
            selectedChar.armrPwr = armorStrength;
        }

        GameManager.instance.RemoveItem(itemName);
    }
}
