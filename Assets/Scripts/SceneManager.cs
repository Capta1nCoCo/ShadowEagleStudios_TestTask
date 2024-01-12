using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject Lose;
    [SerializeField] private GameObject Win;

    private void Awake()
    {
        GameEvents.OnVictory += Victory;
        GameEvents.OnDefeat += GameOver;
    }

    private void OnDestroy()
    {
        GameEvents.OnVictory -= Victory;
        GameEvents.OnDefeat -= GameOver;
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    public void Victory()
    {
        Win.SetActive(true);
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}