using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EventCard : MonoBehaviour
{
	#pragma warning disable 0108

	public Image mapImage;
	public Text name, description, place, dates, status;

	public Event evt;
	public EventRequest evtRequest;

	public GameObject statusIconBg;
	public GameObject[] statusIcons;
	
	public GameObject loadingHolder;

	private Dictionary <string, string> statusDict = new Dictionary<string, string>();

	private Color COLOR_GREEN = new Color(0.1444823f, 0.490566f, 0.1272695f),
				  COLOR_YELLOW = new Color(0.745283f, 0.6818547f, 0f),
				  COLOR_RED = new Color(0.7264151f, 0.06510323f, 0.1408988f);


	public void JoinEvent()
	{
		if (EventsService.eventsRequests == null)
		{
			AlertsService.makeAlert("Alerta", "Não estamos aceitando pedidos de participação no momento. Tente novamente mais tarde.", "Entendi");
			return;
		}

		foreach (EventRequest evtRequest in EventsService.eventsRequests)
		{
			if (evtRequest._appointment._id == evt._id)
			{
				AlertsService.makeAlert("Evento na agenda", "Você já está participando deste evento. Procure outros eventos para ingressar ou consulte sua agenda.", "Entendi");
				return;
			}
		}

		int userId = UserService.user._id;
		StartCoroutine(_JoinEvent(userId));
	}

	public void UpdateEventWithStatus (Event evt, EventRequest evtRequest)
	{
		this.evtRequest = evtRequest;
		statusDict["Aprovado"] = "Approved";
		statusDict["Pendente"] = "Pending";
		statusDict["Negado"] = "Denied";

		string status;
		if (evtRequest.status == null)
			status = "Pendente";
		else
			status = evtRequest.status;

		if (!statusDict.ContainsKey(status))
			status = "Negado";

		foreach (GameObject statusIcon in statusIcons)
			if (statusDict[status] == statusIcon.name)
			{
				statusIcon.SetActive(true);
				FillStatus(evtRequest);
			}
			else
			{
				statusIcon.SetActive(false);
			}

		UpdateEvent(evt);
	}

	public void UpdateEvent (Event evt)
	{
		this.evt = evt;
		UpdateFields();
	}

	public void UpdateTextFields()
	{
		name.text = evt.name;
		description.text = evt.description;
		dates.text = UtilsService.GetDate(evt.start_date) + " - " + UtilsService.GetDate(evt.end_date);
		place.text = evt.place;
	}

	public void UpdateFields()
	{
		UpdateTextFields();
		StartCoroutine(UpdateImage());
	}

	private IEnumerator UpdateImage ()
    {
    	string url = ENV.GOOGLE_MAPS_URL.Replace("PLACE", evt.place).Replace(" ", "%20");

    	Debug.Log("url: " + url);

    	UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		www.SetRequestHeader("Accept", "image/*");
		
		var async = www.SendWebRequest();
		while (!async.isDone)
		    yield return null;

		if (!www.isNetworkError)
		{
		    yield return new WaitForEndOfFrame();

		    byte[] results = www.downloadHandler.data;
		    Texture2D texture = new Texture2D(100, 100);

		    texture.LoadImage(results);
		    Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

		    mapImage.sprite = sprite;
		    Destroy(loadingHolder);
		}
    }

    private void FillStatus(EventRequest evtRequest)
    {
    	string status = evtRequest.status;
    	Image background = statusIconBg.GetComponent<Image>();

    	switch (status)
      	{
          case "Aprovado":
          	  this.status.text = "Participação aprovada";
              background.color = COLOR_GREEN;
              break;
          case "Negado":
          	  this.status.text = "Pedido de participação negado";
              background.color = COLOR_RED;
              break;
          default:
          	  this.status.text = "Pedido de participação pendente";
              background.color = COLOR_YELLOW;
              break;
      	}
    }

   	private IEnumerator _JoinEvent(int userId)
	{
		WWW joinRequest = EventsService.JoinEvent(userId, evt);

		while (!joinRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + joinRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + joinRequest.text);

		if (joinRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
			AlertsService.makeAlert("SUCESSO", "Seu pedido de participação no evento foi enviado. Acompanhe o status do pedido em sua agenda.", "OK");
		else 
			AlertsService.makeAlert("Falha no pedido", "Tente novamente mais tarde.", "Entendi");

		yield return null;
	}
}
