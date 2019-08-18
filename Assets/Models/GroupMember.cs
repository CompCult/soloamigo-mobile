[System.Serializable]
public class GroupMember 
{
	public int 
	_id,
	_user,
	_group;

	public bool 
	is_admin=false,
	joined_at;

	public User info;
}
