using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PostCard : MonoBehaviour
{
	#pragma warning disable 0108

	public RawImage profilePic, imagePost;
	public Text authorName, date, message, likes;

	public Post post;
	public GameObject loadingHolder, likeButton, removeButton;

	public void LikePost ()
	{
		if (UserService.user.points <= 0)
		{
			string title = string.Format("Sem {0}(s)", ENV.POINT),
						 message = string.Format("Você não tem {0}(s) para dar. Realize atividades e participe no aplicativo para ganhar mais.", ENV.POINT);

			AlertsService.makeAlert(title, message, "OK");
			return;
		}

		bool UPDATE_USER_POINTS = true;
		StartCoroutine(_ChangePostPoints(1, UPDATE_USER_POINTS));
	}

	public void UpdatePost (Post post)
	{
		this.post = post;
		UpdateFields();
	}

	public void RemovePost ()
	{
		AlertsService.makeLoadingAlert("Removendo");
		StartCoroutine(_RemovePost());
	}

	public void UpdateFields()
	{
		User currentUser = UserService.user;

		if (currentUser._id == post._user)
		{
			likeButton.SetActive(false);
			removeButton.SetActive(true);
		}
		else
		{
			likeButton.SetActive(true);


			Debug.Log("currentUser.type: " + currentUser.type);


			if (currentUser.type.ToLower().Contains("gestor"))
				removeButton.SetActive(true);
			else
				removeButton.SetActive(false);
		}

		UpdateTextFields();
		StartCoroutine(_GetAuthorPhoto());
		StartCoroutine(_GetPostImage());
	}

	public void UpdateTextFields()
	{
		authorName.text = post.author_name;
		date.text = UtilsService.GetDate(post.created_at);
		message.text = post.text_msg;
		likes.text = post.points.ToString();
	}

	private IEnumerator _RemovePost ()
	{
		WWW removeRequest = TimelineService.RemovePost(post);

		while (!removeRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + removeRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + removeRequest.text);
		AlertsService.removeLoadingAlert();

		if (removeRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			Destroy(this.gameObject);
		}
	}

	private IEnumerator _GetAuthorPhoto ()
	{
		string photoUrl = post.author_photo;
		Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
		{
			texture = UtilsService.GetDefaultProfilePhoto();
		}
		else
		{
			Debug.Log ("Current author photo url is " + photoUrl);
			var www = new WWW(photoUrl);
			yield return www;

			texture = www.texture;
		}

		if (texture == null)
			texture = UtilsService.GetDefaultProfilePhoto();

		profilePic.texture = texture;
		UtilsService.SizeToParent(profilePic, 0f);
	}

	private IEnumerator _GetPostImage ()
    {
    	string photoUrl = post.picture;
    	Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
			yield break;

		Debug.Log ("current post photo url is " + photoUrl);
		var www = new WWW(photoUrl);
		yield return www;

		if (www.responseHeaders["STATUS"] != HTML.HTTP_200)
			yield break;

		texture = www.texture;

		if (texture == null)
		{
			Debug.LogError("Failed to load texture url: " + photoUrl);
			yield break;
		}

		Destroy(loadingHolder);
		imagePost.texture = texture;
		UtilsService.SizeToParent(imagePost, 0f);
    }

	private IEnumerator _ChangePostPoints (int newPoints, bool updateUser)
	{
		WWW likeRequest = TimelineService.UpdatePostPoints(post, newPoints);

		while (!likeRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + likeRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + likeRequest.text);

		if (likeRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			post.points += newPoints;

			if (updateUser)
				StartCoroutine(_ChangeUserPoints(-1 * newPoints));
		}

		UpdateTextFields();
	}

	private IEnumerator _ChangeUserPoints(int newPoints)
	{
		WWW pointsRequest = UserService.UpdatePoints(UserService.user, newPoints);

		while (!pointsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + pointsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + pointsRequest.text);

		if (pointsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.user.points += newPoints;
		}
		else
		{
			bool DONT_UPDATE_USER_POINTS = false;
			StartCoroutine(_ChangePostPoints(-1, DONT_UPDATE_USER_POINTS));
		}
	}
}
