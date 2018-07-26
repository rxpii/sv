using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGrid{

    public static int GRID_DIM = 5;

    private ResUnit[,] grid;

    public UnitGrid()
    {
        // create 2d grid that can hold hex value positions of units
        grid = new ResUnit[2 * GRID_DIM - 1, 2 * GRID_DIM - 1];
    }

    public static VectorHex OffsetForGrid(VectorHex posHex)
    {
        return new VectorHex(posHex.q + GRID_DIM - 1, posHex.r + GRID_DIM - 1);
    }

    public void PlaceUnit (ResUnit unit)
    {
        VectorHex offsetPos = OffsetForGrid(unit.posHex);
        grid[offsetPos.q, offsetPos.r] = unit;
    }

    public void RemoveUnit(ResUnit unit) {
        RemoveAt(unit.posHex);
    }

    public ResUnit GetAt(VectorHex posHex)
    {
        VectorHex offsetPos = OffsetForGrid(posHex);
        return grid[offsetPos.q, offsetPos.r];
    } 

    public ResUnit RemoveAt(VectorHex posHex)
    {
        VectorHex offsetPos = OffsetForGrid(posHex);
        ResUnit removed = grid[offsetPos.q, offsetPos.r];
        grid[offsetPos.q, offsetPos.r] = null;
        return removed;
    }

    public bool InGrid(VectorHex posHex)
    {
        return Mathf.Abs(posHex.q) < GRID_DIM && Mathf.Abs(posHex.r) < GRID_DIM;
    }
    
    public List<ResUnit> GetNeighbors(VectorHex posHex)
    {
        return GetUnitsAt(GetPosNeighbors(posHex));
    }

    public List<VectorHex> GetPosNeighbors(VectorHex posHex)
    {
        List<VectorHex> neighbors = new List<VectorHex>();

        foreach (VectorHex direction in VectorHex.AXIAL_DIRECTIONS)
        {
            VectorHex neighborPos = new VectorHex(posHex.q + direction.q, posHex.r + direction.r);
            if (InGrid(neighborPos) && (GetAt(neighborPos) != null))
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;
    }

    // posHex - the queried position
    // distinguishPlayers - retreive groups of the specified player
    // playerQueried - the id of the specified player
    // ignoreOwnGroup - ignore the group the position's unit is in
    // invertPlayerSelection - retreive groups not of the specified player
    public List<UnitGroup> GetPosNeighborGroups(VectorHex posHex, bool distinguishPlayers=false, int playerQueried=0, bool ignoreOwnGroup=true, bool invertPlayerSelection=false) {
        List<ResUnit> neighbors = GetNeighbors(posHex);
        List<UnitGroup> neighborGroups = new List<UnitGroup>();
        ResUnit unitAtPos = GetAt(posHex);

        foreach (ResUnit neighbor in neighbors) {
            // reference comparison
            if (!neighborGroups.Contains(neighbor.group)) {
                if (ignoreOwnGroup 
                    && unitAtPos != null
                    && neighbor.group == unitAtPos.group) {
                    continue;
                }

                if (distinguishPlayers) {
                    bool unitIsPlayers = invertPlayerSelection ^ (neighbor.owner.playerID == playerQueried);
                    if (!unitIsPlayers)
                        continue;
                }
                neighborGroups.Add(neighbor.group);
            }
                
        }

        return neighborGroups;
    }
    
    // specify whether group units must be of same player with distinguishPlayers
    public List<ResUnit> GridSearchGroup(VectorHex posStart, bool distinguishPlayers=false)
    {
        List<VectorHex> visited = new List<VectorHex>();
        List<VectorHex> frontier = new List<VectorHex>();

        if (GetAt(posStart) == null)
            return new List<ResUnit>();

        frontier.Add(posStart);
        int playerAtPos = GetAt(posStart).owner.playerID;

        while (frontier.Count > 0)
        {
            VectorHex posCurrent = frontier[0];
            frontier.RemoveAt(0);
            List<VectorHex> posNeighbors = GetPosNeighbors(posCurrent);
            foreach (VectorHex posNeighbor in posNeighbors)
            {
                if (!visited.Contains(posNeighbor) 
                    && !frontier.Contains(posNeighbor)
                    && GetAt(posNeighbor).owner.playerID == playerAtPos)
                    frontier.Add(posNeighbor);
            }

            visited.Add(posCurrent);
        }

        return GetUnitsAt(visited);
    }

    // returns list of enemy units that border given pos' group
    public List<ResUnit> GridSearchGroupNeighborUnits(VectorHex posStart) {
        List<VectorHex> visited = new List<VectorHex>();
        List<VectorHex> frontier = new List<VectorHex>();
        List<VectorHex> neighbors = new List<VectorHex>();

        if (GetAt(posStart) == null)
            return new List<ResUnit>();

        frontier.Add(posStart);
        int playerAtPos = GetAt(posStart).owner.playerID;

        while (frontier.Count > 0) {
            VectorHex posCurrent = frontier[0];
            frontier.RemoveAt(0);
            List<VectorHex> posNeighbors = GetPosNeighbors(posCurrent);
            foreach (VectorHex posNeighbor in posNeighbors) {
                if (!visited.Contains(posNeighbor) && !frontier.Contains(posNeighbor)) {
                    if (GetAt(posNeighbor).owner.playerID == playerAtPos)
                        frontier.Add(posNeighbor);
                    else
                        neighbors.Add(posNeighbor);
                }
            }

            visited.Add(posCurrent);
        }

        return GetUnitsAt(neighbors);
    }

    // posStart must contain a unit
    public List<UnitGroup> GridSearchGroupNeighborGroups(VectorHex posStart) {
        List<VectorHex> visited = new List<VectorHex>();
        List<VectorHex> frontier = new List<VectorHex>();
        List<UnitGroup> neighbors = new List<UnitGroup>();

        if (GetAt(posStart) == null)
            return neighbors;

        frontier.Add(posStart);
        UnitGroup groupStart = GetAt(posStart).group;
        int playerAtPos = GetAt(posStart).owner.playerID;

        while (frontier.Count > 0) {
            VectorHex posCurrent = frontier[0];
            frontier.RemoveAt(0);
            List<VectorHex> posNeighbors = GetPosNeighbors(posCurrent);
            foreach (VectorHex posNeighbor in posNeighbors) {
                if (!visited.Contains(posNeighbor) && !frontier.Contains(posNeighbor)) {
                    UnitGroup groupNeighbor = GetAt(posNeighbor).group;
                    // if the id is different, then the unit must be part of another enemy group
                    if (GetAt(posNeighbor).owner.playerID == playerAtPos)
                        frontier.Add(posNeighbor);
                    else if (!neighbors.Contains(GetAt(posNeighbor).group))
                        neighbors.Add(groupNeighbor);
                }
            }

            visited.Add(posCurrent);
        }

        return neighbors;
    }

    private List<ResUnit> GetUnitsAt(List<VectorHex> positions)
    {
        List<ResUnit> units = new List<ResUnit>();

        foreach (VectorHex pos in positions)
        {
            units.Add(GetAt(pos));
        }
        return units;
    }
}
