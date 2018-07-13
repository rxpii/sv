using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour {

    // width ratio: sqrt(3)
    // multiply by size to obtain hexagon's width or height
    private readonly float WIDTH_RATIO = 1.732f;
    private readonly float HEIGHT_RATIO = 2f;

    public int boardSize;
    public int tileSize;
    public GameObject boardTile;


	// Use this for initialization
	void Start () {
		for (int i = 0; i < boardSize; i++)
        {
            int coordSum = 2 * i;
            for (int q = -i; q < i + 1; q++)
            {
                int rLowBound = -i - q;
                int rHighBound = i;

                if (q > 0)
                {
                    rLowBound = -i;
                    rHighBound = i - q;
                }

                for (int r = rLowBound; r < rHighBound + 1; r++)
                {
                    VectorHex posHex = new VectorHex(q, r);
                    Vector2 cartCoord = HexToCart(posHex);
                    GameObject newTile = Instantiate(boardTile, transform);
                    newTile.transform.position = new Vector3(cartCoord.x, 0f, cartCoord.y);
                    TileBehavior tb = newTile.GetComponentInChildren<TileBehavior>();
                    tb.Initialize(posHex);

                }
            }

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // given vc coordinates, respective coordinates in 2d plane will be calculated for the pointy orientation
    private Vector2 CubeToCart(VectorCube vc)
    {
        
        return new Vector2(WIDTH_RATIO * tileSize * (vc.x - vc.y) / 2.0f, HEIGHT_RATIO * tileSize * -vc.z * 3f / 4f);

    }

    private Vector2 HexToCart(VectorHex vx)
    {
        return CubeToCart(new VectorCube(vx));
    }
}
