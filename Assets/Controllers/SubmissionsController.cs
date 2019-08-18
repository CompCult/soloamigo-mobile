using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmissionsController : ScreenController 
{
	public GameObject submissionCard, noSubmissionsCard;

	public void Start ()
	{
		previousView = "Missions";
		submissionCard.SetActive(false);
		noSubmissionsCard.SetActive(false);
		
		StartCoroutine(_GetSubmissions());
	}

	private IEnumerator _GetSubmissions ()
	{
		User currentUser = UserService.user;

		AlertsService.makeLoadingAlert("Recebendo submissões");
		WWW submissionsRequest = MissionsService.GetSubmissions(currentUser._id);

		while (!submissionsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + submissionsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + submissionsRequest.text);

		if (submissionsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			MissionsService.UpdateSubmissions(submissionsRequest.text);
			CreateSubmissionsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Missions");
		}

		yield return null;
	}

	private void CreateSubmissionsCards ()
    {
     	Vector3 position = submissionCard.transform.position;

     	if (MissionsService.submissions.Length > 0)
     		submissionCard.SetActive(true);
     	else
     		noSubmissionsCard.SetActive(true);

     	foreach (MissionAnswer submission in MissionsService.submissions)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(submissionCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	SubmissionCard submissionCardScript = card.GetComponent<SubmissionCard>();
        	submissionCardScript.UpdateSubmissionCard(submission);
        }

        submissionCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

}
