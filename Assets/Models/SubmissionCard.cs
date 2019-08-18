using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubmissionCard : MonoBehaviour
{
	#pragma warning disable 0108 0472

	public GameObject imageButton, audioButton, videoButton, textButton, gpsButton,
					  statusApproved, statusPendent, statusRejected;
	
	public Text title, date;

	public Mission[] missions;
	public MissionAnswer submission;

	public void UpdateSubmissionCard (MissionAnswer submission)
	{
		ResetButtons();
		this.submission = submission;

		title.text = "Missão #" + submission._mission.ToString();
		date.text = "Submissão enviada em " + UtilsService.GetDate(submission.created_at);

		if (submission.image != null && submission.image.Length > 0)
			imageButton.SetActive(true);

		if (submission.audio != null && submission.audio.Length > 0)
			audioButton.SetActive(true);

		if (submission.video != null && submission.video.Length > 0)
			videoButton.SetActive(true);

		if (submission.location_lat != null && submission.location_lng != null && 
			submission.location_lat.Length > 0 && submission.location_lng.Length > 0)
			gpsButton.SetActive(true);

		if (submission.text_msg != null && submission.text_msg.Length > 0)
			textButton.SetActive(true);

		switch (submission.status)
		{
			case "Pendente": statusPendent.SetActive(true); break;
			case "Válida": statusApproved.SetActive(true); break;
			case "Inválida": statusRejected.SetActive(true); break;
		}

		StartCoroutine(_UpdateMission());
	}

	private IEnumerator _UpdateMission ()
	{
		WWW missionRequest = MissionsService.GetMission(submission._mission);

		while (!missionRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + missionRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + missionRequest.text);

		if (missionRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			missions = UtilsService.GetJsonArray<Mission>(missionRequest.text);

			if (missions.Length >= 1)
			{
				title.text = missions[0].name;
			}
		}

		yield return null;
	}

   	public void OpenLink (string type)
	{
		if (type == "Image")
			Application.OpenURL(submission.image);

		if (type == "Video")
			Application.OpenURL(submission.video);

		if (type == "Audio")
			Application.OpenURL(submission.audio);
	}

	public void OpenModal (string modalName)
	{
		UpdateCurrentSubmission();
		
		string modalPath = "Prefabs/Modal" + modalName;

        GameObject modalPrefab = (GameObject) Resources.Load(modalPath),
                   modalInstance = (GameObject) GameObject.Instantiate(modalPrefab, Vector3.zero, Quaternion.identity);
        
        modalInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        modalInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
	}

	private void ResetButtons()
	{
		statusApproved.SetActive(false);
		statusRejected.SetActive(false);
		statusPendent.SetActive(false);
		imageButton.SetActive(false);
		audioButton.SetActive(false);
		videoButton.SetActive(false);
		textButton.SetActive(false);
		gpsButton.SetActive(false);
	}

	private void UpdateCurrentSubmission ()
	{
		MissionsService.UpdateMissionAnswer(submission);
	}
}
