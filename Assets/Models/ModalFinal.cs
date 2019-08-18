using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModalFinal : ModalGeneric 
{
	public InputField textMsg;

	public void Start ()
	{
		sendButton.interactable = true;
		textMsg.text = MissionsService.mission.end_message;
	}

	public void Return ()
	{
		SceneManager.LoadScene("Missions");
	}
}
