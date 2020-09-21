using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsWindow : MonoBehaviour
{
    public CharStats playerStats;
    public Image playerImage;
    public Text nameText;
    public Text hpText;
    public Text mpText;
    public Text strengthText;
    public Text defenceText;
    public Text equippedWpnText;
    public Text wpnPower;
    public Text equippedArmrText;
    public Text armrPower;
    public Text expToNextLevel;
    public Button p1Btn;
    public Button p2Btn;
    public Button p3Btn;

    public static PlayerStatsWindow instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        CharStats[] playerStats = GameManager.instance.playerStats;
        p1Btn.gameObject.SetActive(playerStats[0].gameObject.activeInHierarchy);
        p2Btn.gameObject.SetActive(playerStats[1].gameObject.activeInHierarchy);
        p3Btn.gameObject.SetActive(playerStats[2].gameObject.activeInHierarchy);
        p1Btn.GetComponentInChildren<Text>().text = playerStats[0].charName;
        p2Btn.GetComponentInChildren<Text>().text = playerStats[1].charName;
        p3Btn.GetComponentInChildren<Text>().text = playerStats[2].charName;
    }

    public void ShowPlayerStats(int playerNumber)
    {
        CharStats stats = GameManager.instance.playerStats[playerNumber];

        playerImage.sprite = stats.charImage;
        nameText.text = stats.charName;
        hpText.text = "" + stats.currentHP + "/" + stats.maxHP;
        mpText.text = "" + stats.currentMP + "/" + stats.maxMP;
        strengthText.text = stats.strength.ToString();
        defenceText.text = stats.defence.ToString();
        equippedWpnText.text = stats.equippedWpn == "" ? "None" : stats.equippedWpn;
        wpnPower.text = stats.wpnPwr.ToString();
        equippedArmrText.text = stats.equippedArmr == "" ? "None" : stats.equippedArmr;
        armrPower.text = stats.armrPwr.ToString();
        expToNextLevel.text = (stats.expToNextLevel[stats.playerLevel] - stats.currentEXP).ToString();
    }
}
