using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalGPS : ModalGeneric 
{
	public Text gpsStatus;
	private string modalType = "GPS";

	public void Start ()
	{
		sendButton.interactable = false;
		StartCoroutine(_CheckGPS());
	}

	public void SaveGeolocation ()
	{
		string lat = GPSService.location[0].ToString(),
			   lng = GPSService.location[1].ToString();

		MissionsService.UpdateMissionAnswer(modalType, lat, lng);

		Destroy();
	}

	private IEnumerator _CheckGPS()
	{
		bool mustCheckGPS = false;
		GPSService.StartGPS();

		if (!GPSService.IsActive())
		{
			gpsStatus.text = "Aguardando ativação do GPS no celular";
			mustCheckGPS = true;
		}
		else if (GPSService.location[0] == 0.0 || GPSService.location[1] == 0.0)
		{
			GPSService.ReceivePlayerLocation();
			gpsStatus.text = "Obtendo localização...";
			mustCheckGPS = true;
		}
		else 
		{
			gpsStatus.text = "Dispositivo localizado";
			sendButton.interactable = true;
		}

		if (mustCheckGPS)
		{
			yield return new WaitForSeconds(2);
			yield return StartCoroutine(_CheckGPS());
		}
		else
		{
			yield return null;
		}
	}
}
