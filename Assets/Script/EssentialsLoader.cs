using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public GameObject UIScreen;
    public GameObject player;
    public GameObject gameMan;

    void Start()
    {
        if (UIFade.instance == null)
        {
            UIFade.instance = Instantiate(UIScreen).GetComponent<UIFade>();
        }

        if (PlayerController.instance == null)
        {
            PlayerController clone = Instantiate(player).GetComponent<PlayerController>();
            PlayerController.instance = clone;
            PlayerController.instance.moveTo = GameObject.Find("Area Entrance").gameObject.transform.position;
        }

        if (GameManager.instance == null)
        {
            Instantiate(gameMan);
        }
    }

    void Update()
    {

    }
}
