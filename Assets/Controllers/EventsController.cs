using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsController : ScreenController 
{
	public GameObject eventCard, noEventsCard;

	public void Start ()
	{
		TutorialService.CheckTutorial("Events");

		previousView = "Login";
		eventCard.SetActive(false);

		StartCoroutine(_GetEvents());
	}

	private IEnumerator _GetEvents ()
	{
		AlertsService.makeLoadingAlert("Recebendo eventos");
		WWW eventsRequest = EventsService.GetEvents();

		while (!eventsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + eventsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + eventsRequest.text);
		AlertsService.removeLoadingAlert();

		if (eventsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			StartCoroutine(_GetUserEvents());
			EventsService.UpdateLocalEvents(eventsRequest.text);
			CreateEventCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private IEnumerator _GetUserEvents ()
	{
		AlertsService.makeLoadingAlert("Recebendo eventos");
		WWW eventsRequest = EventsService.GetUserEvents(UserService.user._id);

		while (!eventsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + eventsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + eventsRequest.text);
		AlertsService.removeLoadingAlert();

		if (eventsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
			EventsService.UpdateUserEvents(eventsRequest.text);
		else 
			AlertsService.makeAlert("Alerta", "Por conta de um erro em sua conexão, novos pedidos de participação em eventos serão negados por enquanto.", "Entendi");

		yield return null;
	}

	private void CreateEventCards ()
    {
     	Vector3 position = eventCard.transform.position;

     	if (EventsService.events.Length > 0)
     		eventCard.SetActive(true);
     	else
     		noEventsCard.SetActive(true);

     	foreach (Event evt in EventsService.events)
        {
        	position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(eventCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	EventCard evtCard = card.GetComponent<EventCard>();
        	evtCard.UpdateEvent(evt);
        }

        eventCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

}
