using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupController : ScreenController 
{
	public GameObject memberCard, newMemberCard, newMessageCard, confirmationCard;
	public Button newMemberBtn, newMessageBtn, removeGroupBtn, editGroup, confirmEdit, cancelEdit;
	public Text confirmationMessage;

	// Group info
	public InputField groupName, groupDescription;

	// New Member
	public InputField newMemberEmail;

	// New email
	public InputField newMessage;

	private Color COLOR_WHITE = new Color(1f, 1f, 1f),
			  COLOR_GREY = new Color(0.9150943f, 0.9150943f, 0.9150943f);

	public void Start ()
	{
		previousView = "Groups";

		memberCard.SetActive(false);
		newMemberCard.SetActive(false);
		newMessageCard.SetActive(false);
		confirmEdit.gameObject.SetActive(false);
		cancelEdit.gameObject.SetActive(false);
		editGroup.gameObject.SetActive(false);
		confirmationCard.gameObject.SetActive(false);
	    newMemberBtn.gameObject.SetActive(true);
		newMessageBtn.gameObject.SetActive(false);
		removeGroupBtn.gameObject.SetActive(false);

		UpdateFields();
		StartCoroutine(_GetGroupMembers());
	}

	public void SendMessage ()
	{
		StartCoroutine(_SendMessage());
	}

	public void RemoveGroup ()
	{
		StartCoroutine(_RemoveGroup());
	}

	public void AddMember()
	{
		StartCoroutine(_AddMember());
	}

	public void UpdateFields ()
	{
		groupName.interactable = false;
		groupDescription.interactable = false;

		Group currentGroup = GroupsService.group;
		groupName.text = currentGroup.name;
		groupDescription.text = currentGroup.description;
	}

	public void ConfirmChanges ()
	{
		Group currentGroup = GroupsService.group;
		currentGroup.name = groupName.text;
		currentGroup.description = groupDescription.text;

		Debug.Log("groupName.text: " + groupName.text);

		GroupsService.UpdateCurrentGroup(currentGroup);
		StartCoroutine(_UpdateGroupInfo());

		ToggleEditGroup();
	}

	public void ToggleEditGroup()
	{
		Image nameBg = groupName.GetComponent<Image>(),
			  descriptionBg = groupDescription.GetComponent<Image>();

		if (editGroup.gameObject.activeSelf)
		{
			editGroup.gameObject.SetActive(false);
			confirmEdit.gameObject.SetActive(true);
			cancelEdit.gameObject.SetActive(true);

			groupName.interactable = true;
			nameBg.color = COLOR_GREY;

			groupDescription.interactable = true;
			descriptionBg.color = COLOR_GREY;
		}
		else
		{
			editGroup.gameObject.SetActive(true);
			confirmEdit.gameObject.SetActive(false);
			cancelEdit.gameObject.SetActive(false);

			nameBg.color = COLOR_WHITE;
			descriptionBg.color = COLOR_WHITE;

			UpdateFields();
		}
	}

	public void ToggleConfirmationCard ()
	{
		confirmationMessage.text = "Deseja mesmo excluir o grupo?";
		confirmationCard.SetActive(!confirmationCard.activeSelf);
	}

	public void ToggleNewMemberCard ()
	{
		newMemberCard.SetActive(!newMemberCard.activeSelf);
	}

	public void ToggleNewMessageCard ()
	{
		newMessageCard.SetActive(!newMessageCard.activeSelf);
	}

	private IEnumerator _SendMessage ()
	{
		if (!CheckMessageFields())
		{
			AlertsService.makeAlert("Campos inválidos", "Certifique-se de que digitou uma mensagem minimamente válida para o seu grupo.", "Entendi");
			yield break;
		}

		AlertsService.makeLoadingAlert("Enviando mensagem");

		User currentUser = UserService.user;
		Group currentGroup = GroupsService.group;
		string message = newMessage.text;

		WWW messageRequest = GroupsService.SendMessage(currentGroup, currentUser, message);

		while (!messageRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + messageRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + messageRequest.text);

		if (messageRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
			AlertsService.makeAlert("Mensagem enviada", "Sua mensagem será enviada por e-mail em alguns instantes para todos os integrantes do grupo.", "OK");
		else 
			AlertsService.makeAlert("Falha ao enviar", "Houve uma falha em sua conexão. Tente novamente mais tarde.", "Entendi");

		yield return null;
	}

	private IEnumerator _UpdateGroupInfo ()
	{
		Group currentGroup = GroupsService.group;
		WWW updateRequest = GroupsService.UpdateGroup(currentGroup);

		while (!updateRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + updateRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + updateRequest.text);

		if (updateRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UpdateFields();
		}
		else 
		{
			AlertsService.makeAlert("Falha ao atualizar", "Houve uma falha em sua conexão. Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _AddMember ()
	{
		AlertsService.makeLoadingAlert("Adicionando");

		string userEmail = newMemberEmail.text;
		int groupId = GroupsService.group._id;
		bool isAdmin = false;

		WWW createRequest = GroupsService.AddMember(userEmail, groupId, isAdmin);

		while (!createRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + createRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + createRequest.text);

		if (createRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			AlertsService.makeAlert("Sucesso", "O usuário foi adicionado com sucesso em seu grupo.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Group");
			yield return null;
		}
		else 
		{
			AlertsService.makeAlert("Falha ao adicionar", "Verifique se inseriu o endereço de e-mail do usuário corretamente.", "Entendi");
		}

		yield return null;
	} 

	private IEnumerator _GetGroupMembers ()
	{
		int groupId = GroupsService.group._id;
		WWW membersRequest = GroupsService.GetMembers(groupId);

		while (!membersRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + membersRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + membersRequest.text);

		if (membersRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			GroupsService.UpdateGroupMembers(membersRequest.text);
			CreateMembersCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
			LoadView("Home");
		}

		yield return null;
	}

	private IEnumerator _RemoveGroup ()
	{
		AlertsService.makeLoadingAlert("Removendo");
		int groupId = GroupsService.group._id;

		WWW removeRequest = GroupsService.RemoveGroup(groupId);

		while (!removeRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + removeRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + removeRequest.text);

		if (removeRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
			LoadView("Groups");
		else 
			AlertsService.makeAlert("Falha ao remover", "Verifique sua conexão com a internet e tente novamente mais tarde.", "Entendi");

		yield return null;
	}

	private void CreateMembersCards ()
    {
     	Vector3 position = memberCard.transform.position;
     	memberCard.gameObject.SetActive(true);

     	foreach (GroupMember member in GroupsService.group.members)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(memberCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	GroupMemberCard memberCardScript = card.GetComponent<GroupMemberCard>();
        	memberCardScript.UpdateMember(member);
        }

        CheckAuthority();
        memberCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

    private void CheckAuthority ()
    {
    	if (GroupsService.CurrentUserIsAdmin())
    	{
    		newMemberBtn.gameObject.SetActive(true);
    		newMessageBtn.gameObject.SetActive(true);
    		removeGroupBtn.gameObject.SetActive(true);
    		editGroup.gameObject.SetActive(true);
    	}
    	else
    	{
    		newMemberBtn.gameObject.SetActive(true);
    		newMessageBtn.gameObject.SetActive(false);
    		removeGroupBtn.gameObject.SetActive(false);
    		editGroup.gameObject.SetActive(false);
    	}
    }

    private bool CheckMessageFields()
    {
    	if (newMessage.text.Length < 3)
			return false;

		return true;
    }
}
