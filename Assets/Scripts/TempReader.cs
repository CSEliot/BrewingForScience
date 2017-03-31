using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempReader : MonoBehaviour
{

    public int CurrentType;
    //1 = Fahrenheit
    //2 = Celcius
    //3 = Kelvin

    public Text TempRead;
    public PartMan Parts;

    private float minSpd = 1.5f;
    private float maxSpd;
    private float spdRange;

    private float celciusMin = 23f; //Room temp
    private float celciusMax = 100f; //Boiling point of water
    private float celciusRange;

    private float fahrenheitMin;
    private float fahrenheitMax;
    private float fahrenheitRange;

    private float kelvinMin;
    private float kelvinMax;
    private float kelvinRange;

    private float partSpd;

    /*
     * new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
     * Or a little more readable:
     * OldRange = (OldMax - OldMin)
     * NewRange = (NewMax - NewMin)
     * NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin
     */

    // Use this for initialization
    void Start()
    {
        maxSpd = Parts.SqrBoilPoint;
        spdRange = maxSpd - minSpd;
        celciusRange = celciusMax - celciusMin;
        fahrenheitMin = celciusMin * 1.8f + 32f;
        fahrenheitMax = celciusMax * 1.8f + 32f;
        fahrenheitRange = fahrenheitMax - fahrenheitMin;
        kelvinMin = celciusMin + 273.15f;
        kelvinMax = celciusMax + 273.15f;
        kelvinRange = kelvinMax - kelvinMin;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeReadingType()
    {
        CurrentType++;
        CurrentType = CurrentType > 3 ? 1 : CurrentType;
        SetReadingTemp(partSpd);
    }

    public void SetReadingTemp(float partSpd)
    {
        this.partSpd = partSpd;
        if(CurrentType == 1)
        {
            setFahrenheit(partSpd);
            return;
        }
        if(CurrentType == 2)
        {
            setCelcius(partSpd);
            return;
        }
        if(CurrentType == 3)
        {
            setKelvin(partSpd);
            return;
        }
        else
        {
            CBUG.Error("Bad CurrentType: " + CurrentType);
        }
    }

    public void ResetTempReader ()
    {
        setText("Scanning for heat ...");
    }

    private void setCelcius(float partSpd)
    {
        string temp = "Celcius: \n";
        int round = Mathf.RoundToInt(((partSpd - minSpd) / (maxSpd - minSpd)) * (celciusMax - celciusMin) + celciusMin);
        temp += round;
        setText( temp + "°");
    }

    private void setFahrenheit(float partSpd)
    {
        string temp = "Fahrenheit: \n";
        int round = Mathf.RoundToInt(((partSpd - minSpd) / (maxSpd - minSpd)) * (fahrenheitMax - fahrenheitMin) + fahrenheitMin);
        temp += round;
        setText(temp + "°");
    }

    private void setKelvin(float partSpd)
    {
        string temp = "Kelvin: \n";
        int round = Mathf.RoundToInt(((partSpd - minSpd) / (maxSpd - minSpd)) * (kelvinMax - kelvinMin) + kelvinMin);
        temp += round;
        setText(temp + "°");
    }

    private void setText(string text)
    {
        TempRead.text = text;
    }
}
