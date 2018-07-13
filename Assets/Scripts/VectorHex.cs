using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorHex {
    public static readonly VectorHex[] AXIAL_DIRECTIONS =
        { new VectorHex(1, 0), new VectorHex(1, -1), new VectorHex(0, -1),
        new VectorHex(-1, 0), new VectorHex(-1, 1), new VectorHex(0, 1) };

    public int q { get; set; }
    public int r { get; set; }

    public VectorHex(int q, int r)
    {
        Set(q, r);
    }

    public VectorHex(VectorCube vc)
    {
        VectorHex vx = vc.ToAxial();
        Set(vx.q, vx.r);
    }

    public VectorCube ToCube()
    {
        
        return new VectorCube(this.q, -this.q - this.r, this.r);
    }

    public void Set(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        VectorHex other = (VectorHex)obj;

        return (q == other.q) && (r == other.r); 
    }
}
