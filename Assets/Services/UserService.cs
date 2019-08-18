using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public static class UserService
{
	private static User _user;
	public static User user
	{
        get { return _user; }
    }

	public static WWW Login (string email, string password)
	{
		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("email", email);
		loginForm.AddField ("password", password);

		string paramConnect = UtilsService.GetParam("Conectar"),
					 paramEmail = UtilsService.GetParam("Email"),
					 paramPassword = UtilsService.GetParam("Senha");

		if (PlayerPrefs.HasKey(paramConnect))
		{
			PlayerPrefs.SetString(paramEmail, email);
			PlayerPrefs.SetString(paramPassword, password);
		}

		WebService.route = ENV.USERS_ROUTE;
		WebService.action = ENV.AUTH_ACTION;

		return WebService.Post(loginForm);
	}

	public static WWW Register (User user)
	{
		WWWForm registerForm = new WWWForm();
		registerForm.AddField ("name", user.name);
		registerForm.AddField ("email", user.email);
		Debug.Log("Registrando como " + user.type);
		registerForm.AddField ("type", user.type);
		registerForm.AddField ("password", user.password);
		if (user.institution.Length > 1)
			registerForm.AddField ("institution", user.institution);

		WebService.route = ENV.USERS_ROUTE;
		WebService.action = ENV.REGISTER_ACTION;

		return WebService.Post(registerForm);
	}

	public static WWW Recover (string email, string newPassword)
	{
		WWWForm recoverForm = new WWWForm();
		recoverForm.AddField ("email", email);
		recoverForm.AddField ("new_password", newPassword);

		WebService.route = ENV.USERS_ROUTE;
		WebService.action = ENV.RECOVERY_ACTION;

		return WebService.Post(recoverForm);
	}

	public static WWW Update (User user, string photoBase64)
	{
		WWWForm updateForm = new WWWForm();
		updateForm.AddField ("name", user.name);
		updateForm.AddField ("email", user.email);
		updateForm.AddField ("sex", user.sex);
		updateForm.AddField ("phone", user.phone);
		updateForm.AddField ("birth", UtilsService.GetInverseDate(user.birth));
		updateForm.AddField ("password", user.password);
		updateForm.AddField ("street", user.street);
		updateForm.AddField ("complement", user.complement);
		updateForm.AddField ("number", user.number);
		updateForm.AddField ("neighborhood", user.neighborhood);
		updateForm.AddField ("city", user.city);
		updateForm.AddField ("state", user.state);
		updateForm.AddField ("zipcode", user.zipcode);

		if (photoBase64 != null)
		{
			Debug.Log ("sending picture to update");
			updateForm.AddField ("picture", photoBase64);
		}

		WebService.route = ENV.USERS_ROUTE;
		WebService.action = ENV.UPDATE_ACTION;
		WebService.id = user._id.ToString();

		return WebService.Post(updateForm);
	}

	public static WWW GetUser (int id)
	{
		WebService.route = ENV.USERS_ROUTE;
		WebService.id = id.ToString();

		return WebService.Get();
	}

	public static WWW UpdatePoints (User user, int newPoints)
	{
		WWWForm updateForm = new WWWForm();
		updateForm.AddField ("_id", user._id);
		updateForm.AddField ("points", user.points + newPoints);

		WebService.route = ENV.USERS_ROUTE;
		WebService.action = ENV.UPDATE_ACTION;
		WebService.id = user._id.ToString();

		return WebService.Post(updateForm);
	}

	public static void UpdateLocalUser (User newUser)
	{
		_user = newUser;
	}

	public static void UpdateLocalUser (string json)
	{
		_user = JsonUtility.FromJson<User>(json);
	}

}
