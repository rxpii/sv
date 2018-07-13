using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    private enum Mode : int { AllAtt, InfoHarv, InfoDef, InfoOff }

    public int labelPoolSize = 100;
    public GameObject pf_Label;

    private int previousMode = 0;
    private int currentMode = 0;
    private List<GameObject> labelPool;

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

    private void DisableLabels()
    {
        foreach (GameObject label in labelPool)
            label.SetActive(false);
    }
}
