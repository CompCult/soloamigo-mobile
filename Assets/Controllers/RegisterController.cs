using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : ScreenController
{
	public GameObject institutionFieldObj;
	public InputField nameField, emailField, passwordField, institutionField;
	public Dropdown userTypeDropdown;

	private string TYPE_PLACEHOLDER = "Você é...";

	public void Start()
	{
		previousView = "Login";
	}

	public void Register()
	{
		StartCoroutine(_Register());
	}

	public void CheckUserType ()
	{
		if (userTypeDropdown.captionText.text == "Estudante")
			institutionFieldObj.SetActive(true);
		else if (userTypeDropdown.captionText.text == TYPE_PLACEHOLDER)
			userTypeDropdown.value = 1;
		else
			institutionFieldObj.SetActive(false);
	}

	private IEnumerator _Register()
	{
		if (!CheckFields())
		{
			AlertsService.makeAlert("Campos inválidos", "Preencha todos os campos corretamente antes de registrar-se.", "Entendi");
			yield break;
		}

		AlertsService.makeLoadingAlert("Registrando");
		User newUser = new User();

		newUser.name = nameField.text;
		newUser.email = emailField.text;
		newUser.password = passwordField.text;
		newUser.type = userTypeDropdown.captionText.text;
		if (institutionFieldObj.activeSelf)
			newUser.institution = institutionField.text;

		WWW registerForm = UserService.Register(newUser);

		while (!registerForm.isDone)
			yield return new WaitForSeconds(1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + registerForm.responseHeaders["STATUS"]);
		Debug.Log("Text: " + registerForm.text);

		if (registerForm.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.UpdateLocalUser(registerForm.text);
			yield return StartCoroutine(_GetUserPhoto());
		}
		else
		{
			if (registerForm.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("E-mail em uso", "O endereço de e-mail inserido está em uso, tente um diferente.", "Entendi");
			else
				AlertsService.makeAlert("Falha na conexão", "Ocorreu um erro inesperado. Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _GetUserPhoto ()
	{
		string photoUrl = UserService.user.picture,
					 param = UtilsService.GetParam("Email");

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
		PlayerPrefs.SetString(param, emailField.text);
		LoadView("Home");
	}

	private bool CheckFields()
	{
		bool validInstitution;
		if (institutionFieldObj.activeSelf)
			if (institutionField.text.Length > 3)
				validInstitution = true;
			else
				validInstitution = false;
		else
			validInstitution = true;

		return UtilsService.CheckName(nameField.text) &&
			   UtilsService.CheckEmail(emailField.text) &&
			   UtilsService.CheckPassword(passwordField.text) &&
			   userTypeDropdown.captionText.text != "Você é..." &&
			   validInstitution;
	}
}
