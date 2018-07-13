using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

    private new Camera camera;

    // Use this for initialization
    void Start () {
        camera = gameObject.GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject other = hit.collider.gameObject;
                Debug.Log(other.tag);
                switch(other.tag)
                {
                    case "board_tile":
                        TileBehavior tb = other.GetComponent<TileBehavior>();
                        tb.SpawnUnit(Vector3.zero);
                        break;
                    default:
                        break;

                }
                /*
                if (hit.transform.CompareTag(targetsTag))
                    cam.SetTarget(hit.transform);
                else
                    cam.ResetTarget();
                    */
            }
        }
    }
}
