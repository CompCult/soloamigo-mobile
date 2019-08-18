using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class MissionsService
{
	private static Mission[] _missions;
	public static Mission[] missions { get { return _missions; } }

	private static Mission _mission;
	public static Mission mission { get { return _mission; } }

	private static MissionAnswer _missionAnswer = new MissionAnswer();
	public static MissionAnswer missionAnswer { get { return _missionAnswer; } }

	private static MissionAnswer[] _submissions;
	public static MissionAnswer[] submissions { get { return _submissions; } }

	public static WWW SearchMission (int userId, string secretCode)
	{
		WebService.route = ENV.MISSIONS_ROUTE;
		WebService.action = ENV.SEARCH_PRIVATE +
							"user_id=" + userId +
							"&secret_code=" + secretCode;

		return WebService.Get();
	}

	public static WWW SendResponse (int userId, int missionId, int groupId)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("_user", userId);
		requestForm.AddField ("_mission", missionId);

		if (mission.is_grupal)
			requestForm.AddField ("_group", groupId);

		if (mission.has_image)
			requestForm.AddField ("image", missionAnswer.image);

		if (mission.has_audio)
			requestForm.AddField ("audio", missionAnswer.audio);

		if (mission.has_video)
			requestForm.AddField ("video", missionAnswer.video);

		if (mission.has_text)
			requestForm.AddField ("text_msg", missionAnswer.text_msg);

		if (mission.has_geolocation)
		{
			requestForm.AddField ("location_lat", missionAnswer.location_lat);
			requestForm.AddField ("location_lng", missionAnswer.location_lng);
		}

		WebService.route = ENV.MISSION_ANSWERS_ROUTE;
		WebService.action = "";

		return WebService.Post(requestForm);
	}

	public static WWW GetMission (int missionId)
	{
		ResetContent();

		WebService.route = ENV.MISSIONS_ROUTE;
		WebService.id = ENV.QUERY_ACTION +
					 	"_id=" + missionId;

		return WebService.Get();
	}

	public static WWW GetMissions (int userId)
	{
		ResetContent();

		WebService.route = ENV.MISSIONS_ROUTE;
		WebService.action = ENV.SEARCH_PUBLIC +
							"user_id=" + userId;

		return WebService.Get();
	}

	public static WWW GetSubmissions (int userId)
	{
		ResetContent();

		WebService.route = ENV.MISSION_ANSWERS_ROUTE;
		WebService.action = ENV.QUERY_ACTION +
							"_user=" + userId;

		return WebService.Get();
	}

	public static void UpdateMissions (string json)
	{
		_missions = UtilsService.GetJsonArray<Mission>(json);
	}

	public static void UpdateSubmissions (string json)
	{
		_submissions = UtilsService.GetJsonArray<MissionAnswer>(json);
	}

	public static void UpdateMission (string json)
	{
		ResetContent();
		_mission = JsonUtility.FromJson<Mission>(json);
	}

	public static void UpdateMission (Mission mission)
	{
		ResetContent();
		_mission = mission;
	}

	public static void UpdateMissionAnswer (MissionAnswer missionAnswer)
	{
		_missionAnswer = missionAnswer;
	}

	public static void UpdateMissionAnswer (string field, string fieldValue)
	{
		UpdateMissionAnswer(field, fieldValue, null);
	}

	public static void UpdateMissionAnswer (string field, string fieldValue, string fieldValue2)
	{
		if (_missionAnswer == null)
			_missionAnswer = new MissionAnswer();

		if (fieldValue == null)
		{
			string message = "Ocorreu um problema com sua submissão de " + field + ". Por favor, verifique sua submissão.";
			AlertsService.makeAlert("Campo inválido", message, "OK");
			return;
		}

		if (field == "Image")
			_missionAnswer.image = fieldValue;

		if (field == "GPS")
		{
			_missionAnswer.location_lat = fieldValue;
			_missionAnswer.location_lng = fieldValue2;
		}

		if (field == "Text")
			_missionAnswer.text_msg = fieldValue;

		if (field == "Video")
			_missionAnswer.video = fieldValue;

		if (field == "Audio")
			_missionAnswer.audio = fieldValue;
	}

	private static void ResetContent ()
	{
		_missions = null;
		_mission = new Mission();
		_missionAnswer = new MissionAnswer();
	}

}
