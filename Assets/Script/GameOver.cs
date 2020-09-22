using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public string mainMenuScene;
    public string loadGameScene;

    private bool firstRun = true;

    void Start()
    {
        AudioManager.instance.PlayBGM(4);

        DeactivateAll();
    }

    void Update()
    {
        if (firstRun)
        {
            DeactivateAll();
            firstRun = false;
        }
    }

    private void DeactivateAll()
    {
        if (PlayerController.instance) PlayerController.instance.gameObject.SetActive(false);
        if (GameMenu.instance) GameMenu.instance.gameObject.SetActive(false);
        if (BattleManager.instance) BattleManager.instance.gameObject.SetActive(false);
    }

    private void CleanupAndLoadScene(string sceneToLoad)
    {
        if (GameManager.instance) Destroy(GameManager.instance.gameObject);
        if (PlayerController.instance) Destroy(PlayerController.instance.gameObject);
        if (GameMenu.instance) Destroy(GameMenu.instance.gameObject);
        if (BattleManager.instance) Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitToMain()
    {
        Destroy(AudioManager.instance.gameObject);
        CleanupAndLoadScene(mainMenuScene);
    }

    public void LoadLastSave()
    {
        CleanupAndLoadScene(loadGameScene);
    }
}
