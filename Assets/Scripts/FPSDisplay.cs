using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    private float fps;
    public Text targetText;

    public bool DisplayOnOfficial;

    void Update()
    {
        if (DisplayOnOfficial || Debug.isDebugBuild) {
            //Method obtained from: http://wiki.unity3d.com/index.php?title=FramesPerSecond&oldid=18981
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            //float msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            targetText.text = ("" + fps) + "  ";
            //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }
    }
}