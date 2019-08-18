using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalShowText : ModalGeneric 
{
	public InputField textMsg;

	public void Start ()
	{
		MissionAnswer currentAnswer = MissionsService.missionAnswer;
		textMsg.text = currentAnswer.text_msg;
	}

}
