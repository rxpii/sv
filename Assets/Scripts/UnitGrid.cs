using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGrid{

    private ResUnit[,] grid;
    private int gridDim;

    public UnitGrid(int gridDim)
    {
        // create 2d grid that can hold hex value positions of units
        grid = new ResUnit[2 * gridDim - 1, 2 * gridDim - 1];
        this.gridDim = gridDim;
    }

    public VectorHex OffsetForGrid(VectorHex posHex)
    {
        return new VectorHex(posHex.q + gridDim - 1, posHex.r + gridDim - 1);
    }

    public void PlaceUnit (ResUnit unit)
    {
        VectorHex offsetPos = OffsetForGrid(unit.posHex);
        grid[offsetPos.q, offsetPos.r] = unit;
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
        return Mathf.Abs(posHex.q) < gridDim && Mathf.Abs(posHex.r) < gridDim;
    }
    
    public List<ResUnit> NeighborsOf(VectorHex posHex)
    {
        return GetUnitsAt(NeighborsPosOf(posHex));
    }

    public List<VectorHex> NeighborsPosOf(VectorHex posHex)
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

    public List<ResUnit> GridSearchGroup(VectorHex startPos)
    {
        List<VectorHex> visited = new List<VectorHex>();
        List<VectorHex> frontier = new List<VectorHex>();

        if (GetAt(startPos) == null)
            return new List<ResUnit>();

        frontier.Add(startPos);

        while (frontier.Count > 0)
        {
            VectorHex currentPos = frontier[0];
            frontier.RemoveAt(0);
            List<VectorHex> neighbors = NeighborsPosOf(currentPos);
            foreach (VectorHex neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && !frontier.Contains(neighbor))
                    frontier.Add(neighbor);
            }

            visited.Add(currentPos);
        }

        return GetUnitsAt(visited);
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
