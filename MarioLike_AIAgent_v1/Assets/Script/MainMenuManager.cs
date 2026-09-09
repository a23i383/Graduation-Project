using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        SceneManager.LoadScene("Stage1");
    }
}
