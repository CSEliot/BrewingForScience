using UnityEngine;
using System.Collections;

public class ActivateOnCount : MonoBehaviour
{

    public int CountTrigger;
    private int count;

    public GameControls _GameControls;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(CountTrigger == count)
        {
            count = 0;
            _GameControls.AskQuestion();
        }
    }

    public void CountUp()
    {
        count++;
    }
}
