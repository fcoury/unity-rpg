﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Image buttonImage;
    public Text amountText;
    public int buttonValue;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Press()
    {
        if (GameMenu.instance.theMenu.activeInHierarchy)
        {
            string heldItem = GameManager.instance.itemsHeld[buttonValue];
            if (heldItem != "")
            {
                GameMenu.instance.SelectItem(GameManager.instance.GetItemDetails(heldItem));
            }
        }

        if (Shop.instance.shopMenu.activeInHierarchy)
        {
            if (Shop.instance.buyMenu.activeInHierarchy)
            {
                Shop.instance.SelectBuyItem(GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[buttonValue]));
            }

            if (Shop.instance.sellMenu.activeInHierarchy)
            {
                Shop.instance.SelectSellItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonValue]));
            }
        }
    }
}
