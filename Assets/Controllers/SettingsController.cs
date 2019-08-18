using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : ScreenController
{
	public GameObject personalPage, addressPage;
	public InputField passField;

	// Personal
	public InputField nameField, emailField, birthField, phoneField;
	public Dropdown genreDropdown;

	// Address
	public InputField streetField, numberField, neighborhoodField, cityField, complementField, zipField;
	public Dropdown stateDropdown;

	// Photo
	public CameraCaptureService camService;
	public GameObject takePhotoButton, cancelPhotoButton;

	private string STATUS_OK = "OK";
	private Texture originalPhoto;

	public void Start ()
	{
		previousView = "Home";

		camService.resetFields("default_avatar");

		StartCoroutine(_CheckPhotoStatus());
		FillInputFields();
	}

	public void CancelPhoto ()
	{
		camService.resetFields("default_avatar");
		camService.pickPreiveimage.texture = originalPhoto;

		UtilsService.SizeToParent(camService.pickPreiveimage, 0f);
	}

	public void ToggleDataToShow()
	{
		if (personalPage.activeSelf)
		{
			personalPage.SetActive(false);
			addressPage.SetActive(true);
		}
		else
		{
			personalPage.SetActive(true);
			addressPage.SetActive(false);
		}
	}

	public void Logout ()
	{
		string param = UtilsService.GetParam("Email");
		PlayerPrefs.DeleteKey(param);

		LoadView("Login");
	}

	public void SaveChanges()
	{
		string personalInfoStatus = CheckPersonalInfo(),
			   addressStatus = CheckAddress();

		if (personalInfoStatus != STATUS_OK)
		{
			AlertsService.makeAlert("Dados pessoais inválidos", personalInfoStatus, "Entendi");
			return;
		}

		if (addressStatus != STATUS_OK)
		{
			AlertsService.makeAlert("Endereço inválido", addressStatus, "Entendi");
			return;
		}

		User aux = UserService.user;

		if (camService.photoBase64 != null)
			UserService.user.profilePicture = camService.pickPreiveimage.texture as Texture2D;

		if (!aux.name.Equals(nameField.text))
			aux.name = nameField.text;

		if (!aux.name.Equals(nameField.text))
			aux.name = nameField.text;

		if (!aux.birth.Equals(birthField.text))
			aux.birth = birthField.text;

		if (!genreDropdown.captionText.text.Equals("Gênero"))
			aux.sex = genreDropdown.captionText.text;

		if (!aux.phone.Equals(phoneField.text))
			aux.phone = phoneField.text;

		if (!aux.street.Equals(streetField.text))
			aux.street = streetField.text;

		if (!aux.neighborhood.Equals(neighborhoodField.text))
			aux.neighborhood = neighborhoodField.text;

		if (numberField.text.Length > 0)
			aux.number = numberField.text;

		if (!aux.complement.Equals(complementField.text))
			aux.complement = complementField.text;

		if (!aux.zipcode.Equals(zipField.text))
			aux.zipcode = zipField.text;

		if (!aux.city.Equals(cityField.text))
			aux.city = cityField.text;

		if (!aux.state.Equals(stateDropdown.captionText.text))
			aux.state = stateDropdown.captionText.text;

		if (!aux.password.Equals(passField.text))
			aux.password = passField.text;

		AlertsService.makeLoadingAlert("Atualizando dados");
		StartCoroutine(_SaveChanges(aux));
	}

	private IEnumerator _CheckPhotoStatus()
	{
		if (camService.photoBase64 != null)
		{
			takePhotoButton.SetActive(false);
			cancelPhotoButton.SetActive(true);
		}
		else
		{
			takePhotoButton.SetActive(true);
			cancelPhotoButton.SetActive(false);
		}

		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(_CheckPhotoStatus());
	}

	private void FillInputFields()
	{
		User aux = UserService.user;

		camService.pickPreiveimage.texture = UserService.user.profilePicture;
		originalPhoto = camService.pickPreiveimage.texture;

		if (aux.name.Length > 0)
			nameField.text = aux.name;
		if (aux.email.Length > 0)
			emailField.text = aux.email;
		if (aux.birth.Length > 0)
			birthField.text = UtilsService.GetDate(aux.birth);
		if (aux.sex.Length > 0)
			genreDropdown.value = FindIndexFromGenre(aux);
		if (aux.phone.Length > 0)
			phoneField.text = aux.phone;
		if (aux.street.Length > 0)
			streetField.text = aux.street;
		if (aux.neighborhood.Length > 0)
			neighborhoodField.text = aux.neighborhood;
		if (aux.complement.Length > 0)
			complementField.text = aux.complement;
		if (aux.zipcode.Length > 0)
			zipField.text = aux.zipcode;
		if (aux.city.Length > 0)
			cityField.text = aux.city;
		if (aux.state.Length > 0)
			stateDropdown.value = FindIndexFromState(aux);
		if (aux.number != null)
			numberField.text = aux.number.ToString();
	}

	private IEnumerator _SaveChanges (User aux)
	{
		string photoBase64 = camService.photoBase64;

		WWW updateResponse = UserService.Update(aux, photoBase64);

		while (!updateResponse.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + updateResponse.responseHeaders["STATUS"]);
		Debug.Log("Text: " + updateResponse.text);
		AlertsService.removeLoadingAlert();

		if (updateResponse.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			AlertsService.makeAlert("PERFIL ATUALIZADO", "Suas informações foram atualizadas com sucesso.", "");
			UserService.UpdateLocalUser(aux);
			yield return new WaitForSeconds(3f);
			ReloadView();
		}
		else
		{
			AlertsService.makeAlert("Falha na conexão", "Verifique sua conexão e tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private int FindIndexFromState(User user)
	{
		for (int i=0; i < stateDropdown.options.Count; i++)
			if (stateDropdown.options[i].text.Equals(user.state))
				return i;

		return 0;
	}

	private int FindIndexFromGenre(User user)
	{
		for (int i=0; i < genreDropdown.options.Count; i++)
			if (genreDropdown.options[i].text.Equals(user.sex))
				return i;

		return 0;
	}

	private string CheckPersonalInfo ()
	{
		User aux = UserService.user;
		string message = STATUS_OK;

		if (!aux.name.Equals(nameField.text) && nameField.text.Length < 3)
			message = "O nome deve conter, pelo menos, três caracteres.";

		if (!aux.email.Equals(emailField.text) && !UtilsService.CheckEmail(emailField.text))
			message = "Insira um e-mail num formato válido.";

		if (!aux.phone.Equals(phoneField.text))
			if (phoneField.text.Length < 10)
				message = "Insira um telefone num formato válido.";

		if (!aux.birth.Equals(birthField.text) && birthField.text.Length > 0)
		{
			if (birthField.text.Length != 10)
				message = "Insira uma data de nascimento válida.";

			string[] birthDate = birthField.text.Split('/');

			if (birthDate.Length != 3)
				message = "Insira uma data da nascimento válida.";
			else
			{
				int year = birthDate[2].Length == 4 ? int.Parse(birthDate[2]) : 0,
					month = birthDate[1].Length == 2 ? int.Parse(birthDate[1]) : 0,
					day = birthDate[0].Length == 2 ? int.Parse(birthDate[0]) : 0;

				if (birthDate[2].Length >= 2 && year > System.DateTime.Now.Year)
					message = "Verifique se inseriu o ano correto em seu registro.";

				if (birthDate[1].Length == 2 && month > 12 || month < 1)
					message = "Verifique se inseriu o mês correto em seu registro.";

				if (birthDate[0].Length == 2 && day > 31 || day < 1)
					message = "Verifique se inseriu o dia correto em seu registro.";
			}
		}

		if (aux.sex.Length > 0)
			if (genreDropdown.captionText.text.Equals("Gênero"))
				message = "Por favor, selecione um gênero válido.";

		if (passField.text.Length < 6)
			message = "A senha deve conter, pelo menos, seis caracteres.";

		if (passField.text.Length == 0)
			message = "Insira sua senha (ou uma nova senha) no campo específico para confirmar suas mudanças.";

		return message;
	}

	private string CheckAddress ()
	{
		User aux = UserService.user;
		string message = STATUS_OK;

		if (!aux.street.Equals(streetField.text))
			if (streetField.text.Length < 3)
				message = "O campo de rua deve conter, pelo menos, três caracteres.";

		if (!aux.neighborhood.Equals(neighborhoodField.text))
			if (neighborhoodField.text.Length < 3)
				message = "O campo de bairro deve conter, pelo menos, três caracteres.";

		if (!aux.city.Equals(cityField.text))
			if (cityField.text.Length < 3)
				message = "O campo de cidade deve conter, pelo menos, três caracteres.";

		if (!aux.zipcode.Equals(zipField.text))
			if (zipField.text.Length != 8)
				message = "O CEP deve possuir oito caracteres.";

		return message;
	}

}
