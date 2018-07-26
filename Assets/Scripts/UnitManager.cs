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

    public ResUnit CreateUnitSingle(Vector3 attributes, TileBehavior spawnTile, PlayerController owner) {
        // remove old tile
        if (!spawnTile.empty) {
            /*if (allUnits.Contains(spawnTile.unit)) {
                RemoveUnit(spawnTile.unit);
            }*/
            if (!spawnTile.unit.owner.Equals(owner)) {
                return null;
            }
            spawnTile.unit.group.ChangeUnitAtt(spawnTile.unit, attributes);
            spawnTile.unit.SetAttributes(attributes);
            return spawnTile.unit;
        }
        ResUnit newUnit = spawnTile.SpawnUnit(attributes, owner);
        IntegrateUnit(newUnit);
        return newUnit;
    }

    public void CreateUnitMultiple(Vector3 attributes, List<TileBehavior> spawnTiles, PlayerController owner) {
        foreach (TileBehavior spawnTile in spawnTiles) {
            ResUnit newUnit = CreateUnitSingle(attributes, spawnTile, owner);
            
        }
        foreach (PlayerController player in PlayerInteractionManager.Instance.players) {
            if (!player.Equals(owner)) {
                TerritoryUpdateGroup(player);
            }
        }

        TerritoryUpdateGroup(owner);

        UIManager.Instance.UpdateModeUI(bypassPreviousCheck:true);
    }

    // places new unit within nearby group if one exists, aggregating multiple should it bridge different groups.
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
    }

    // destroys the players units that do not survive the enemy groups
    // does not affect the units not controlled by the given player
    private void TerritoryUpdateGroup(PlayerController player) {
        // loop through all groups
        for (int i = allGroups.Count - 1; i >= 0; i--) {
            UnitGroup group = allGroups[i];

            // check if they are owned by the player
            if (group.owner.Equals(player)) {

                // reevaluate for new dead units if a unit in the group was previously removed
                bool unitRemoved = false;
                do {
                    unitRemoved = false;
                    for (int j = group.units.Count - 1; j >= 0; j--) {
                        ResUnit unit = group.units[j];
                        bool unitSurvived = EndureUnit(unit);
                        if (!unitSurvived) {
                            RemoveUnit(unit);
                            unitRemoved = true;
                        }
                    }
                } while (unitRemoved && !group.empty);

                // remove the group if all units within it are dead
                if (group.empty) {
                    allGroups.Remove(group);
                }
            }
        }
    }

    // endures the unit through onslaught of enemies. Returns true upon survival, otherwise false.
    private bool EndureUnit(ResUnit enduree) {
        // select all groups that are not under control of the enduree's owner
        List<UnitGroup> neighborGroups = grid.GetPosNeighborGroups(enduree.posHex, distinguishPlayers:true, playerQueried:enduree.owner.playerID, ignoreOwnGroup:true, invertPlayerSelection:true);
        int sumOff = 0;
        foreach (UnitGroup neighborGroup in neighborGroups) {
            
            sumOff += neighborGroup.groupOffense;
        }

        return sumOff <= enduree.group.groupDefense;
    }

    private void RemoveUnit(ResUnit removedUnit) {
        allUnits.Remove(removedUnit);
        grid.RemoveUnit(removedUnit);
        UnitGroup removedUnitGroup = removedUnit.group;
        removedUnit.group.RemoveUnit(removedUnit);
        removedUnit.tile.RemoveUnit();
        Destroy(removedUnit.gameObject);
    }

    private void ChangeGroup(ResUnit changingUnit, UnitGroup newGroup) {
        changingUnit.group.RemoveUnit(changingUnit);
        newGroup.AddUnit(changingUnit);
    }
}
