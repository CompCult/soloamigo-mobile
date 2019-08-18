[System.Serializable]
public class Mission 
{
	public int 
	_id,
	points;

	public string 
	name,
	description,
	end_message,
	secret_code,
	start_time,
	end_time,
	created_at;

	public bool
	is_public,
	is_grupal,
	single_answer,
	has_image,
	has_audio,
	has_video,
	has_text,
	has_geolocation;

	public User _user;
}
