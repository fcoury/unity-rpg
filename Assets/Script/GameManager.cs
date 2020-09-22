using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharStats[] playerStats;
    public bool gameMenuOpen, dialogActive, fadingBetweenAreas, shopActive, battleActive;

    public string[] itemsHeld;
    public int[] numberOfItems;
    public Item[] referenceItems;

    public int currentGold;

    void Start()
    {
        if (instance == null) instance = this;

        DontDestroyOnLoad(gameObject);
        SortItems();
    }

    void Update()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive)
        {
            PlayerController.instance.canMove = false;
        }
        else {
            PlayerController.instance.canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AddItem("Iron Armor");
            AddItem("Blabla");
            RemoveItem("Health Potion");
            RemoveItem("XXX");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadData();
        }
    }

    public Item GetItemDetails(string itemToGrab)
    {
        for (int i = 0; i < referenceItems.Length; i++)
        {
            if (referenceItems[i].itemName == itemToGrab)
            {
                return referenceItems[i];
            }
        }

        return null;
    }

    public void SortItems()
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace)
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Length-1; i++)
            {
                if (itemsHeld[i] == "")
                {
                    itemsHeld[i] = itemsHeld[i+1];
                    itemsHeld[i+1] = "";

                    numberOfItems[i] = numberOfItems[i+1];
                    numberOfItems[i+1] = 0;

                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemToAdd)
    {
        int newItemPosition = 0;
        bool foundSpace = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == "" || itemToAdd == itemsHeld[i])
            {
                newItemPosition = i;
                foundSpace = true;
                break;
            }
        }

        if (foundSpace)
        {
            bool itemExists = false;
            for (int i = 0; i < referenceItems.Length; i++)
            {
                if (referenceItems[i].itemName == itemToAdd)
                {
                    itemExists = true;
                    break;
                }
            }
            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;
            }
            else
            {
                Debug.LogError("Attempt to add inexistent item " + itemToAdd);
            }
        }

        GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemToRemove)
    {
        bool foundItem = false;
        int itemPosition = 0;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == itemToRemove)
            {
                foundItem = true;
                itemPosition = i;
                break;
            }
        }

        if (foundItem)
        {
            numberOfItems[itemPosition]--;
            if (numberOfItems[itemPosition] <= 0)
            {
                itemsHeld[itemPosition] = "";
            }

            GameMenu.instance.ShowItems();
        }
        else
        {
            Debug.LogError("Attempt to remove inexistent item " + itemToRemove);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.gameObject.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.gameObject.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.gameObject.transform.position.z);

        for (int i = 0; i < playerStats.Length; i++)
        {
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", playerStats[i].gameObject.activeInHierarchy ? 1 : 0);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_playerLevel", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentEXP", playerStats[i].currentEXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_maxLevel", playerStats[i].maxLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_baseEXP", playerStats[i].baseEXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_maxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentMP", playerStats[i].currentMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_maxMP", playerStats[i].maxMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_strength", playerStats[i].strength);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_defence", playerStats[i].defence);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_wpnPwr", playerStats[i].wpnPwr);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_armrPwr", playerStats[i].armrPwr);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_equippedWpn", playerStats[i].equippedWpn);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_equippedArmr", playerStats[i].equippedArmr);
        }

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
        }
    }

    public void LoadData()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene"));
        PlayerController.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Position_x"), PlayerPrefs.GetFloat("Player_Position_y"), PlayerPrefs.GetFloat("Player_Position_z")
        );

        for (int i = 0; i < playerStats.Length; i++)
        {
            playerStats[i].gameObject.SetActive(PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 1);
            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_playerLevel");
            playerStats[i].currentEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentEXP");
            playerStats[i].maxLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_maxLevel");
            playerStats[i].baseEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_baseEXP");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_maxHP");
            playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentMP");
            playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_maxMP");
            playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_strength");
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_defence");
            playerStats[i].wpnPwr = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_wpnPwr");
            playerStats[i].armrPwr = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_armrPwr");
            playerStats[i].equippedWpn = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_equippedWpn");
            playerStats[i].equippedArmr = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_equippedArmr");
        }

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
        }
    }

}
