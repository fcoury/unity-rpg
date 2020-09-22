using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour
{
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Press()
    {
        BattleChar player = BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn];
        if (player.currentMP >= spellCost)
        {
            BattleManager.instance.magicMenu.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);
            player.currentMP -= spellCost;
        }
        else
        {
            BattleManager.instance.battleNotice.theText.text = "Not Enough MP!";
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.magicMenu.SetActive(false);
        }
    }
}
