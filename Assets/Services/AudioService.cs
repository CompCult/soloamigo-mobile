using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414
public static class AudioService
{
	public static AudioSource audioSource;
	public static string audioBase64;

	private static int minFreq, maxFreq;
	private static string micName;

	private static void ConfigureMicrophone ()
	{
		if (hasMicrophone())
		{
			micName = Microphone.devices[0];
			Microphone.GetDeviceCaps(micName, out minFreq, out maxFreq);

			audioBase64 = null;

			minFreq = 0;
			maxFreq = 44100;
		}
	}

	#pragma warning disable 0618
	public static void ToggleRecord ()
	{
		if (!hasMicrophone())
		{
			AlertsService.makeAlert("Sem microfone", "Nenhum microfone foi encontrado em seu dispositivo.", "Entendi");
			return;
		}

		ConfigureMicrophone();

		if (isRecording()) //Recording is in progress, then stop it.
		{
			SavWav.instance.Init();
			var fileName = GetAudioFileName();
			var position = Microphone.GetPosition(micName);

			Microphone.End(micName); //Stop the audio recording

			var soundData = new float[audioSource.clip.samples * audioSource.clip.channels];
			audioSource.clip.GetData (soundData, 0);

			//Create shortened array for the data that was used for recording
			var newData = new float[position * audioSource.clip.channels];

			//Copy the used samples to a new array
			for (int i = 0; i < newData.Length; i++)
			    newData[i] = soundData[i];

			//Creates a newClip with the correct length
			var newClip = AudioClip.Create (fileName,
			                                 position,
			                                 audioSource.clip.channels,
			                                 audioSource.clip.frequency,
			                                 true, false);

			newClip.SetData (newData, 0); //Give it the data from the old clip

		 	//Replace the old clip
			AudioClip.Destroy (audioSource.clip);
			audioSource.clip = newClip;

			SavWav.instance.Save (fileName, audioSource.clip);
		}
		else // Starts the recording
		{
			audioSource.clip = Microphone.Start(micName, true, 600, maxFreq);
		}

	}

	public static void ToggleAudio ()
	{
		if (audioSource.clip == null)
		{
			AlertsService.makeAlert("Sem gravações", "Você ainda não gravou nenhum áudio para poder reproduzir.", "Entendi");
		}
		else if (isRecorded())
		{
			if (audioSource.isPlaying)
				audioSource.Stop();
			else
				audioSource.Play ();
		}
	}

	public static bool isRecorded ()
	{
		var filepath = Path.Combine(Application.persistentDataPath, GetAudioFileName());

		if (System.IO.File.Exists (filepath))
			return true;

		return false;
	}

	public static bool isPlaying ()
	{
		return audioSource.isPlaying;
	}

	public static bool isRecording ()
	{
		return Microphone.IsRecording(micName);
	}

	public static bool hasMicrophone ()
	{
		return Microphone.devices.Length > 0;
	}

	private static string GetAudioFileName()
	{
		var currentMissionId = MissionsService.mission._id;
		var fileName = string.Format("{0}-mission-{1}-audio.wav", ENV.GAME, currentMissionId);

		return fileName;
	}
}
