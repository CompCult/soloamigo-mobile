using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class FORMATTER : MonoBehaviour 
{
	private InputField inputField;

	public string type;
	public bool upperCase;

	private string regex,
	phoneRegex = "^[0-9]|[-]|[()]",
	cpfRegex = "^[0-9]|[.]|[-]",
	dateRegex = "^[0-9]|[/]";

	private int stringSize;

	public void Start()
	{
		inputField = GetComponent<InputField>();
		stringSize = inputField.text.Length;

		CheckRegexType();

		inputField.onValueChanged.AddListener (delegate {LockInput (inputField);});
		inputField.onValueChanged.AddListener (delegate {CheckRegex (inputField);});
	}

	private void CheckRegexType()
	{
		switch (type)
		{
			case "phone": 
				regex = phoneRegex; break;
			case "cpf": 
				regex = cpfRegex; break;
			case "date":
				regex = dateRegex; break;
		}
	}

	private void LockInput(InputField input)
	{
		switch (type)
		{
			case "phone": 
				FormatPhone(input); 
				break;
			case "cpf": 
				FormatCPF(input); 
				break;
			case "date":
				FormatDate(input);
				break;
		}

		stringSize = input.text.Length;
	}

	private void CheckRegex(InputField input)
	{
		if (regex == null)
			return;

		if (input.text.Length > 0 && !Regex.IsMatch(input.text, regex))
		{
			Debug.Log("Incorrect char");
			input.text = input.text.Substring(0, input.text.Length - 1);
			return;
		}
	}

	private void FormatPhone(InputField input)
	{
		int newSize = input.text.Length;

		if (newSize > stringSize)
		{
			if (input.text.Length == 2)
				input.text = "(" + input.text + ")";

			input.caretPosition = input.text.Length + 1;
		}
	}

	private void FormatCPF(InputField input)
	{
		if (input.text.Length == 11)
		{
			input.characterLimit = 14;
			input.text = string.Format("{0}.{1}.{2}-{3}", input.text.Substring(0, 3), input.text.Substring(3, 3), input.text.Substring(6, 3), input.text.Substring(9, 2));
		}
	}

	private void FormatDate(InputField input)
	{
		int newSize = input.text.Length;

		if (newSize > stringSize && (input.text.Length == 2 || input.text.Length == 5))
		{
			input.text = input.text + "/";
			input.caretPosition = newSize + 1;
		}
	}
}