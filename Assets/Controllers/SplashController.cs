using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashController : ScreenController
{
	public GameObject loadingHolder;
	public Button reloadButton;
	private string IN_MAINTENANCE = "true";

	public void Start ()
	{
		// Use Play Sound to cache this sound first
		PlaySound("click_1");

		loadingHolder.SetActive(true);
		reloadButton.gameObject.SetActive(false);

		StartCoroutine(_CheckMaintenance());
	}

	private IEnumerator _CheckMaintenance ()
	{
		WWW checkRequest = SystemService.CheckMaintenance();
		float timeLoading = 0f;

		while (!checkRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		if (timeLoading < 3.0f)
		{
			float remainingTime = 3.0f - timeLoading;
			yield return new WaitForSeconds(remainingTime);
		}

		Debug.Log("Header: " + checkRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + checkRequest.text);

		if (checkRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			if (checkRequest.text == IN_MAINTENANCE)
			{
				string message = string.Format("O {0} está em manutenção no momento. Por favor, tente novamente mais tarde.", ENV.GAME);

				loadingHolder.SetActive(false);
				AlertsService.makeAlert("EM MANUTENÇÃO", message, "Entendi");
				reloadButton.gameObject.SetActive(true);
			}
			else
				LoadView("Login");
		}
		else
		{
			string message = string.Format("Houve uma falha na conexão com o {0}. Por favor, tente novamente mais tarde.", ENV.GAME);

			loadingHolder.SetActive(false);
			AlertsService.makeAlert("FALHA NA CONEXÃO", message, "OK");
			reloadButton.gameObject.SetActive(true);
		}

		yield return null;
	}
}
