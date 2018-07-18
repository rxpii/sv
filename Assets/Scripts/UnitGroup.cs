using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGroup {

    // holds all units in the group
    public List<ResUnit> units { get; private set; }

    // the pooled attributes of the group
    public int groupHarvest { get; private set; }
    public int groupDefense { get; private set; }
    public int groupOffense { get; private set; }

    public Color groupColor { get; private set; }
    public PlayerController owner { get; private set; }
    public bool empty { get { return units.Count == 0; } private set { } }

    // constructor
    public UnitGroup (PlayerController owner, List<ResUnit> newUnits) : this(owner)
    {
        AddUnits(newUnits);
    }

    // constructor
    public UnitGroup(PlayerController owner) : this() {
        groupColor = owner.color;
        this.owner = owner;
    }

    // constructor
    public UnitGroup()
    {
        this.units = new List<ResUnit>();
        groupHarvest = 0;
        groupDefense = 0;
        groupOffense = 0;
    }
    
    // add new unit to the group
    public void AddUnit(ResUnit newUnit)
    {
        units.Add(newUnit);
        AddUnitAtt(newUnit);
        newUnit.SubscribeToGroup(this);
    }

    public void RemoveUnit(ResUnit removedUnit) {
        units.Remove(removedUnit);
        RemoveUnitAtt(removedUnit);
        removedUnit.UnsubscribeFromGroup();
    }

    public void AddUnits(List<ResUnit> newUnits)
    {
        foreach(ResUnit newUnit in newUnits)
        {
            AddUnit(newUnit);
        }
    }

    // recalculate the new pooled attributes
    public void CalculateGroupAtt()
    {
        groupHarvest = 0;
        groupDefense = 0;
        groupOffense = 0;

        foreach (ResUnit unit in units)
        {
            AddUnitAtt(unit);
        }
    }

    private void AddUnitAtt(ResUnit newUnit)
    {
        groupHarvest += newUnit.attHarvest;
        groupDefense += newUnit.attDefense;
        groupOffense += newUnit.attOffense;
    }

    private void RemoveUnitAtt(ResUnit removedUnit) {
        groupHarvest -= removedUnit.attHarvest;
        groupDefense -= removedUnit.attDefense;
        groupOffense -= removedUnit.attOffense;
    }
}
