using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsController : ScreenController 
{
	public GameObject missionCard, noMissionsCard;
	public InputField secretCodeField;

	public void Start ()
	{
		TutorialService.CheckTutorial("Missions");

		previousView = "Home";
		missionCard.SetActive(false);
		noMissionsCard.SetActive(false);
		
		StartCoroutine(_GetMissions());
	}

	public void SearchMission ()
	{
		StartCoroutine(_SearchMission());
	}

	private IEnumerator _SearchMission ()
	{
		if (!CheckFields())
		{
			AlertsService.makeAlert("Código inválido", "Digite um código secreto com pelo menos quatro caracteres para realizar a busca.", "Entendi");
			yield break;
		}

		AlertsService.makeLoadingAlert("Buscando");
		
		string secretCode = secretCodeField.text;
		User currentUser = UserService.user;

		WWW missionRequest = MissionsService.SearchMission(currentUser._id, secretCode);

		while (!missionRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + missionRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + missionRequest.text);

		if (missionRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			MissionsService.UpdateMission(missionRequest.text);
			LoadView("Mission");
		}
		else 
		{
			AlertsService.makeAlert("Não encontrado", "Não encontramos nenhuma missão com esse código secreto. Por favor, verifique o código e tente novamente.", "OK");
		}

		yield return null;
	}

	private IEnumerator _GetMissions ()
	{
		User currentUser = UserService.user;

		AlertsService.makeLoadingAlert("Recebendo missões");
		WWW missionsRequest = MissionsService.GetMissions(currentUser._id);

		while (!missionsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + missionsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + missionsRequest.text);

		if (missionsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			MissionsService.UpdateMissions(missionsRequest.text);
			CreateMissionsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Activities");
		}

		yield return null;
	}

	private void CreateMissionsCards ()
    {
     	Vector3 position = missionCard.transform.position;

     	if (MissionsService.missions.Length > 0)
     		missionCard.SetActive(true);
     	else
     		noMissionsCard.SetActive(true);

     	foreach (Mission mission in MissionsService.missions)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(missionCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	MissionCard missionCardScript = card.GetComponent<MissionCard>();
        	missionCardScript.UpdateMissionCard(mission);
        }

        missionCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

    private bool CheckFields()
	{
		return UtilsService.CheckName(secretCodeField.text);
	}

}
