using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    private enum Mode : int { AllAtt, InfoHarv, InfoDef, InfoOff }

    public int labelPoolSize = 100;
    public GameObject pf_Label;
    public GameObject pf_Marker;

    private int previousMode = 0;
    private int currentMode = 0;
    private List<GameObject> labelPool;
    private GameObject[,] markerPool;

	// Use this for initialization
	void Start () {
        labelPool = new List<GameObject>();

		for (int i = 0; i < labelPoolSize; i++)
        {
            GameObject label = Instantiate(pf_Label, transform);
            label.SetActive(false);
            labelPool.Add(label);
        }

        
    }

    // Remove these initialization functions later. Reduce the coupling in these start function. Possibly handle them in a dedicated initialization script?
    public void Initialize() {
        markerPool = new GameObject[UnitGrid.GRID_DIM * 2 - 1, UnitGrid.GRID_DIM * 2 - 1];

        for (int i = 0; i < BoardManager.Instance.tiles.Count; i++) {

            GameObject tile = BoardManager.Instance.tiles[i];
            Debug.Log(tile);
            VectorHex tilePos = tile.GetComponentInChildren<TileBehavior>().posHex;
            VectorHex offsetPos = UnitGrid.OffsetForGrid(tilePos);
            GameObject marker = Instantiate(pf_Marker, transform);
            markerPool[offsetPos.q, offsetPos.r] = marker;
            marker.SetActive(true);
            marker.transform.position = new Vector3(tile.transform.position.x, 1.5f, tile.transform.position.z);
            marker.GetComponentInChildren<Image>().color = Color.grey;
            marker.GetComponentInChildren<Text>().text = "" + tilePos.q + " " + tilePos.r;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (InputManager.InfoHarvest())
        {
            currentMode = (int) Mode.InfoHarv;
        }
        else if (InputManager.InfoDefense())
        {
            currentMode = (int) Mode.InfoDef;
        }
        else if (InputManager.InfoOffense())
        {
            currentMode = (int) Mode.InfoOff;
        }
        else
        {
            currentMode = (int) Mode.AllAtt;
        }

        UpdateModeUI();
	}

    public void UpdateModeUI(bool bypassPreviousCheck=false)
    {
        if (currentMode == previousMode && !bypassPreviousCheck)
            return;

        previousMode = currentMode;

        DisableLabels();

        switch(currentMode)
        {
            
            case (int) Mode.AllAtt:
                for (int i = 0; i < UnitManager.Instance.allUnits.Count; i++)
                {
                    ResUnit unit = UnitManager.Instance.allUnits[i];
                    labelPool[i].transform.position = new Vector3(unit.transform.position.x, 1.5f, unit.transform.position.z);
                    labelPool[i].GetComponentInChildren<Image>().color = Color.gray;
                    labelPool[i].GetComponentInChildren<Text>().text = unit.attHarvest + " | " + unit.attDefense + " | " + unit.attOffense;
                    labelPool[i].SetActive(true);
                }
                break;
            case (int) Mode.InfoHarv:
                for (int i = 0; i < UnitManager.Instance.allGroups.Count; i++)
                {
                    UnitGroup group = UnitManager.Instance.allGroups[i];
                    Vector3 center = Vector3.zero;
                    foreach (ResUnit unit in group.units)
                        center += unit.gameObject.transform.position;
                    center /= group.units.Count;

                    center.y = 1.5f;
                    labelPool[i].transform.position = center;
                    labelPool[i].GetComponentInChildren<Image>().color = Color.green;
                    labelPool[i].GetComponentInChildren<Text>().text = group.groupHarvest.ToString();
                    labelPool[i].SetActive(true);
                }
                break;
            case (int) Mode.InfoDef:
                for (int i = 0; i < UnitManager.Instance.allGroups.Count; i++)
                {
                    UnitGroup group = UnitManager.Instance.allGroups[i];
                    Vector3 center = Vector3.zero;
                    foreach (ResUnit unit in group.units)
                        center += unit.gameObject.transform.position;
                    center /= group.units.Count;

                    center.y = 1.5f;
                    labelPool[i].transform.position = center;
                    labelPool[i].GetComponentInChildren<Image>().color = Color.blue;
                    labelPool[i].GetComponentInChildren<Text>().text = group.groupDefense.ToString();
                    labelPool[i].SetActive(true);
                }
                break;
            case (int) Mode.InfoOff:
                for (int i = 0; i < UnitManager.Instance.allGroups.Count; i++)
                {
                    UnitGroup group = UnitManager.Instance.allGroups[i];
                    Vector3 center = Vector3.zero;
                    foreach (ResUnit unit in group.units)
                        center += unit.gameObject.transform.position;
                    center /= group.units.Count;

                    center.y = 1.5f;
                    labelPool[i].transform.position = center;
                    labelPool[i].GetComponentInChildren<Image>().color = Color.red;
                    labelPool[i].GetComponentInChildren<Text>().text = group.groupOffense.ToString();
                    labelPool[i].SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public void UpdateMarkers(List<VectorHex> posHighlighted) {
        foreach (GameObject tile in BoardManager.Instance.tiles) {
            TileBehavior tb = tile.GetComponentInChildren<TileBehavior>();
            VectorHex offsetPos = UnitGrid.OffsetForGrid(tb.posHex);
            GameObject marker = markerPool[offsetPos.q, offsetPos.r];
            if (posHighlighted.Contains(tb.posHex))
                marker.GetComponentInChildren<Image>().color = Color.cyan;
            else
                marker.GetComponentInChildren<Image>().color = Color.grey;

        }
    }

    private void DisableLabels()
    {
        foreach (GameObject label in labelPool)
            label.SetActive(false);
    }

}
