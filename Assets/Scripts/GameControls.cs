using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages all the more specific classes
/// </summary>
public class GameControls : MonoBehaviour {

    public LidMovement Lid;
    public PartMan Parts;

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

    public bool QuestionAsked = false;

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
    }

    [System.Serializable]
    public struct NPCImgs
    {
        [SerializeField] public Sprite[] Img;
    }

    public NPCImgs[] CharSprites;

    public GameObject SpeechBubble;

    public Animator FrontChar;
    public Animator FadeInGame;

    public NPC[] Day1;
    public NPC[] Day2;
    public NPC[] Day3;
    private NPC[][] Days;
    
    private int[][] DaysOrder;

    public Image[] NPC_Line;

    private int currentNPC;
    private int currentDay;

    private bool InMinSpd;
    private bool InMaxSpd;
    private bool InMinVol;
    private bool InMaxVol;
    private float MinSpd;
    private float MaxSpd;
    private float MinVol;
    private float MaxVol;

    public int totalCharactersPerDay;

    // Use this for initialization
    void Start () {

        volHeights = new float[] {-1f, -0.03f, 0.7f, 1.43f, 2.16f, 2.89f, 3.62f, 5f};

        currentDay = 0;
        currentNPC = -1;

        DaysOrder = new int[][] {
            new int[5] { 0, 1, 2, 3, 4 },
            new int[5] { 4, 1, 3, 0, 2 },
            new int[5] { 2, 0, 4, 1, 3 }
        };

        Days = new NPC[3][] { Day1, Day2, Day3 };
        
        for (int x = 0; x < NPC_Line.Length; x++) {
            NPC_Line[x].sprite = CharSprites[DaysOrder[currentDay][x]].Img[2];
        }

        Lid.SetFillState(LidMovement.FillStates.Empty);
        Parts.ClearParts();
        //EvapoMount = (int)(/*evapoVolSpeed / Lid.MovIncrement) + 1*/;
    }
	
	// Update is called once per frame
	void Update () {

        if (Parts.IsBoiling && !Smoke.isPlaying) {
            Smoke.Play();
        }else if (!Parts.IsBoiling && Smoke.isPlaying) {
            Smoke.Stop();
        }
        if (Parts.IsBoiling) {
            if (Time.time - prevTime >= Parts.EvapoRate) {
                prevTime = Time.time;
                //Incremental parts deletion
                Parts.DeleteParts(EvapoMount);
                //Incremental volume lowering
                if (Lid.SmoothEmpty(EvapoVolSpeed)) {
                    Parts.IsBoiling = false;
                }
            }
        }
    }

    public void SpawnNPC()
    {
        currentNPC++;

        if(currentNPC > totalCharactersPerDay) {
            currentNPC = 0;
            currentDay++;
        }

        for (int x = 0; x < NPC_Line.Length; x++) {
            NPC_Line[x].enabled = false;
        }
        for(int x = 0 + currentNPC + 1; x < NPC_Line.Length; x++) {
            NPC_Line[x - currentNPC - 1].enabled = true;
            NPC_Line[x - currentNPC - 1].sprite = CharSprites[DaysOrder[currentDay][x]].Img[2];
        }
        FrontChar.SetTrigger("WalkIn");
        //DO IF IS QUIZ GUY CHECK
        //DO if is end of day check
    }

    public void SetRequest()
    {
        //Set question
        SpeechBubble.SetActive(true);
        SpeechBubble.GetComponentInChildren<Text>().text = Days[currentDay][currentNPC].Request;

        QuestionAsked = true;

        if (Days[currentDay][currentNPC].IsQuizGuy) {

        }
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

        if (!QuestionAsked)
            return;

        MaxSpd = Mathf.Pow(Days[currentDay][currentNPC].MaxTemp, 2);
        MinSpd = Mathf.Pow(Days[currentDay][currentNPC].MinTemp, 2);
        MinVol = volHeights[(int)Days[currentDay][currentNPC].MinVol];
        MaxVol = volHeights[(int)Days[currentDay][currentNPC].MaxVol];
        InMinSpd = Parts.AvgSpd >= MinSpd;
        InMaxSpd = Parts.AvgSpd <= MaxSpd;
        InMinVol = Lid.CurrHeight >= MinVol;
        InMaxVol = Lid.CurrHeight <= MaxVol;

        if ( InMinSpd && InMaxSpd && InMinVol && InMaxVol ) {
            SpeechBubble.GetComponentInChildren<Text>().text = Days[currentDay][currentNPC].Thanks;
            //animate leaving
            FrontChar.SetTrigger("WalkOut");
            //fade
            //FrontChar.set
            //spawn next
            //check somewhere to go to next day
            ClearCoffee();
        } else {
            SpeechBubble.GetComponentInChildren<Text>().text = Days[currentDay][currentNPC].Anger;
            ClearCoffee();
        }

        QuestionAsked = false;
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
    }

    public void IncreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Large)
            return;
        Parts.AddParts();
        Parts.IsBoiling = false;
        Parts.HeatUpRateMod += HeatUpVolMod;
        Lid.FillSome();
    }

    public void DecreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Empty)
            return;
        Parts.DeleteParts(0);
        Parts.HeatUpRateMod -= HeatUpVolMod;
        if(Lid.EmptySome()){
            Parts.IsBoiling = false;
        }
    }

    public void IncreaseHeat()
    {
        Parts.DecreaseRate();
    }

    public void DecreaseHeat()
    {
        Parts.IncreaseRate();
    }

    public Sprite GetCurrentCharSprite(float num)
    {
        return CharSprites[DaysOrder[currentDay][currentNPC]].Img[(int)num];
    }
}
