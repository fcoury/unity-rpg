using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    public string areaToLoad;
    public string areaTransitionName;
    public AreaEntrance theEntrance;
    public float waitToLoad = 1f;

    private bool shouldLoadAfterFade;

    void Start()
    {
        theEntrance.transitionName = areaTransitionName;
    }

    void Update()
    {
        if (shouldLoadAfterFade)
        {
            waitToLoad -= Time.deltaTime;
            if (waitToLoad <= 0)
            {
                shouldLoadAfterFade = false;
                SceneManager.LoadScene(areaToLoad);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by " + other);
        if (other.CompareTag("Player"))
        {
            shouldLoadAfterFade = true;
            GameManager.instance.fadingBetweenAreas = true;
            UIFade.instance.FadeToBlack();

            PlayerController.instance.areaTransitionName = areaTransitionName;
        }
    }
}
