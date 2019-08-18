using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalVideo : ModalGeneric
{
	public CameraCaptureService camService;
	private string modalType = "Video";

	public void Start ()
	{
		camService.resetFields();
	}

	public void FixedUpdate ()
	{
		if (camService.videoBase64 != null)
			sendButton.interactable = true;
		else
			sendButton.interactable = false;
	}

	public void SaveVideo ()
	{
		string videoBase64 = camService.videoBase64;
		MissionsService.UpdateMissionAnswer(modalType, videoBase64);

		Destroy();
	}
}
