using UnityEngine;
using System.Collections;

public class LidMovement : MonoBehaviour {

    public float YMax = -1.15f;
    public float YMin = -7.15f;
    public float MovIncrement;

    private float currY;

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
        //if amt is defined as 0 or less, default movIncrement is used.
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    YMin + ((float)CurrFill * MovIncrement),
                    transform.localPosition.z
        );
        //transform.Translate(new Vector3(
        //    0f,
        //    movIncrement,
        //    0f));
    }

    public void EmptySome()
    {
        if (transform.localPosition.y - 1f < YMin)
            return;
        CurrFill--;
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    YMin + ((float)CurrFill * MovIncrement),
                    transform.localPosition.z
        );
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

    //ASSIGNABLE FILL
    public void SetFillState(FillStates NewFill)
    {
        CurrFill = NewFill;
        transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    YMin + ((float)CurrFill),
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

    public void SmoothEmpty(float amt)
    {
        if (transform.localPosition.y - amt < YMin)
            return;
        if (transform.localPosition.y - amt < YMin + ((float)CurrFill * MovIncrement)) {
            CurrFill--;
            CBUG.Do("CUYRR MINUS IN SMOOTH");
        }
        
        transform.Translate(new Vector3(
            0f,
            -amt,
            0f));
    }

}
