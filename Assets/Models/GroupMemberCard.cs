using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GroupMemberCard : MonoBehaviour
{
	#pragma warning disable 0108

	public Text name, email;
	public GroupMember member;

	public GameObject crownIcon;
	public Button toggleAdmin, removeButton, logoutButton;

	public bool isCurrentUser;

	private Color COLOR_GREEN = new Color(0.07450981f, 0.7568628f, 0.3333333f),
				  COLOR_RED = new Color(0.8745099f, 0.3058824f, 0.3647059f);

	public void UpdateMember(GroupMember member)
	{
		User currentUser = UserService.user;
		this.member = member;

		Image crownBg = toggleAdmin.GetComponent<Image>();
		if (member.is_admin)
		{
			crownIcon.SetActive(true);
			crownBg.color = COLOR_RED;
		}
		else 
		{
			crownIcon.SetActive(false);
			crownBg.color = COLOR_GREEN;
		}

		if (currentUser._id == member._user)
		{
			isCurrentUser = true;

			logoutButton.gameObject.SetActive(true);
			toggleAdmin.gameObject.SetActive(false);
			removeButton.gameObject.SetActive(false);

			UpdateFields();
		}
		else
		{
			isCurrentUser = false;

			if (GroupsService.CurrentUserIsAdmin())
			{
				logoutButton.gameObject.SetActive(false);
				toggleAdmin.gameObject.SetActive(true);
				removeButton.gameObject.SetActive(true);
			}
			else
			{
				logoutButton.gameObject.SetActive(false);
				toggleAdmin.gameObject.SetActive(false);
				removeButton.gameObject.SetActive(false);
			}

			StartCoroutine(_GetMemberInfo());
		}
	}

	public void UpdateFields()
	{
		if (isCurrentUser)
		{
			User user = UserService.user;
			name.text = user.name;
			email.text = user.email;
		}
		else
		{
			name.text = member.info.name;
			email.text = member.info.email;
		}
	}

	public void ToggleAdmin ()
	{
		StartCoroutine(_ToggleAdmin());
	}

	public void RemoveMember ()
	{
		StartCoroutine(_RemoveMember());
	}

	private IEnumerator _RemoveMember ()
	{
		AlertsService.makeLoadingAlert("Removendo");
		WWW removeRequest = GroupsService.RemoveMember(member._id);

		while (!removeRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + removeRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + removeRequest.text);
		AlertsService.removeLoadingAlert();

		if (removeRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			if (isCurrentUser)
				SceneManager.LoadScene("Groups");
			else
				Destroy(this.gameObject);

			yield return null;
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _ToggleAdmin ()
	{
		member.is_admin = !member.is_admin;
		WWW updateRequest = GroupsService.UpdateMember(member);

		while (!updateRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + updateRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + updateRequest.text);

		if (updateRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UpdateMember(member);
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _GetMemberInfo ()
	{
		WWW infoRequest = UserService.GetUser(member._user);

		while (!infoRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + infoRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + infoRequest.text);

		if (infoRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			member.info = JsonUtility.FromJson<User>(infoRequest.text);
			UpdateFields();
		}
		else 
		{
			this.gameObject.SetActive(false);
		}

		yield return null;
	}
}
