using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : ScreenController
{
	private string STATUS_DONE = "OK";

	public void CheckTutorial (string stepName)
	{
		string hash = GetHash(stepName);

		if (!PlayerPrefs.HasKey(hash))
		{
			OpenModal(stepName);
			PlayerPrefs.SetString(hash, STATUS_DONE);
		}
	}

	private string GetHash (string stepName)
	{
		int userId = UserService.user._id;
		string param = UtilsService.GetParam("Tutorial"),
					 hash = string.Format("{0}:{1}:{2}", param, userId, stepName);

		return hash;
	}
}
