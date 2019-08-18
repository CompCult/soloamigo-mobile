using System.Collections.Generic;

[System.Serializable]
public class Quiz 
{
	public int 
	_id,
	points;

	public string 
	title,
	description,
	secret_code,
	alternative_a="",
	alternative_b="",
	alternative_c="",
	alternative_d="",
	alternative_e="",
	correct_answer="",
	start_time,
	end_time,
	created_at;

	public bool
	is_public,
	single_answer;

	public User _user;

	public List<string> answers = new List<string>();

	public void BuildAnswers()
	{
		answers = new List<string>();

		answers.Add(alternative_a);
		answers.Add(alternative_b);
		answers.Add(alternative_c);
		answers.Add(alternative_d);
		answers.Add(alternative_e);
	}

	public bool HasCorrectAnswer()
	{
		return (correct_answer.Length > 0);
	}
}
