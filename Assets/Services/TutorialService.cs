using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialService
{
	private static string STATUS_DONE = "OK";

	public static void CheckTutorial (string stepName)
	{
		if (!UserService.user.type.ToLower().Contains("estudante"))
			return;

		string hash = GetHash(stepName);

		if (!PlayerPrefs.HasKey(hash))
		{
			OpenModal(stepName);
			PlayerPrefs.SetString(hash, STATUS_DONE);
		}
	}

	public static void CheckParentalAlert ()
	{
		if (UserService.user.type.ToLower().Contains("estudante"))
			return;

		int userId = UserService.user._id;
		string param = UtilsService.GetParam("Parental"),
					 hash = string.Format("{0}:{1}", param, userId);

		if (!PlayerPrefs.HasKey(hash))
		{
			PlaySound();

			AlertsService.makeAlert("Aviso", "Essa página é focada para estudantes do município. A maior parte do conteúdo aqui presente foi apresentado por tutores ou professores previamente em sala.", "Entendi");
			PlayerPrefs.SetString(hash, STATUS_DONE);
		}
	}

	private static void OpenModal (string modalName)
	{
		PlaySound();

		string modalPath = "Prefabs/Modal" + modalName;

        GameObject modalPrefab = (GameObject) Resources.Load(modalPath),
                   modalInstance = (GameObject) GameObject.Instantiate(modalPrefab, Vector3.zero, Quaternion.identity);

        modalInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        modalInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
	}

	private static string GetHash (string stepName)
	{
		int userId = UserService.user._id;
		string param = UtilsService.GetParam("Tutorial"),
					 hash = string.Format("{0}:{1}:{2}", param, userId, stepName);

		return hash;
	}

	private static void PlaySound ()
	{
		string path = "Sounds/modal";
		AudioClip clip = (AudioClip) Resources.Load(path);

		if (clip == null)
			return;

		GameObject[] instances = GameObject.FindGameObjectsWithTag("Audio");
		if (instances.Length < 1)
			return;

		AudioSource audioSource = instances[0].GetComponent<AudioSource>();

		audioSource.clip = clip;
        audioSource.Play();
	}
}
