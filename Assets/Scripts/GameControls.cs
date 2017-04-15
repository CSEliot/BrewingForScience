using UnityEngine;
using UnityEngine.UI;
using LoLSDK;
using System.Collections;

/// <summary>
/// Manages all the more specific classes
/// </summary>
public class GameControls : MonoBehaviour {

    public LidMovement Lid;
    public PartMan Parts;
    public TempReader _TempReader;

    public GameObject ServeBtn;
    public GameObject TempControls;
    public GameObject HeatElementHighlight;
    public GameObject[] DayPanels;
    public GameObject EndGamePanel;
    public GameObject HeatControls;
    public GameObject CoolControls;
    public GameObject TempReaderObj;

    #region Coffee Visual Art
    public GameObject CoffeeBG;
    public GameObject CoffeeTop;
    public GameObject CoffeeBottom;
    #endregion


    private int currentTemp = 0;
    private int MaxTemp = 5;

    private float prevTime;

    public ParticleSystem Smoke;
    
    [Tooltip("Amount that's deleted every evaporation frame")]
    public int EvapoMount;
    /// <summary>
    /// Speed at which the lid top decreases.
    /// </summary>
    public float EvapoVolSpeed;
    /// <summary>
    /// More liquid in the cup means it takes longer to heat up.
    /// </summary>
    public float HeatUpVolMod;

    private float[] volHeights;

    /// <summary>
    /// Disables Serve button until question is answered.
    /// </summary>
    public bool QuestionAsked = false;

    public QuizMan Quiz;

    [System.Serializable]
    public struct NPC
    {
        public string Request;
        public string Thanks;
        public string Anger;
        public float MinTemp;
        public float MaxTemp;
        public float MinVol;
        public float MaxVol;
        public bool IsQuizGuy;
        public float MinTherm;
        public float MaxTherm;
    }

    [System.Serializable]
    public struct NPCImgs
    {
        [SerializeField] public Sprite[] Img;
    }

    [System.Serializable]
    public struct QnA
    {
        string Question;
        [SerializeField]
        string[] Answers;
        int CorrectAnswer;
    }

    public NPCImgs[] CharSprites;

    public GameObject SpeechBubble;

    public Animator FrontChar;
    public Animator FadeInGame;

    public NPC[] Day1;
    public NPC[] Day2;
    public NPC[] Day3;
    private NPC[][] days;
    
    private int[][] DaysOrder;

    public Image[] NPC_Line;

    public int CurrentNPC;
    public int CurrentDay;

    private bool InMinSpd;
    private bool InMaxSpd;
    private bool InMinVol;
    private bool InMaxVol;
    private float MinSpd;
    private float MaxSpd;
    private float MinVol;
    private float MaxVol;

    public int TotalCharactersPerDay;

    public bool IsPaused;

    public int MaxProgress;
    public int CurrentProgress;
    public int CurrentScore;

    public bool CantTestServeStatus;

    // Use this for initialization
    void Start () {
        IsPaused = false;

        LOLSDK.Init("com.kiteliongames.brewcoffee");
        LOLSDK.Instance.SubmitProgress(0, 0, MaxProgress);

        volHeights = new float[] {-1f, -0.03f, 0.7f, 1.43f, 2.16f, 2.89f, 3.62f, 5f};

        CurrentDay = 0;
        CurrentNPC = -1;
        CurrentProgress = 0;
        CurrentScore = 0;

        DaysOrder = new int[][] {
            new int[5] { 0, 1, 2, 3, 4 },
            new int[5] { 0, 1, 2, 3, 4 },
            new int[5] { 0, 1, 2, 3, 4 }
        };

        days = new NPC[3][] { Day1, Day2, Day3 };
        
        for (int x = 0; x < NPC_Line.Length; x++) {
            NPC_Line[x].sprite = CharSprites[DaysOrder[CurrentDay][x]].Img[2];
        }

        Lid.SetFillState(LidMovement.FillStates.Empty);
        Parts.ClearParts();
        //EvapoMount = (int)(/*evapoVolSpeed / Lid.MovIncrement) + 1*/;
    }
	
	// Update is called once per frame
	void Update () {

        if (IsPaused)
            return;

        if (Parts.IsBoiling && !Smoke.isPlaying) {
            Smoke.Play();
        }else if (!Parts.IsBoiling && Smoke.isPlaying) {
            Smoke.Stop();
        }
        if (Parts.IsBoiling) {
            if (Time.time - prevTime >= Parts.EvaporateWaitTime) {
                prevTime = Time.time;
                //Incremental parts deletion
                Parts.DeleteParts(EvapoMount);
                //Incremental volume lowering
                if (Lid.SmoothEmpty(EvapoVolSpeed)) {
                    Parts.IsBoiling = false;
                    Parts.CanBoil = false;
                }
            }
        }

        canServeStatus();
    }

    public void SpawnNPC()
    {
        CurrentNPC++;
        

        if(CurrentNPC >= TotalCharactersPerDay) {
            CurrentNPC = 0;
            CurrentDay++;

            if(CurrentDay == 1)
            {
                TempReaderObj.SetActive(true);
            }

            if(CurrentDay == 2)
            {
                Tips.Spawn(3);
            }

            if(CurrentDay == 3) {
                
                EndGamePanel.SetActive(true);
                return;
            }
            TempControls.SetActive(true);
            HeatElementHighlight.SetActive(true);
        }

        for (int x = 0; x < NPC_Line.Length; x++) {
            NPC_Line[x].enabled = false;
        }
        for(int x = 0 + CurrentNPC + 1; x < NPC_Line.Length; x++) {
            NPC_Line[x - CurrentNPC - 1].enabled = true;
            NPC_Line[x - CurrentNPC - 1].sprite = CharSprites[DaysOrder[CurrentDay][x]].Img[2];
        }
        FrontChar.SetTrigger("WalkIn");
        if(CurrentDay == 2 && CurrentNPC == 1)
        {
            Tips.Spawn(5);
        }
    }

    public void SetRequest()
    {
        //Set question
        SpeechBubble.SetActive(true);
        SpeechBubble.GetComponentInChildren<Text>().text = days[CurrentDay][CurrentNPC].Request;

        QuestionAsked = true;

        if (days[CurrentDay][CurrentNPC].IsQuizGuy) {
            Smoke.Pause();
            Parts.PartSys.Pause();
            IsPaused = true;
            Parts.IsPaused = true;

            //CALL PARTS INIT
            Quiz.Init(true);
        }
    }

    public void AskQuestion()
    {
        Smoke.Pause();
        Parts.PartSys.Pause();
        IsPaused = true;
        Parts.IsPaused = true;

        //CALL PARTS INIT
        Quiz.Init(true);
    }

    public void EndRequest()
    {
        SpeechBubble.SetActive(false);
        SpeechBubble.GetComponentInChildren<Text>().text = "";
        QuestionAsked = false;
    }

    public void FadeInGameAnim()
    {
        FadeInGame.SetTrigger("FadeIn");
    }

    public void Serve()
    {
        ServeBtn.SetActive(false);
        if (!QuestionAsked || days[CurrentDay][CurrentNPC].IsQuizGuy)
        {
            ServeBtn.SetActive(true);
            return;
        }

        Parts.UpdateAvgSpd();

        MaxSpd = days[CurrentDay][CurrentNPC].MaxTemp;
        MinSpd = days[CurrentDay][CurrentNPC].MinTemp;
        MinVol = volHeights[(int)days[CurrentDay][CurrentNPC].MinVol];
        MaxVol = volHeights[(int)days[CurrentDay][CurrentNPC].MaxVol];
        InMinSpd = Parts.SqrAvgSpd >= MinSpd;
        InMaxSpd = Parts.SqrAvgSpd <= MaxSpd;
        InMinVol = Lid.CurrHeight >= (MinVol - 0.5f); 
        InMaxVol = Lid.CurrHeight <= (MaxVol + 0.5f);
        //Added 0.5f for possible strange rounding errors in WebGL port.

        if ( InMinSpd && InMaxSpd && InMinVol && InMaxVol ) {
            SpeechBubble.GetComponentInChildren<Text>().text = days[CurrentDay][CurrentNPC].Thanks;
            //animate leaving
            FrontChar.SetTrigger("WalkOut");
            //fade
            //FrontChar.set
            //spawn next
            //check somewhere to go to next day
            ClearCoffee();
            CoffeeBG.SetActive(false);
            CoffeeBottom.SetActive(false);
            CoffeeTop.SetActive(false);
            QuestionAsked = false;
            CurrentProgress++;
            UpdateCurrentProgress();
        } else {
            if(SpeechBubble.GetComponentInChildren<Text>().text == days[CurrentDay][CurrentNPC].Anger)
            {
                SpeechBubble.GetComponentInChildren<Text>().text = days[CurrentDay][CurrentNPC].Request;
            }
            else
            {
                SpeechBubble.GetComponentInChildren<Text>().text = days[CurrentDay][CurrentNPC].Anger;  
            }
            //ClearCoffee();
        }
        StartCoroutine(ReactiveServeButton());
    }

    private void canServeStatus()
    {
        if (CantTestServeStatus)
            return;
        MaxSpd = days[CurrentDay][CurrentNPC].MaxTemp;
        MinSpd = days[CurrentDay][CurrentNPC].MinTemp;
        bool prevGoodStatus = (InMinSpd && InMaxSpd); 
        InMinSpd = Parts.SqrAvgSpd >= MinSpd;
        InMaxSpd = Parts.SqrAvgSpd <= MaxSpd;
        if (prevGoodStatus != (InMinSpd && InMaxSpd))
        {
            //_TempReader.SetReadingTemp(Parts.SqrAvgSpd);
            CBUG.Log("Servable Status changed!");
        }
    }

    /// <summary>
    /// Iterates through state towards target fill state.
    /// Possible edge case includes that the state changes require a frame between them.
    /// </summary>
    /// <param name="TargetFillState"></param>
    public void SetCoffeeState(LidMovement.FillStates TargetFillState)
    {
        if (TargetFillState == Lid.CurrFill)
            return;
        if(TargetFillState < Lid.CurrFill) { 
            while (Lid.CurrFill != TargetFillState) {
                DecreaseCoffee();
            }
        }else {
            while (Lid.CurrFill != TargetFillState) {
                IncreaseCoffee();
            }
        }   
    }

    public void ClearCoffee()
    {
        Parts.ClearParts();
        Lid.EmptyAll();
        Parts.IsBoiling = false;
        Parts.CanBoil = false;
        CoffeeBG.SetActive(false);
        CoffeeBottom.SetActive(false);
        CoffeeTop.SetActive(false);
        _TempReader.ResetTempReader();
    }

    public void IncreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Large)
        {
            if(CurrentDay == 2)
            {
                Tips.Spawn(2);
            }
            return;
        }
        Parts.AddParts();
        Parts.IsBoiling = false;
        Parts.CanBoil = false;
        Parts.VolumeChangeWaitTime += HeatUpVolMod;
        Lid.FillSome();
        CoffeeBG.SetActive(true);
        CoffeeBottom.SetActive(true);
        CoffeeTop.SetActive(true);
    }

    public void DecreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Empty)
            return;
        //Parts.DeleteParts(0);
        Parts.VolumeChangeWaitTime -= HeatUpVolMod;
        if(Lid.EmptySome()){
            Parts.IsBoiling = false;
            CoffeeBG.SetActive(false);
            CoffeeBottom.SetActive(false);
            CoffeeTop.SetActive(false);
            _TempReader.ResetTempReader();
        }
    }

    public void IncreaseHeat()
    {
        Tips.Spawn(6);
        if (currentTemp >= MaxTemp) {
            HeatControls.SetActive(false);
            Tips.Spawn(1);
        } else {
            CoolControls.SetActive(true);
            currentTemp++;
            Parts.HeatUpEnabled = true;
        }
        Parts.SpeedupHeatup();
    }

    public void DecreaseHeat()
    {
        if(currentTemp <= 0) {
            CoolControls.SetActive(false);
            Tips.Spawn(4);
            Parts.HeatUpEnabled = false;
        } else {
            HeatControls.SetActive(true);
            currentTemp--;
        }
        Parts.SlowdownHeatup();
    }

    public Sprite GetCurrentCharSprite(float num)
    {
        return CharSprites[DaysOrder[CurrentDay][CurrentNPC]].Img[(int)num];
    }

    public void GameComplete()
    {
        LOLSDK.Instance.CompleteGame();
    }

    public void UpdateCurrentProgress()
    {
        LOLSDK.Instance.SubmitProgress(CurrentScore, CurrentProgress, MaxProgress);
    }

    public void SetAnswer(bool isCorrect)
    {

    }

    public NPC[][] Days {
        get {
            return days;
        }
    }

    public static GameControls GetSelf()
    {
        return GameObject.FindGameObjectWithTag("GameControls").GetComponent<GameControls>();
    }

    public static bool IsQuizTime()
    {
        return GameObject.FindGameObjectWithTag("GameControls").GetComponent<GameControls>().IsQuizTimeBool();
    }

    public bool IsQuizTimeBool()
    {
        return days[CurrentDay][CurrentNPC].IsQuizGuy;
    }
    
    private IEnumerator ReactiveServeButton()
    {
        yield return new WaitForSeconds(1f);
        ServeBtn.SetActive(true);
    }
}
