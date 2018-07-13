using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager> {
    private readonly int GRID_DIM = 5;

    private readonly int MODE_NONE = 0;
    private readonly int MODE_CREATE = 1;
    private readonly int MODE_CHANGE = 2;
    private readonly int ATT_HARV = 0;
    private readonly int ATT_DEF = 1;
    private readonly int ATT_OFF = 2;
    private readonly int MAX_ATT = 6;

    private int currentMode = 0;
    private TileBehavior selectedTile;
    private List<int> selectedAtt;
    private int currentAtt = 0;
    private Camera camera;

    public List<ResUnit> allUnits { get; private set; }
    public List<UnitGroup> allGroups { get; private set; }
    private UnitGrid grid;

	// Use this for initialization
	void Start () {
        currentMode = 0;
        selectedAtt = new List<int>() { 0, 0, 0 };
        allUnits = new List<ResUnit>();
        allGroups = new List<UnitGroup>();
        camera = Camera.main;
        grid = new UnitGrid(GRID_DIM);
	}
	
	// Update is called once per frame
	void Update () {
        
        // determine the current mode
        if (InputManager.CreateUnit()) {
            Debug.Log("END: " + InputManager.CreateUnit() + " " + currentMode);
            if (currentMode == MODE_NONE)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject other = hit.collider.gameObject;
                    if (other.tag == Tags.BoardTile)
                    {
                        TileBehavior tb = other.GetComponent<TileBehavior>();
                        if (tb.empty)
                        {
                            selectedTile = tb;
                            EnterCreateMode();
                        }
                    }
                }
            }
            // handle attribute selection end
            else if (currentMode == MODE_CREATE)
            {
                Debug.Log(selectedAtt[ATT_HARV] + selectedAtt[ATT_DEF] + selectedAtt[ATT_OFF]);
                if (selectedAtt[ATT_HARV] + selectedAtt[ATT_DEF] + selectedAtt[ATT_OFF] >= MAX_ATT)
                {
                    
                    CullAlloc();
                    ResUnit newUnit = selectedTile.SpawnUnit(new Vector3(selectedAtt[ATT_HARV], selectedAtt[ATT_DEF], selectedAtt[ATT_OFF]));
                    IntegrateUnit(newUnit);
                    currentMode = MODE_NONE;
                }
            }
        }
        else if (InputManager.ChangeUnit())
        {
            if (currentMode == MODE_NONE)
                EnterChangeMode();
        }

        // handle attribute selection
        if (currentMode == MODE_CREATE)
        {
            int attAlloc = 0;
            bool inputReceived = false;
            if (InputManager.Allocate_1())
            {
                attAlloc = 1;
                inputReceived = true;
            }
            if (InputManager.Allocate_2())
            {
                attAlloc = 2;
                inputReceived = true;
            }
            if (InputManager.Allocate_3())
            {
                attAlloc = 3;
                inputReceived = true;
            }
            if (InputManager.Allocate_4())
            {
                attAlloc = 4;
                inputReceived = true;
            }
            if (InputManager.Allocate_5())
            {
                attAlloc = 5;
                inputReceived = true;
            }
            if (InputManager.Allocate_6())
            {
                attAlloc = 6;
                inputReceived = true;
            }
            
            if (inputReceived)
            {
                selectedAtt.Insert(0, attAlloc);
                CullAlloc();
            }
        }
	}

    private void EnterCreateMode()
    {
        Debug.Log("ENTERED CREATE");
        currentMode = MODE_CREATE;
        selectedAtt = new List<int>() { 0, 0, 0 };
        currentAtt = 0;
    }

    private void EnterChangeMode()
    {

    }

    // truncates attribute allocation so selectedAtt's magnitude is 6
    private void CullAlloc()
    {
        selectedAtt[0] = Mathf.Clamp(selectedAtt[ATT_HARV], 0, MAX_ATT);
        selectedAtt[1] = Mathf.Clamp(selectedAtt[ATT_DEF], 0, MAX_ATT - selectedAtt[ATT_HARV]);
        selectedAtt[2] = Mathf.Clamp(selectedAtt[ATT_OFF], 0, MAX_ATT - selectedAtt[ATT_HARV] - selectedAtt[ATT_DEF]);
    }

    private void IntegrateUnit(ResUnit unit)
    {
        Debug.Log("INTEGRATING");
        VectorHex posHex = unit.posHex;
        List<ResUnit> neighbors = grid.NeighborsOf(posHex);
        List<UnitGroup> neighborGroups = new List<UnitGroup>();

        foreach (ResUnit neighbor in neighbors)
        {
            if (!neighborGroups.Contains(neighbor.group))
                neighborGroups.Add(neighbor.group);
        }

        // if there are no neighbors
        if (neighborGroups.Count < 1)
        {
            UnitGroup newGroup = new UnitGroup();
            newGroup.AddUnit(unit);
            allGroups.Add(newGroup);
        }
        else if (neighborGroups.Count > 1)
        {
            UnitGroup superGroup = neighborGroups[0];
            for (int i = 1; i < neighborGroups.Count; i++)
            {
                superGroup.AddUnits(neighborGroups[i].units);
                if (allGroups.Contains(neighborGroups[i]))
                {
                    allGroups.Remove(neighborGroups[i]);
                }
            }
            superGroup.AddUnit(unit);
        }
        else
        {
            neighborGroups[0].AddUnit(unit);
        }

        grid.PlaceUnit(unit);
        allUnits.Add(unit);
        UIManager.Instance.UpdateModeUI(true);
    }
}
