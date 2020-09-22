using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public GameObject UIScreen;
    public GameObject player;
    public GameObject gameMan;
    public GameObject audioMan;

    void Start()
    {
        if (DialogManager.instance == null) {
            DialogManager.instance = Instantiate(UIScreen.GetComponent<DialogManager>());
        }

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

        if (AudioManager.instance == null)
        {
            Instantiate(audioMan);
        }
    }

    void Update()
    {

    }
}
