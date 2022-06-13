using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	[SerializeField] private GameObject control;
	[SerializeField] private GameObject exitMenu;
	[SerializeField] private string sceneName = "";

	private Teleporter teleporter;

    public string SceneName { get => sceneName;}

    private void Awake()
    {
		var blackFilter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag);
		if (blackFilter != null)
			teleporter = blackFilter.GetComponent<Teleporter>();
	}

    public void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void ChangeScene()
	{
		SceneManager.LoadScene(sceneName);
	}

	public void GoToTutorialLevelOrVillage()
    {
		var playButton = GetComponent<PlayButton>();
		if (playButton != null && playButton.ForcedTutorial)
        {
			ServiceInfo.TutorialDone = false;
			LoadTutorial();
			return;
		}

		var tutorialDone = bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone, "false"));
		ServiceInfo.TutorialDone = tutorialDone;

		if (tutorialDone)
			teleporter.Go(() => SceneManager.LoadScene(ServiceInfo.VillageScene), 0.7f);
		else
			LoadTutorial();
    }

	public void Exit()
	{
		Application.Quit();
	}

	private void LoadTutorial()
    {
		PlayerPrefs.DeleteAll();
		teleporter.Go(() => SceneManager.LoadScene(ServiceInfo.TutorialLevel), 0.7f);
	}

	public void Control()
    {
		control.SetActive(true);
    }

	public void ExitMenu()
    {
		exitMenu.SetActive(true);
    }

	public void ControlButton()
	{
		control.SetActive(false);
	}

	public void ExitMenuReturn()
	{
		exitMenu.SetActive(false);
	}
}
