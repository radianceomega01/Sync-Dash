using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public void StartClicked()
    {
        GameManager.Instance.StartGame();
    }

    public void ExitClicked()
    {
        Application.Quit();
    }
}
