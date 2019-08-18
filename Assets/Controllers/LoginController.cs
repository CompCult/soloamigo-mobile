using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : ScreenController
{
	public InputField emailField, passwordField;

	public void Start ()
	{
		CheckAuthenticatedUser();
	}

	private void CheckAuthenticatedUser ()
	{
		string param = UtilsService.GetParam("Email");

		if (PlayerPrefs.HasKey(param))
			emailField.text = PlayerPrefs.GetString(param);
	}

	private void SaveUser (string data)
	{
		string param = UtilsService.GetParam("Email");

		PlayerPrefs.SetString(param, emailField.text);
	}

	public void Authenticate ()
	{
		AlertsService.makeLoadingAlert("Autenticando");
		StartCoroutine(_Authenticate());
	}

	private IEnumerator _Authenticate ()
	{
		WWW loginRequest = UserService.Login(emailField.text, passwordField.text);

		while (!loginRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + loginRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + loginRequest.text);

		if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			SaveUser(loginRequest.text);
			UserService.UpdateLocalUser(loginRequest.text);
			yield return StartCoroutine(_GetUserPhoto());
		}
		else
		{
			AlertsService.removeLoadingAlert();

			if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("Senha incorreta", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_404 || loginRequest.responseHeaders["STATUS"] == HTML.HTTP_401)
				AlertsService.makeAlert("Usuário não encontrado", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else
				AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _GetUserPhoto ()
	{
		string photoUrl = UserService.user.picture;
		Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
		{
			texture = UtilsService.GetDefaultProfilePhoto();
		}
		else
		{
			var www = new WWW(photoUrl);
			yield return www;

			texture = www.texture;
		}

		if (texture != null)
			UserService.user.profilePicture = texture;

		UserService.user.profilePicture = texture;
		LoadView("Home");
	}

	private bool CheckFields ()
	{
		return UtilsService.CheckEmail(emailField.text) &&
			   UtilsService.CheckPassword(passwordField.text);
	}
}
