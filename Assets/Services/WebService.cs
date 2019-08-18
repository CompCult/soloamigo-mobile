using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class WebService
{
	public static string route, action, id;

	#pragma warning disable 0219
	public static WWW Get()
	{
		string apiLink = BuildRequestUrl();
		WWW www = new WWW (apiLink);

		Debug.Log("WebAPI - Get: " + apiLink);

		ResetRequestUrl();
		return www;
	}

	#pragma warning disable 0219
	public static WWW Post(WWWForm form)
	{
		string apiLink = BuildRequestUrl();
		WWW www = new WWW(apiLink, form);

		Debug.Log("WebAPI - Post: " + apiLink);

		ResetRequestUrl();
		return www;
	}

	#pragma warning disable 0219
	public static WWW GetExternal (string url)
	{
		WWW www = new WWW (url);

		Debug.Log("WebExternal - Get: " + url);

		return www;
	}

	#pragma warning disable 0219
	public static WWW PostExternal (WWWForm form, string url)
	{
		WWW www = new WWW(url, form);

		Debug.Log("WebExternal - Post: " + url);

		return www;
	}

	private static string BuildRequestUrl ()
	{
		string apiLink = ENV.API_URL;

		if (route != null && route.Length > 0)
			apiLink += "/" + route;

		if (action != null && action.Length > 0)
			apiLink += "/" + action;

		if (id != null && id.Length > 0)
			apiLink += "/" + id;

		return apiLink;
	}

	private static void ResetRequestUrl ()
	{
		route = "";
		action = "";
		id = "";
	}
}
