using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupsController : ScreenController 
{
	public GameObject groupCard, newGroupCard, noGroupsCard;
	public Text groupName, groupDescription;
	public InputField newGroupName, newGroupDescription;

	public void Start ()
	{
		TutorialService.CheckTutorial("Groups");

		previousView = "Home";
		groupCard.SetActive(false);
		noGroupsCard.SetActive(false);
		newGroupCard.SetActive(false);
		
		StartCoroutine(_GetGroups());
	}

	public void ToggleNewGroupCard ()
	{
		newGroupCard.SetActive(!newGroupCard.activeSelf);
	}

	public void CreateGroup()
	{
		StartCoroutine(_CreateGroup());
	}

	private IEnumerator _CreateGroup ()
	{
		if (!CheckFields())
		{
			AlertsService.makeAlert("Campos inválidos", "Preencha os campos corretamente.", "OK");
			yield break;
		}

		AlertsService.makeLoadingAlert("Criando grupo");

		string groupName = newGroupName.text,
			   groupDescription = newGroupDescription.text;

		WWW createRequest = GroupsService.CreateGroup(groupName, groupDescription);

		while (!createRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + createRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + createRequest.text);

		if (createRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			GroupsService.UpdateCurrentGroup(createRequest.text);
			yield return StartCoroutine(_AddOwnership());
		}
		else 
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");

		yield return null;
	}

	private IEnumerator _AddOwnership ()
	{
		User currentUser = UserService.user;
		int groupId = GroupsService.group._id;
		bool isAdmin = true;

		WWW createRequest = GroupsService.AddMember(currentUser.email, groupId, isAdmin);

		while (!createRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + createRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + createRequest.text);

		if (createRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			LoadView("Groups");
		}
		else 
		{
			AlertsService.makeAlert("Erro", "Falha em sua conexão. Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Groups");
		}

		yield return null;
	} 

	private IEnumerator _GetGroups ()
	{
		AlertsService.makeLoadingAlert("Recebendo grupos");
		User currentUser = UserService.user;
		WWW groupsRequest = GroupsService.GetUserGroups(currentUser._id);

		while (!groupsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + groupsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + groupsRequest.text);

		if (groupsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			GroupsService.UpdateCurrentGroups(groupsRequest.text);
			CreateGroupsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private void CreateGroupsCards ()
    {
     	Vector3 position = groupCard.transform.position;

     	if (GroupsService.groups.Length > 0)
     		groupCard.SetActive(true);
     	else
     		noGroupsCard.SetActive(true);

     	foreach (Group group in GroupsService.groups)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(groupCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	GroupCard groupCardScript = card.GetComponent<GroupCard>();
        	groupCardScript.UpdateGroupCard(group);
        }

        groupCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

    private bool CheckFields()
	{
		return UtilsService.CheckName(newGroupName.text);
	}

}
