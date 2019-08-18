using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminToggleService : MonoBehaviour 
{
	private int CLICKS_COUNT = 0;

	private bool debugActivated = false;
	private string debugConsoleName = "IngameDebugConsole";

	public void Toggle ()
	{
		CLICKS_COUNT += 1;

		if (CLICKS_COUNT >= 10)
		{
			ToggleDebugConsole();
			CLICKS_COUNT = 0;
		}
	}

	#pragma warning disable 0219
	private void ToggleDebugConsole ()
	{
		GameObject debugConsole = GameObject.Find(debugConsoleName);
		debugActivated = !debugActivated;

		if (debugConsole == null)
		{
			string debugPath = "Prefabs/" + debugConsoleName;

	        GameObject debugPrefab = (GameObject) Resources.Load(debugPath),
	                   debugInstance = (GameObject) GameObject.Instantiate(debugPrefab, Vector3.zero, Quaternion.identity);
	        
	        AlertsService.makeAlert("Debug ativado", "Uma instância do monitor de logs foi criada ao lado.", "OK");
		}
		else
		{
			AlertsService.makeAlert("Debug desativado", "Você não verá mais o monitor de logs.", "OK");
			Destroy(debugConsole);
		}

	}
}
