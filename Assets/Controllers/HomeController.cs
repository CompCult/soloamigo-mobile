using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : ScreenController
{
	public Text nameField, pointsField;
	public RawImage profilePic;

	private User currentUser;
	private Texture2D photoTexture;

	public void Start ()
	{
		currentTab = "Home";
		photoTexture = UserService.user.profilePicture;

		TutorialService.CheckTutorial("Welcome");

		UpdateAndShowUser ();
	}

	public void UpdateAndShowUser()
	{
		FillInfoFields();
		StartCoroutine(_UpdateUserInfo());
	}

	private void FillInfoFields ()
	{
		currentUser = UserService.user;

		if (currentUser.name != null)
			nameField.text = currentUser.name;

		if (currentUser.points.ToString() != null)
			pointsField.text = currentUser.points.ToString();

		UserService.user.profilePicture = photoTexture;
		profilePic.texture = photoTexture;
	}

	private IEnumerator _UpdateUserInfo ()
	{
		WWW userRequest = UserService.GetUser(currentUser._id);

		while (!userRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + userRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + userRequest.text);

		if (userRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.UpdateLocalUser(userRequest.text);
			FillInfoFields();
		}

		yield return null;
	}
}
