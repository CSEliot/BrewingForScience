using UnityEngine;
using System.Collections;

public class LidMovement : MonoBehaviour {

    public float YMax = -1.15f;
    public float YMin = -7.15f;
    public float MovIncrement;

    private float currY;

    public PartMan Parts;
    public GameControls GameCont;

    public enum FillStates
    {
        Empty,
        Small,
        SmallMedium, 
        Medium,
        LargeMedium,
        Large
    }
    public FillStates CurrFill; 
    private int totalStates = 6;

    private float currHeight;

    // Use this for initialization
    void Start () {
        MovIncrement = Mathf.Abs((YMax - YMin) / totalStates);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //INCREMENTAL FILL
    public void FillSome()
    {
        if (transform.localPosition.y + 1f > YMax)
            return;
        CurrFill++;
        currHeight = (YMin + ((float)CurrFill * MovIncrement));
        
        //if amt is defined as 0 or less, default movIncrement is used.
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    currHeight,
                    transform.localPosition.z
        );

        //transform.Translate(new Vector3(
        //    0f,
        //    movIncrement,
        //    0f));
    }

    /// <summary>
    /// Returns True if cup is completely empty.
    /// </summary>
    public bool EmptySome()
    {
        if (transform.localPosition.y <= YMin)
            return true;
        CurrFill--;
        currHeight = (YMin + ((float)CurrFill * MovIncrement));
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    currHeight,
                    transform.localPosition.z
        );
        if (transform.localPosition.y <= YMin) {
            Parts.ClearParts();
            return true;
        }

        return false;
        //transform.Translate(new Vector3(
        //    0f,
        //    -movIncrement,
        //    0f));
    }

    public void EmptyAll()
    {
        SetFillState(FillStates.Empty);
    }

    public int TotalStates {
        get {
            return totalStates;
        }
    }

    public float CurrHeight {
        get {
            return currHeight;
        }
    }

    //ASSIGNABLE FILL
    public void SetFillState(FillStates NewFill)
    {
        CurrFill = NewFill;
        currHeight = (YMin + ((float)CurrFill * MovIncrement));
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    currHeight,
                    transform.localPosition.z
        );

        //switch (CurrFill) {
        //    case FillStates.Empty:
        //        transform.localPosition = new Vector3(
        //            transform.localPosition.x,
        //            YMin,
        //            transform.localPosition.z
        //        );
        //        break;
        //    case FillStates.Empty:
        //        transform.localPosition = new Vector3(
        //            transform.localPosition.x,
        //            YMin,
        //            transform.localPosition.z
        //        );
        //        break;
        //    default:
        //        CBUG.Error("BAD NEWFILL GIVEN SOMEHOW");
        //        break;
        //}
    }

    /// <summary>
    /// Returns true of completely empty.
    /// </summary>
    /// <param name="amt"></param>
    /// <returns></returns>
    public bool SmoothEmpty(float amt)
    {
        if (transform.localPosition.y - amt <=  YMin) {
            Parts.ClearParts();
            return true;
        }
        if (transform.localPosition.y - amt < YMin + ((float)CurrFill * MovIncrement)) {
            CurrFill--;
            Parts.HeatUpRateMod -= GameCont.HeatUpVolMod;
        }
        
        transform.Translate(new Vector3(
            0f,
            -amt,
            0f));
        if (transform.localPosition.y - amt <= YMin) {
            Parts.ClearParts();
            return true;
        }
        return false;
    }

}
