using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalTimelinePost : ModalGeneric 
{
	public CameraCaptureService camService;
	public GameObject rotateButton;
	public InputField newMessageField;

	private string STATUS_OK = "OK";

	public void Start ()
	{
		camService.resetFields("pot_seeding");
		StartCoroutine(_CheckCapturedPhoto());
	}

	public void SendNewPost()
	{
		StartCoroutine(_SendNewPost());
	}

	private IEnumerator _SendNewPost ()
	{
		AlertsService.makeLoadingAlert("Enviando");

		int userId = UserService.user._id;
		string imageBase64 = camService.photoBase64,
			   message = newMessageField.text;

		WWW postForm = TimelineService.NewPost(userId, imageBase64, message);

		while (!postForm.isDone)
			yield return new WaitForSeconds(1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + postForm.responseHeaders["STATUS"]);
		Debug.Log("Text: " + postForm.text);

		if (postForm.responseHeaders["STATUS"] == HTML.HTTP_200)
			ReloadView();
		else
			AlertsService.makeAlert("Falha na conexão", "Ocorreu um problema ao enviar sua publicação. Tente novamente.", "Entendi");

		yield return null;
	}

	private IEnumerator _CheckCapturedPhoto ()
	{
		if (camService.photoBase64 != null)
		{
			rotateButton.SetActive(true);
			sendButton.interactable = true;
		}
		
		else
		{
			rotateButton.SetActive(false);
			sendButton.interactable = false;
		}

		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(_CheckCapturedPhoto());
	}

	private string CheckFields()
    {
    	string message = STATUS_OK;

    	if (camService.photoBase64 == null)
			message = "Você não selecionou uma foto para sua postagem.";

		if (newMessageField.text.Length < 2)
			message = "Escreva uma mensagem para sua publicação.";

		return message;
    }
}
