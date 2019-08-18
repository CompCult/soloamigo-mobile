using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class GroupsService
{
	private static Group[] _groups;
	public static Group[] groups { get { return _groups; } }

	private static Group _group;
	public static Group group { get { return _group; } }

	public static bool isAdmin;

	public static WWW CreateGroup (string name, string description)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("name", name);
		if (description != null && description.Length > 0)
			requestForm.AddField ("description", description);

		WebService.route = ENV.GROUPS_ROUTE;
		WebService.action = "";

		return WebService.Post(requestForm);
	}

	public static WWW UpdateGroup (Group group)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("name", group.name);
		requestForm.AddField ("description", group.description);

		WebService.route = ENV.GROUPS_ROUTE;
		WebService.action = ENV.UPDATE_ACTION;
		WebService.id = group._id.ToString();

		return WebService.Post(requestForm);
	}

	public static WWW RemoveGroup (int id)
	{
		WWWForm requestForm = new WWWForm ();

		WebService.route = ENV.GROUPS_ROUTE;
		WebService.action = ENV.REMOVE_ACTION;
		WebService.id = id.ToString();

		return WebService.Post(requestForm);
	}

	public static WWW GetUserGroups (int userId)
	{
		WebService.route = ENV.GROUP_MEMBERS_ROUTE;
		WebService.action = ENV.GROUPS_ACTION +
							"?_user=" + userId;

		return WebService.Get();
	}

	public static WWW SendMessage (Group group, User user, string message)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("_group", group._id);
		requestForm.AddField ("author", user.name);
		requestForm.AddField ("message", message);

		WebService.route = ENV.GROUPS_ROUTE;
		WebService.action = ENV.EMAIL_ACTION;

		return WebService.Post(requestForm);
	}

	public static void UpdateCurrentGroups (string json)
	{
		_groups = UtilsService.GetJsonArray<Group>(json);
	}

	public static void UpdateCurrentGroup (string json)
	{
		_group = JsonUtility.FromJson<Group>(json);
	}

	public static void UpdateCurrentGroup (Group group)
	{
		_group = group;
	}

	public static List<string> GetGroupNames ()
	{
		List<string> groupNames = new List<string>();
		foreach (Group group in _groups)
		{
			Debug.Log("Added: " + group.name);
			groupNames.Add(group.name);
		}

		return groupNames;
	}

	public static bool CurrentUserIsAdmin()
	{
		User currentUser = UserService.user;

		if (groups != null && groups.Length > 0)
			foreach (GroupMember member in group.members)
				if (currentUser._id == member._user)
					return member.is_admin;

		return false;

	}

	//------------------------ MEMBER ACTIONS

	public static WWW GetMembers (int groupId)
	{
		WebService.route = ENV.GROUP_MEMBERS_ROUTE;
		WebService.action = ENV.QUERY_ACTION +
							"_group=" + groupId;

		return WebService.Get();
	}

	public static WWW AddMember (string memberEmail, int groupId, bool isAdmin)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("email", memberEmail);
		requestForm.AddField ("_group", groupId);
		requestForm.AddField ("is_admin", isAdmin ? 1 : 0);

		WebService.route = ENV.GROUP_MEMBERS_ROUTE;

		return WebService.Post(requestForm);
	}

	public static WWW UpdateMember (GroupMember member)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("is_admin", member.is_admin ? 1 : 0);

		WebService.route = ENV.GROUP_MEMBERS_ROUTE;
		WebService.action = ENV.UPDATE_ACTION;
		WebService.id = member._id.ToString();

		return WebService.Post(requestForm);
	}

	public static WWW RemoveMember (int id)
	{
		WWWForm requestForm = new WWWForm ();

		WebService.route = ENV.GROUP_MEMBERS_ROUTE;
		WebService.action = ENV.REMOVE_ACTION;
		WebService.id = id.ToString();

		return WebService.Post(requestForm);
	}

	public static void UpdateGroupMembers (string json)
	{
		if (group != null)
			group.members = UtilsService.GetJsonArray<GroupMember>(json);
	}

}
