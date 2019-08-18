using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionCard : MonoBehaviour
{
	#pragma warning disable 0108 0472

	public GameObject groupalIcon, individualIcon,
					  infinityChancesIcon, singleChancesIcon,
					  imageIcon, audioIcon, videoIcon, textIcon, geolocationIcon;
	
	public Text title, description, date, points;
	public Mission mission;

	public void UpdateMissionCard (Mission mission)
	{
		ResetIcons();

		this.mission = mission;

		title.text = mission.name;
		date.text = "Até " + UtilsService.GetDate(mission.end_time);
		points.text = mission.points.ToString();

		if (mission.description.Length > 60)
			description.text = mission.description.Substring(0, 57) + "...";
		else
			description.text = mission.description;

		if (mission.single_answer != null && mission.single_answer)
			singleChancesIcon.SetActive(true);
		else
			infinityChancesIcon.SetActive(true);

		if (mission.is_grupal != null && mission.is_grupal)
			groupalIcon.SetActive(true);
		else
			individualIcon.SetActive(true);

		if (mission.has_image != null && mission.has_image)
			imageIcon.SetActive(true);

		if (mission.has_audio != null && mission.has_audio)
			audioIcon.SetActive(true);

		if (mission.has_video != null && mission.has_video)
			videoIcon.SetActive(true);

		if (mission.has_text != null && mission.has_text)
			textIcon.SetActive(true);

		if (mission.has_geolocation != null && mission.has_geolocation)
			geolocationIcon.SetActive(true);
	}

   	public void OpenMission ()
	{
		MissionsService.UpdateMission(mission);
		SceneManager.LoadScene("Mission");
	}

	private void ResetIcons()
	{
		groupalIcon.SetActive(false);
		individualIcon.SetActive(false);
		infinityChancesIcon.SetActive(false);
		singleChancesIcon.SetActive(false);
		imageIcon.SetActive(false);
		audioIcon.SetActive(false);
		videoIcon.SetActive(false);
		textIcon.SetActive(false);
		geolocationIcon.SetActive(false);
	}
}
