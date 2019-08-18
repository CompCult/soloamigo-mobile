using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : ScreenController
{
	public GameObject imageButton, audioButton, videoButton, textButton, geolocationButton;
	public Button sendButton;
	public InputField missionName;
	public Text missionDescription;
	public Dropdown senderTypeDropdown;

	private Mission currentMission;
	private MissionAnswer currentAnswer;
	private bool canSend = false;
	private Color COLOR_GREEN = new Color(0.07450981f, 0.7568628f, 0.3333333f),
		  	  	  COLOR_RED = new Color(1f, 0.4392157f, 0.4392157f);

	public void Start ()
	{
		previousView = "Missions";
		currentMission = MissionsService.mission;
		sendButton.interactable = false;

		missionName.text = currentMission.name;
		missionDescription.text = currentMission.description;

		currentMission = MissionsService.mission;
		currentAnswer = MissionsService.missionAnswer;

		ResetButtons ();
		UpdateMissionInfo ();

		StartCoroutine(_CheckResponses());
	}

	public void SendResponse ()
	{
		string message = null;

		if (currentAnswer != null)
		{
			if (currentMission.has_image && currentAnswer.image == null)
				message = "Você precisa registrar uma foto antes de enviar essa resposta.";

			if (currentMission.has_audio && currentAnswer.audio == null)
				message = "Você precisa capturar um áudio antes de enviar essa resposta.";

			if (currentMission.has_video && currentAnswer.video == null)
				message = "Você precisa registrar um vídeo antes de enviar essa resposta.";

			if (currentMission.has_text && currentAnswer.text_msg == null)
				message = "Você precisa escrever um texto antes de enviar essa resposta.";

			if (currentMission.has_geolocation && (currentAnswer.location_lat == null || currentAnswer.location_lng == null))
				message = "Você precisa registrar sua geolocalização antes de enviar essa resposta.";
		}
		else
		{
			message = "Você precisa enviar os dados solicitados pela missão antes de enviar uma resposta.";
		}

		if (message == null)
			StartCoroutine(_SendResponse());
		else
			AlertsService.makeAlert("Aviso", message, "Entendi");
	}

	#pragma warning disable 0472
	private IEnumerator _CheckResponses ()
	{
		canSend = true;

		if (currentMission.is_grupal != null && currentMission.is_grupal)
			if (GroupsService.groups == null || GroupsService.groups.Length < 1)
				canSend = false;

		if (currentMission.has_image != null && currentMission.has_image)
			if (currentAnswer.image == null)
			{
				canSend = false;
				imageButton.GetComponent<Image>().color = COLOR_RED;
			}
			else
				imageButton.GetComponent<Image>().color = COLOR_GREEN;

		if (currentMission.has_audio != null && currentMission.has_audio)
			if (currentAnswer.audio == null)
			{
				canSend = false;
				audioButton.GetComponent<Image>().color = COLOR_RED;
			}
			else
				audioButton.GetComponent<Image>().color = COLOR_GREEN;

		if (currentMission.has_video != null && currentMission.has_video)
			if (currentAnswer.video == null)
			{
				canSend = false;
				videoButton.GetComponent<Image>().color = COLOR_RED;
			}
			else
				videoButton.GetComponent<Image>().color = COLOR_GREEN;

		if (currentMission.has_text != null && currentMission.has_text)
			if (currentAnswer.text_msg == null)
			{
				canSend = false;
				textButton.GetComponent<Image>().color = COLOR_RED;
			}
			else
				textButton.GetComponent<Image>().color = COLOR_GREEN;

		if (currentMission.has_geolocation != null && currentMission.has_geolocation)
			if (currentAnswer.location_lat == null || currentAnswer.location_lng == null)
			{
				canSend = false;
				geolocationButton.GetComponent<Image>().color = COLOR_RED;
			}
			else
				geolocationButton.GetComponent<Image>().color = COLOR_GREEN;

		sendButton.interactable = canSend;

		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(_CheckResponses());
	}

	private IEnumerator _SendResponse ()
	{
		AlertsService.makeLoadingAlert("Enviando resposta");

		int currentUserId = UserService.user._id,
			missionId = MissionsService.mission._id,
			groupId = GetSelectedGroupId(senderTypeDropdown.captionText.text);

		WWW responseRequest = MissionsService.SendResponse(currentUserId, missionId, groupId);

		while (!responseRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + responseRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + responseRequest.text);
		AlertsService.removeLoadingAlert();

		if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			Mission currentMission = MissionsService.mission;

			if (currentMission.end_message != null && currentMission.end_message.Length > 0)
				OpenModal("Final");
			else
			{
				AlertsService.makeAlert("Resposta enviada", "Boa! Sua resposta foi enviada com sucesso. Você será redirecionado(a) para as missões.", "");
				yield return new WaitForSeconds(5f);
				LoadView("Missions");
			}
		}
		else
		{
			if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("Senha incorreta", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_404 || responseRequest.responseHeaders["STATUS"] == HTML.HTTP_401)
				AlertsService.makeAlert("Usuário não encontrado", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else
				AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	#pragma warning disable 0472
	private void UpdateMissionInfo ()
	{
		if (currentMission.is_grupal != null && currentMission.is_grupal)
			StartCoroutine(_GetGroups());

		if (currentMission.has_image != null && currentMission.has_image)
			imageButton.SetActive(true);

		if (currentMission.has_audio != null && currentMission.has_audio)
			audioButton.SetActive(true);

		if (currentMission.has_video != null && currentMission.has_video)
			videoButton.SetActive(true);

		if (currentMission.has_text != null && currentMission.has_text)
			textButton.SetActive(true);

		if (currentMission.has_geolocation != null && currentMission.has_geolocation)
			geolocationButton.SetActive(true);
	}

	private IEnumerator _GetGroups ()
	{
		User currentUser = UserService.user;
		WWW groupsRequest = GroupsService.GetUserGroups(currentUser._id);

		while (!groupsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + groupsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + groupsRequest.text);

		if (groupsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			GroupsService.UpdateCurrentGroups(groupsRequest.text);

			if (GroupsService.groups.Length < 1)
			{
				senderTypeDropdown.gameObject.SetActive(false);
				AlertsService.makeAlert("Sem grupos", "Essa é uma missão em grupo e você não está em nenhum grupo. Participe de um grupo para poder responder essa missão.", "");
				yield return new WaitForSeconds(5f);
				LoadView("Missions");
			}

			FillGroupsDropdown();
		}
		else
		{
			AlertsService.makeAlert("Falha na conexão", "Essa é uma missão em grupo e não pudemos listar seus grupos.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Missions");
		}

		yield return null;
	}

	private int GetSelectedGroupId (string groupName)
	{
		if (MissionsService.mission.is_grupal && groupName != null && GroupsService.groups.Length > 0)
			foreach (Group group in GroupsService.groups)
				if (group.name == groupName)
					return group._id;

		return 0;
	}

	private void FillGroupsDropdown ()
	{
		senderTypeDropdown.ClearOptions();
	    senderTypeDropdown.AddOptions(GroupsService.GetGroupNames());
	    senderTypeDropdown.RefreshShownValue();
	}

	private void ResetButtons ()
	{
		imageButton.SetActive(false);
		audioButton.SetActive(false);
		videoButton.SetActive(false);
		geolocationButton.SetActive(false);
		textButton.SetActive(false);
	}

}
