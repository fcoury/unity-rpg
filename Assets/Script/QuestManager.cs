using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    void Start()
    {
        instance = this;
        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Marking Complete");
            MarkQuestComplete("Quest Test");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveQuestData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadQuestData();
        }
    }

    public int GetQuestNumber(string questName)
    {
        int res = System.Array.IndexOf(questMarkerNames, questName);
        if (res <= 0)
        {
            Debug.LogError("Quest number for invalid quest " + questName);
        }
        return res;
    }

    public bool CheckIfComplete(string questToCheck)
    {
        return questMarkersComplete[GetQuestNumber(questToCheck)];
    }

    public void MarkQuestComplete(string questToMark)
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = true;
        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark)
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = false;
        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects()
    {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();
        for (int i = 0; i < questObjects.Length; i++)
        {
            questObjects[i].CheckCompletion();
        }
    }

    public void SaveQuestData()
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], questMarkersComplete[i] ? 1 : 0);
        }
    }

    public void LoadQuestData()
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            string key = "QuestMarker_" + questMarkerNames[i];
            questMarkersComplete[i] = PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;
        }
    }
}
