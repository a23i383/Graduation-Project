using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    private bool pauseFlag = false;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        mainMenuButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();

        mainMenuButton.onClick.AddListener(OnMainMenu);
        restartButton.onClick.AddListener(OnRestart);
        continueButton.onClick.AddListener(OnContinue);
        settingButton.onClick.AddListener(OnSetting);
    }

    private void Update()
    {
        if (!pauseFlag)
        {
            return;
        }
    }
    private void OnMainMenu()
    {
        Debug.Log("メインメニューに戻る");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
    private void OnRestart()
    {
        Debug.Log("リスタートする");
        Destroy(RespawnManager.instance);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnContinue()
    {
        Debug.Log("コンティニューする");
        pauseFlag = !pauseFlag;
        anim.SetTrigger("pauseClose");
        Time.timeScale = 1.0f;
    }
    private void OnSetting()
    {
        Debug.Log("設定を開く");
    }
    public void TogglePause()
    {
        pauseFlag = !pauseFlag;

        if (pauseFlag)
        {
            anim.SetTrigger("pauseOpen");
            Time.timeScale = 0.0f;
        }
        else
        {
            anim.SetTrigger("pauseClose");
            Time.timeScale = 1.0f;
        }
    }
}
