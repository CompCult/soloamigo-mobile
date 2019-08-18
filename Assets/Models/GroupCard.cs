using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GroupCard : MonoBehaviour
{
	#pragma warning disable 0108

	public Text name, description;
	public Group group;

	public void UpdateGroupCard (Group group)
	{
		this.group = group;

		name.text = group.name;

		if (group.description != null)
			if (group.description.Length >= 23)
				description.text = group.description.Substring(0, 20) + "...";
			else
        		description.text = group.description;
        else
        	description.text = "Sem descrição";
	}

   	public void OpenGroup ()
	{
		GroupsService.UpdateCurrentGroup(group);
		SceneManager.LoadScene("Group");
	}

	public void ExitGroup ()
	{
		StartCoroutine(_ExitGroup());
	}

	private IEnumerator _ExitGroup ()
	{
		AlertsService.makeLoadingAlert("Saindo");
		WWW exitRequest = GroupsService.RemoveMember(UserService.user._id);

		while (!exitRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + exitRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + exitRequest.text);
		AlertsService.removeLoadingAlert();

		if (exitRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			AlertsService.makeAlert("Sucesso", "Você saiu do grupo.", "");
			yield return new WaitForSeconds(2f);
			SceneManager.LoadScene("Groups");
			yield return null;
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
			SceneManager.LoadScene("Home");
		}

		yield return null;
	}
}
