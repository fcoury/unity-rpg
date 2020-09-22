﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string[] dialogLines;
    public int currentLine;

    public static DialogManager instance;

    private bool justStarted;
    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (dialogBox.activeInHierarchy)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (!justStarted)
                {
                    currentLine++;
                    if (currentLine >= dialogLines.Length)
                    {
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogActive = false;
                        if (shouldMarkQuest)
                        {
                            shouldMarkQuest = false;
                            if (markQuestComplete)
                            {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            }
                            else
                            {
                                QuestManager.instance.MarkQuestIncomplete(questToMark);
                            }
                        }
                    }
                    else
                    {
                        CheckIfName();
                        dialogText.text = dialogLines[currentLine];
                    }
                }
                else
                {
                    justStarted = false;
                }
            }
        }
    }

    public void ShowDialog(string[] newLines, bool isPerson)
    {
        dialogLines = newLines;
        currentLine = 0;
        CheckIfName();

        dialogText.text = dialogLines[currentLine];
        dialogBox.SetActive(true);
        nameBox.SetActive(isPerson);

        justStarted = true;
        GameManager.instance.dialogActive = true;
    }

    public void CheckIfName()
    {
        if (dialogLines[currentLine].StartsWith("n-"))
        {
            nameText.text = dialogLines[currentLine].Substring(2);
            currentLine++;
        }
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markAsComplete)
    {
        questToMark = questName;
        markQuestComplete = markAsComplete;

        shouldMarkQuest = true;
    }
}
