using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizzesController : ScreenController 
{
	public GameObject quizCard, noQuizzesCard;
	public InputField secretCodeField;

	public void Start ()
	{
		TutorialService.CheckTutorial("Quizzes");

		previousView = "Home";
		quizCard.SetActive(false);
		noQuizzesCard.SetActive(false);
		
		StartCoroutine(_GetQuizzes());
	}

	public void SearchQuiz ()
	{
		StartCoroutine(_SearchQuiz());
	}

	private IEnumerator _SearchQuiz ()
	{
		if (!CheckFields())
		{
			AlertsService.makeAlert("Código inválido", "Digite um código secreto com pelo menos quatro caracteres para realizar a busca.", "Entendi");
			yield break;
		}

		AlertsService.makeLoadingAlert("Buscando");
		
		string secretCode = secretCodeField.text;
		User currentUser = UserService.user;

		WWW quizRequest = QuizzesService.SearchQuiz(currentUser._id, secretCode);

		while (!quizRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + quizRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + quizRequest.text);

		if (quizRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			QuizzesService.UpdateQuiz(quizRequest.text);
			LoadView("Quiz");
		}
		else 
		{
			AlertsService.makeAlert("Não encontrado", "Não encontramos nenhum quiz com esse código secreto. Por favor, verifique o código e tente novamente.", "OK");
		}

		yield return null;
	}

	private IEnumerator _GetQuizzes ()
	{
		User currentUser = UserService.user;

		AlertsService.makeLoadingAlert("Recebendo quizzes");
		WWW quizzesRequest = QuizzesService.GetQuizzes(currentUser._id);

		while (!quizzesRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + quizzesRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + quizzesRequest.text);

		if (quizzesRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			QuizzesService.UpdateQuizzes(quizzesRequest.text);
			CreateQuizzesCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Activities");
		}

		yield return null;
	}

	private void CreateQuizzesCards ()
    {
     	Vector3 position = quizCard.transform.position;

     	if (QuizzesService.quizzes.Length > 0)
     		quizCard.SetActive(true);
     	else
     		noQuizzesCard.SetActive(true);

     	foreach (Quiz quiz in QuizzesService.quizzes)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(quizCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	QuizCard quizCardScript = card.GetComponent<QuizCard>();
        	quizCardScript.UpdateQuizCard(quiz);
        }

        quizCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

    private bool CheckFields()
	{
		return UtilsService.CheckName(secretCodeField.text);
	}

}
