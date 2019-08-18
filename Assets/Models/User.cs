using System;
using UnityEngine;

[System.Serializable]
public class User 
{
	public int 
	_id=0,
	points=0,
	sec_points=0,
	request_limit=5;

	public string 
	name="",
	type="",
	picture="",
	email="",
	password="",
	birth="",
	institution="",
	sex="",
	phone="",
	street="",
	complement="",
	number="",
	neighborhood="",
	city="",
	state="",
	zipcode="",
	banned_until="",
	created_at="";

	public Texture2D profilePicture;

	public bool IsMinor()
	{
		if (birth.Length != 10)
			return false;

		string[] date = birth.Split('/');
		if (date.Length == 3)
		{
			DateTime todaysDate = DateTime.Now.Date;
			int minimumYear = todaysDate.Year - 18;
			int birthYear = int.Parse(date[2]);

			if (birthYear > minimumYear)
				return true;
		}

		return false;
	}
}
