using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VertDragController : MonoBehaviour, IDragHandler
{

    public Camera cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DFRAG");

        //Borrowed from Unity DragMe UI Sample
        var tf = GetComponent<Transform>();
        tf.position = new Vector3(tf.position.x,
                                  cam.ScreenToWorldPoint(eventData.position).y,
                                  0f);
    }
}
