using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour 
{
	public static string currentTab;

	protected static AudioSource audioSource;
	protected static string previousView, nextView;

	public void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
         
	public void OnDisable()
	{
	    SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
	    if (GetViewName() != "Login" && 
	    	GetViewName() != "Register" &&
	    	GetViewName() != "Recover" &&
	    	GetViewName() != "Splash")
			ShowFooterMenu();
		else
			DestroyFooterMenu();
	}

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			PlaySound("click_2");
			LoadPreviousView();
			PlaySound("click_2");
		}
	}

	public void LoadNextView()
	{
		if (nextView != null)
			LoadView(nextView);
	}

	public void LoadPreviousView()
	{
		if (previousView != null)
			LoadView(previousView);
	}

	public void LoadView(string Scene) 
	{
		if (Scene != null) 
			SceneManager.LoadScene(Scene);
		else
			Application.Quit();
	}

	public void ReloadView()
	{
        SceneManager.LoadScene(GetViewName());
	}

	public string GetViewName()
	{
		Scene scene = SceneManager.GetActiveScene();
		return scene.name;
	}

	public void PlaySound(string name)
	{
		string path = "Sounds/" + name;
		AudioClip clip = (AudioClip) Resources.Load(path);

		if (clip == null)
			return;

		GameObject[] instances = GameObject.FindGameObjectsWithTag("Audio");
		if (instances.Length < 1)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.gameObject.tag = "Audio";
			audioSource.transform.SetParent(null);

			DontDestroyOnLoad(audioSource);
			return;
		}

		audioSource.clip = clip;
        audioSource.Play();
	}

	public void OpenModal (string modalName)
	{
		PlaySound("click_2");
		
		string modalPath = "Prefabs/Modal" + modalName;

        GameObject modalPrefab = (GameObject) Resources.Load(modalPath),
                   modalInstance = (GameObject) GameObject.Instantiate(modalPrefab, Vector3.zero, Quaternion.identity);
        
        modalInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        modalInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
	}

	private void ShowFooterMenu()
	{
		GameObject[] instances = GameObject.FindGameObjectsWithTag("Footer");
		if (instances.Length > 0)
			return;

		GameObject footerPrefab = (GameObject) Resources.Load("Prefabs/Footer Menu"),
                   footerInstance = (GameObject) GameObject.Instantiate(footerPrefab, Vector3.zero, Quaternion.identity);
        
        footerInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        footerInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
	}

	private void DestroyFooterMenu()
	{
		GameObject[] instances = GameObject.FindGameObjectsWithTag("Footer");
		if (instances.Length > 0)
			Destroy(instances[0]);
	}
}


