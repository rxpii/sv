using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorCube {

    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

    public VectorCube(int x, int y, int z)
    {
        
        Set(x, y, z);
    }

    public VectorCube(VectorHex vx)
    {
        VectorCube vc = vx.ToCube();
        
        Set(vc.x, vc.y, vc.z);
    }

    public VectorHex ToAxial()
    {
        return new VectorHex(this.x, this.z);
    }

    public void Set(int x, int y, int z)
    {

        if ((x + y + z) != 0) {
            x = 0; y = 0; z = 0;
        }

        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        VectorCube other = (VectorCube)obj;

        return (x == other.x) && (y == other.y) && (z == other.z);
    }
}
