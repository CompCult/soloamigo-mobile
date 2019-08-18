using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : ScreenController 
{
	public GameObject optionCard;
	public Text quizTitle, quizDescription;

	public void Start ()
	{
		previousView = "Quizzes";
		optionCard.SetActive(false);

		quizTitle.text = QuizzesService.quiz.title;
		quizDescription.text = QuizzesService.quiz.description;
		
		FillOptions();
	}

	private void FillOptions ()
    {
    	Quiz currentQuiz = QuizzesService.quiz;
     	Vector3 position = optionCard.transform.position;
     	optionCard.SetActive(true);

     	for (int i=0; i < currentQuiz.answers.Count; i++)
     	{
     		string answer = currentQuiz.answers[i];

     		if (answer.Length < 1)
     			continue;

     		position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(optionCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	QuizOptionCard optionCardScript = card.GetComponent<QuizOptionCard>();
        	string alternative = GetAlternative(i);     
        	
        	optionCardScript.UpdateOption(answer, alternative);
     	}

        optionCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

    private string GetAlternative (int i)
    {
    	string alternatives = "abcdefg";
    	return alternatives[i].ToString();
    }

}
