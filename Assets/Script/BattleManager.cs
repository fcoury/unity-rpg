﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    private bool battleActive;
    public GameObject battleScene;
    public Transform[] playerPositions;
    public Transform[] enemyPositions;
    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public GameObject uiButtonsHolder;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;
    public DamageNumber damageNumber;

    public Text[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;
    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    public BattleNotification battleNotice;

    public GameObject itemsMenu;
    public ItemButton[] itemButtons;
    public Item itemToUse;
    public Text itemName;
    public Text itemDescription;

    private bool fleeing;
    public int chanceToFlee = 35;

    public string gameOverScene;

    public int rewardXP;
    public string[] rewardItems;
    public bool cannotFlee;

    public static BattleManager instance;

    void Start()
    {
        Debug.Log("RewardXP: " + rewardXP + " Reward Items: " + rewardItems);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Eyeball" }, false);
        }

        if (battleActive)
        {
            if (turnWaiting)
            {
                if (activeBattlers[currentTurn].isPlayer)
                {
                    uiButtonsHolder.SetActive(true);
                }
                else
                {
                    uiButtonsHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCo());
                }
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee)
    {
        if (!battleActive)
        {
            cannotFlee = setCannotFlee;
            battleActive = true;
            GameManager.instance.battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);
            AudioManager.instance.PlayBGM(0);

            for (int i = 0; i < playerPositions.Length; i++)
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < playerPrefabs.Length; j++)
                    {
                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName)
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);

                            CharStats thePlayer = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].wpnPower = thePlayer.wpnPwr;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].armPower = thePlayer.armrPwr;
                        }
                    }
                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }

            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count);
            UpdateUIStats();
        }
    }

    public void NextTurn()
    {
        currentTurn++;
        if (currentTurn >= activeBattlers.Count) currentTurn = 0;

        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            Debug.Log("S - Battler: " + activeBattlers[i] + " " + activeBattlers[i].currentHP + " Enemies Dead: " + allEnemiesDead + " Players Dead: " + allPlayersDead);

            if (activeBattlers[i].currentHP < 0)
            {
                activeBattlers[i].currentHP = 0;
            }

            if (activeBattlers[i].currentHP == 0)
            {
                 if (activeBattlers[i].isPlayer)
                 {
                     activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
                 }
                 else
                 {
                    activeBattlers[i].EnemyFade();
                 }
            }
            else
            {
                if (activeBattlers[i].isPlayer)
                {
                    allPlayersDead = false;
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSprite;
                }
                else
                {
                    allEnemiesDead = false;
                }
            }

            Debug.Log("E - Battler: " + activeBattlers[i] + " " + activeBattlers[i].currentHP + " Enemies Dead: " + allEnemiesDead + " Players Dead: " + allPlayersDead);
        }

        if (allEnemiesDead || allPlayersDead)
        {
            if (allEnemiesDead)
            {
                // end battle in victory
                StartCoroutine(EndBattleCo());
            }
            else
            {
                // end in failure
                StartCoroutine(GameOverCo());
            }

            // battleScene.SetActive(false);
            // GameManager.instance.battleActive = false;
            // battleActive = false;
        }
        else
        {
            while (activeBattlers[currentTurn].currentHP == 0)
            {
                currentTurn++;
                if (currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()
    {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
            {
                players.Add(i);
            }
        }

        int selectedIndex = Random.Range(0, players.Count);
        Debug.Log("EnemyAttack - Selected Target " + selectedIndex + " Count:" + players.Count);
        int selectedTarget = players[selectedIndex];
        // activeBattlers[selectedTarget].currentHP -= 30;
        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack])
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)
    {
        float atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].wpnPower;
        float defPwr = activeBattlers[target].defence + activeBattlers[target].armPower;

        Debug.Log("atkPwr " + atkPwr + " defPwr " + defPwr + " movePower " + movePower);
        float damageCalc = (atkPwr / defPwr) * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);

        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToGive + ") damage to " + activeBattlers[target].charName);

        activeBattlers[target].currentHP -= damageToGive;

        Vector3 position = activeBattlers[target].transform.position;
        Instantiate(damageNumber, position, activeBattlers[target].transform.rotation).SetDamage(damageToGive);

        UpdateUIStats();
    }

    public void UpdateUIStats()
    {
        for (int i = 0; i < playerName.Length; i++)
        {
            if (activeBattlers.Count > i)
            {
                if (activeBattlers[i].isPlayer)
                {
                    BattleChar playerData = activeBattlers[i];
                    playerName[i].gameObject.SetActive(true);

                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Max(playerData.currentHP, 0) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Max(playerData.currentMP, 0) + "/" + playerData.maxMP;
                }
                else
                {
                    playerName[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerName[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == moveName)
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        DealDamage(selectedTarget, movePower);

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        Debug.Log("Open Target " + moveName + " " + activeBattlers.Count);
        targetMenu.SetActive(true);
        List<int> enemies = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (!activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
            {
                enemies.Add(i);
            }
        }

        Debug.Log("Enemies " + enemies.Count);
        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (enemies.Count > i)
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = enemies[i];
                targetButtons[i].targetName.text = activeBattlers[enemies[i]].charName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i)
            {
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = activeBattlers[currentTurn].movesAvailable[i];

                for (int j = 0; j < movesList.Length; j++)
                {
                    if (movesList[j].moveName == magicButtons[i].spellName)
                    {
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            }
            else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenItemsMenu()
    {
        BattleChar player = activeBattlers[currentTurn];

        if (!player.isPlayer) {
            return;
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (i < GameManager.instance.itemsHeld[i].Length)
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
        itemsMenu.SetActive(true);
    }

    public void SelectItem(Item selectedItem)
    {
        itemToUse = selectedItem;
        itemName.text = itemToUse.itemName;
        itemDescription.text = itemToUse.description;
    }

    public void UseItem()
    {
        Debug.Log("Use item " + itemToUse);
        int charToUseOn = -1;
        CharStats[] playerStats = GameManager.instance.playerStats;
        BattleChar player = activeBattlers[currentTurn];

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].charName == player.charName)
            {
                charToUseOn = i;
                break;
            }
        }

        Debug.Log("Use item " + itemToUse + " -> " + charToUseOn);
        itemToUse.Use(charToUseOn);
        CloseItemsMenu();

        NextTurn();
        battleNotice.theText.text = player.charName + " used " + itemToUse.itemName + "!";
        battleNotice.Activate();
    }

    public void CloseItemsMenu()
    {
        itemsMenu.SetActive(false);
    }

    public void Flee()
    {
        if (cannotFlee)
        {
            battleNotice.theText.text = "Cannot flee this battle!";
            battleNotice.Activate();
        }
        else
        {
            int fleeSuccess = Random.Range(0, 100);
            if (fleeSuccess < chanceToFlee)
            {
                // battleActive = false;
                // battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            }
            else
            {
                NextTurn();
                battleNotice.theText.text = "Couldn't escape!";
                battleNotice.Activate();
            }
        }
    }

    public IEnumerator EndBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemsMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            BattleChar battler = activeBattlers[i];

            if (battler.isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    CharStats playerStats = GameManager.instance.playerStats[j];
                    if (battler.charName == playerStats.charName)
                    {
                        playerStats.currentHP = battler.currentHP;
                        playerStats.currentMP = battler.currentMP;
                    }
                }
            }

            Destroy(battler.gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;
        if (fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
        }
        else
        {
            Debug.Log("RewardXP: " + rewardXP + " Reward Items: " + rewardItems);
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardItems);
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

    public IEnumerator GameOverCo()
    {
        battleActive = false;
        GameManager.instance.battleActive = false;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);

        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}
