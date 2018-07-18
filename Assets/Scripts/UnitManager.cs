using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager> {
    
    private int currentMode = 0;
    private TileBehavior selectedTile;
    private List<int> selectedAtt;
    private int currentAtt = 0;
    private Camera camera;

    public List<ResUnit> allUnits { get; private set; }
    public List<UnitGroup> allGroups { get; private set; }
    public UnitGrid grid { get; private set; }

	// Use this for initialization
	void Start () {
        currentMode = 0;
        selectedAtt = new List<int>() { 0, 0, 0 };
        allUnits = new List<ResUnit>();
        allGroups = new List<UnitGroup>();
        grid = new UnitGrid();
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    public void CreateUnit(Vector3 attributes, TileBehavior spawnTile, PlayerController owner) {
        ResUnit newUnit = spawnTile.SpawnUnit(attributes, owner);
        IntegrateUnit(newUnit);
        
    }

    private void IntegrateUnit(ResUnit unit)
    {
        Debug.Log("INTEGRATING");
        VectorHex posHex = unit.posHex;
        Debug.Log("ID: " + unit.owner.playerID);
        List<UnitGroup> neighborGroups = grid.GetPosNeighborGroups(posHex, distinguishPlayers:true, playerQueried:unit.owner.playerID, ignoreOwnGroup:true, invertPlayerSelection:false);

        // if there are no neighbors
        if (neighborGroups.Count < 1)
        {
            UnitGroup newGroup = new UnitGroup(unit.owner);
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
        TerritoryUpdateGroup(unit);

        UIManager.Instance.UpdateModeUI(true);
    }

    // destroys enemy units within range of group specified by unit
    // the algorithm first loops through all affected units and removes ones that do not survive. Then it 
    // repeats the process with the survived units and continues to do so until no units are removed or none are left
    private void TerritoryUpdateGroup(ResUnit groupUnit) {
        List<UnitGroup> enemyGroups = grid.GridSearchGroupNeighborGroups(groupUnit.posHex);
        
        for (int i = enemyGroups.Count - 1; i >= 0; i--) {
            bool unitRemoved = false;
            do {
                for (int j = enemyGroups[i].units.Count - 1; j >= 0; j--) {
                    ResUnit enemy = enemyGroups[i].units[j];
                    bool enemySurvived = EndureUnit(enemy);
                    if (!enemySurvived) {
                        RemoveUnit(enemy);
                    }
                }
            } while (unitRemoved && enemyGroups[i].units.Count > 0);
        }
        
    }

    // endures the unit through onslaught of enemies. Returns true upon survival, otherwise false.
    private bool EndureUnit(ResUnit enduree) {
        // select all groups that are not under control of the enduree's owner
        List<UnitGroup> neighborGroups = grid.GetPosNeighborGroups(enduree.posHex, distinguishPlayers:true, playerQueried:enduree.owner.playerID, ignoreOwnGroup:true, invertPlayerSelection:true);
        int sumOff = 0;
        foreach (UnitGroup neighborGroup in neighborGroups)
            sumOff += neighborGroup.groupOffense;

        return sumOff <= enduree.group.groupDefense;
    }

    private void RemoveUnit(ResUnit removedUnit) {
        allUnits.Remove(removedUnit);
        UnitGroup removedUnitGroup = removedUnit.group;
        removedUnit.tile.RemoveUnit();
        if (removedUnitGroup.empty) {
            allGroups.Remove(removedUnitGroup);
        }
    }
}
