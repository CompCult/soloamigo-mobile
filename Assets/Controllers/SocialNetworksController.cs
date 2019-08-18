using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialNetworksController : ScreenController 
{
	public void OpenPage (string type)
	{
		string url = "";

		if (type == "Facebook")
			url = ENV.FACEBOOK_PAGE;
		if (type == "Instagram")
			url = ENV.INSTAGRAM_PAGE;

		if (url.Length > 0)
			Application.OpenURL(url);
	}

}
