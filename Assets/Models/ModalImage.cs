using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalImage : ModalGeneric 
{
	public CameraCaptureService camService;
	public GameObject rotateButton;

	private string modalType = "Image";

	public void Start ()
	{
		camService.resetFields("pot_seeding");
		rotateButton.SetActive(false);

		StartCoroutine(_CheckCapturedPhoto());
	}

	public void SavePicture ()
	{
		string photoBase64 = camService.photoBase64;
		MissionsService.UpdateMissionAnswer(modalType, photoBase64);

		Destroy();
	}

	private IEnumerator _CheckCapturedPhoto ()
	{
		if (camService.photoBase64 != null)
		{
			rotateButton.SetActive(true);
			sendButton.interactable = true;
		}
		
		else
		{
			rotateButton.SetActive(false);
			sendButton.interactable = false;
		}

		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(_CheckCapturedPhoto());
	}
}
