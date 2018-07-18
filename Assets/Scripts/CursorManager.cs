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
                    case "res_unit":
                        //List<UnitGroup> neighbors = UnitManager.Instance.grid.GetPosNeighborGroups(other.GetComponentInParent<ResUnit>().posHex, distinguishPlayers: true, playerQueried: other.GetComponentInParent<ResUnit>().owner.playerID, ignoreOwnGroup: true, invertPlayerSelection: true);
                        List<UnitGroup> neighbors = UnitManager.Instance.grid.GridSearchGroupNeighborGroups(other.GetComponentInParent<ResUnit>().posHex);
                        List<VectorHex> posNeighbors = new List<VectorHex>();
                        foreach (UnitGroup group in neighbors) {
                            foreach (ResUnit unit in group.units) {
                                posNeighbors.Add(unit.posHex);
                            }
                        }
                        UIManager.Instance.UpdateMarkers(posNeighbors);
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
