using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecoverController : ScreenController
{
	public InputField emailField, passwordField;

	public void Start ()
	{
		previousView = "Login";
	}

	public void RecoverPassword ()
	{
		AlertsService.makeLoadingAlert("Enviando");
		StartCoroutine(_RecoverPassword());
	}

	private IEnumerator _RecoverPassword ()
	{
		if (!CheckFields())
		{
			AlertsService.makeAlert("Campos Inválidos", "Insira um e-mail válido e uma nova senha de pelo menos seis dígitos.", "Entendi");
			yield break;
		}

		string email = emailField.text,
			   newPassword = passwordField.text;

		WWW recoverRequest = UserService.Recover(email, newPassword);

		while (!recoverRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + recoverRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + recoverRequest.text);
		AlertsService.removeLoadingAlert();

		if (recoverRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			AlertsService.makeAlert("SUCESSO", "Você receberá um e-mail para confirmar a modificação da senha em breve.", "");
			yield return new WaitForSeconds(4f);
			LoadView("Login");
		}
		else
		{
			AlertsService.makeAlert("FALHA NA CONEXÃO", "Houve uma falha na conexão. Por favor, tente novamente mais tarde.", "OK");
		}

		yield return null;
	}

	private bool CheckFields ()
	{
		return UtilsService.CheckEmail(emailField.text) &&
			   UtilsService.CheckPassword(passwordField.text);
	}
}
