using UnityEngine;
using System.Collections;

public class LidMovement : MonoBehaviour {

    public float YMax = -1.15f;
    public float YMin = -7.15f;
    private float movIncrement;

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
        movIncrement = Mathf.Abs((YMax - YMin) / totalStates);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //INCREMENTAL FILL
    public void FillSome()
    {
        if (transform.localPosition.y + 1f > YMax)
            return;
        transform.Translate(new Vector3(
            0f,
            movIncrement,
            0f));
        CurrFill++;
    }

    public void EmptySome()
    {
        if (transform.localPosition.y - 1f < YMin)
            return;
        transform.Translate(new Vector3(
            0f,
            -movIncrement,
            0f));
        CurrFill--;
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

}
