using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class QuizMan : MonoBehaviour {

    public GameControls GCont;

    public GameObject[] QButtons;
    public GameObject QPanel;

    [System.Serializable]
    public struct Quiz
    {
        public string Question;
        public string Q_ID;
        public string[] Answers_ID;
        public string[] Answers;
        public string CorrectAnswer_ID;
        public int TotalQs;
        public Sprite Img;
    }

    public Quiz[] Quizzes;

    public Text QuestionBox;
    public Text[] AnswerBoxes;

    private int currentSet;
    private Animator quizAnim;

    public Image QuizImg;

    private string[] alternativeIDs;

	// Use this for initialization
	void Start () {
        currentSet = -1;
        quizAnim = GetComponent<Animator>();
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(GetQuestions);
        GetQuestions(new MultipleChoiceQuestionList());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetQuestions(MultipleChoiceQuestionList questionList) {
        if(questionList == null || questionList.questions == null) {
            return;
        }
        for(int x = 0; x < Quizzes.Length; x++) {
            Quizzes[x].Question = questionList.questions[x].stem;
            Quizzes[x].TotalQs = questionList.questions[x].alternatives.Length;
            StartCoroutine(GetURL(x, questionList.questions[x].imageURL));
            for (int y = 0; y < Quizzes[x].TotalQs; y++) {
                Quizzes[x].Answers[y] = questionList.questions[x].alternatives[y].text;
                Quizzes[x].Answers_ID[y] = questionList.questions[x].alternatives[y].alternativeId;
                Quizzes[x].Q_ID = questionList.questions[x].questionId;
                Quizzes[x].CorrectAnswer_ID = questionList.questions[x].correctAlternativeId;
            }
        }
    }

    public void skipQuestion()
    {
        GCont.SpeechBubble.GetComponentInChildren<Text>().text = GCont.Days[GCont.CurrentDay][GCont.CurrentNPC].Anger;
        quizAnim.SetTrigger("Exit");
        GCont.FrontChar.SetTrigger("WalkOut");
        GCont.QuestionAsked = false;
        GCont.CurrentProgress++;
        GCont.UpdateCurrentProgress();

        GCont.Smoke.Play();
        GCont.Parts.PartSys.Play();
        GCont.IsPaused = false;
        GCont.Parts.IsPaused = false;

        MultipleChoiceAnswer answer = new MultipleChoiceAnswer();
        answer.alternativeId = "";
        answer.questionId = "";
        LOLSDK.Instance.SubmitAnswer(answer);
    }

    IEnumerator GetURL(int quizNum, string URL) {
        if (string.IsNullOrEmpty(URL)) {
            Quizzes[quizNum].Img = null;
        } else {
            CBUG.Do("Image URL IS: " + URL);
            WWW www = new WWW(URL);
            yield return www;
            if (!string.IsNullOrEmpty(www.error)) {
               Quizzes[quizNum].Img = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            }
        }
    }

    public void CheckAnswer(int answerNum)
    {

        MultipleChoiceAnswer answer = new MultipleChoiceAnswer();
        answer.alternativeId = Quizzes[currentSet].Answers_ID[answerNum];
        answer.questionId = Quizzes[currentSet].Q_ID;
        LOLSDK.Instance.SubmitAnswer(answer);
        if (Quizzes[currentSet].Answers_ID[answerNum] == Quizzes[currentSet].CorrectAnswer_ID) {
            GCont.CurrentScore++;
            GCont.SpeechBubble.GetComponentInChildren<Text>().text = GCont.Days[GCont.CurrentDay][GCont.CurrentNPC].Thanks;
        }else {
            GCont.SpeechBubble.GetComponentInChildren<Text>().text = GCont.Days[GCont.CurrentDay][GCont.CurrentNPC].Anger;
        }
        quizAnim.SetTrigger("Exit");
        GCont.FrontChar.SetTrigger("WalkOut");
        GCont.QuestionAsked = false;
        GCont.CurrentProgress++;
        GCont.UpdateCurrentProgress();

        GCont.Smoke.Play();
        GCont.Parts.PartSys.Play();
        GCont.IsPaused = false;
        GCont.Parts.IsPaused = false;

    }

    public void Init()
    {
        currentSet++;

        QPanel.SetActive(true);
        quizAnim.SetTrigger("Quiz");
        QuestionBox.text = Quizzes[currentSet].Question;
        if(Quizzes[currentSet].Img == null) {
            QuizImg.color = new Color(1f, 1f, 1f, 0f);
        }else {
            QuizImg.sprite = Quizzes[currentSet].Img;
        }
        for (int x = 0; x < AnswerBoxes.Length; x++) {
            AnswerBoxes[x].text = Quizzes[currentSet].Answers[x];
        }
        for(int x = 0; x < AnswerBoxes.Length; x++) {
            if(x >= Quizzes[currentSet].TotalQs) {
                QButtons[x].SetActive(false);
            }else {
                QButtons[x].SetActive(true);
            }
        }
        ////lazy bubble shuffle
        //for (int x = 0; x < AnswerBoxes.Length/2; x++) {
        //    string temp = AnswerBoxes[x].text;
        //    string tempID = Quizzes[currentSet].Answers_ID[x];
        //    //CBUG.Do("Temp is: " + temp);
        //    //CBUG.Do("Temp was: " + temp);
        //    int swapNum = Random.Range(0, AnswerBoxes.Length);
        //    //CBUG.Do("Swapping Correct answer: " + Quizzes[currentSet].CorrectAnswer + " to: " + swapNum);
        //    AnswerBoxes[x].text = AnswerBoxes[swapNum].text;
        //    AnswerBoxes[swapNum].text = temp;
        //    Quizzes[currentSet].Answers_ID[x] = Quizzes[currentSet].Answers_ID[swapNum];
        //    Quizzes[currentSet].Answers_ID[swapNum] = tempID;
        //}
    }


}
