using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalText : ModalGeneric
{
	public InputField textMsg;
	private string modalType = "Text";

	public void Start ()
	{
		sendButton.interactable = false;
	}

	public void FixedUpdate ()
	{
		if (textMsg.text.Length < 5)
			sendButton.interactable = false;
		else
			sendButton.interactable = true;
	}

	public void SaveText ()
	{
		string text = textMsg.text;
		MissionsService.UpdateMissionAnswer(modalType, text);

		Destroy();
	}
}
