using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalVoice : ModalGeneric 
{
	public Text micStatus;
	public AudioSource audioSource;
	public Button startRecordingButton, stopRecordingButton, playButton, stopButton, submitButton;
	
	private bool isRecording;
	private string modalType = "Voice";

	public void Start () 
	{
		AudioService.audioSource = audioSource;

		submitButton.interactable = false;
		playButton.interactable = false;
		stopButton.interactable = false;

		startRecordingButton.gameObject.SetActive(true);
		stopRecordingButton.gameObject.SetActive(false);

		micStatus.text = "Aguardando gravação";
	}

	public void SaveAudio ()
	{
		string audioBase64 = MissionsService.missionAnswer.audio;
		MissionsService.UpdateMissionAnswer(modalType, audioBase64);

		Destroy();
	}

	public void ToggleRecord ()
	{
		AudioService.ToggleRecord();

		if (AudioService.isRecording())
		{
			micStatus.text = "Gravando...";

			stopRecordingButton.gameObject.SetActive(true);
			startRecordingButton.gameObject.SetActive(false);

			playButton.interactable = false;
			stopButton.interactable = false;
			submitButton.interactable = false;
		}
		else
		{
			startRecordingButton.gameObject.SetActive(true);
			stopRecordingButton.gameObject.SetActive(false);

			if (AudioService.isRecorded())
			{
				micStatus.text = "Gravação concluída";

				playButton.interactable = true;
				stopButton.interactable = true;
				submitButton.interactable = true;
			}
			else 
			{
				micStatus.text = "Aguardando gravação";

				playButton.interactable = false;
				stopButton.interactable = false;
				submitButton.interactable = false;
			}
		}
	}

	public void ToggleAudio ()
	{
		AudioService.ToggleAudio();
	}
}
