using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager> {

    // width ratio: sqrt(3)
    // multiply by size to obtain hexagon's width or height
    private readonly float WIDTH_RATIO = 1.732f;
    private readonly float HEIGHT_RATIO = 2f;

    public int boardSize;
    public float tileSize;
    public GameObject boardTile;
    public GameObject board;

    public List<GameObject> tiles { get; private set; }


	// Use this for initialization
	void Start () {
        tiles = new List<GameObject>();

        int boardWidth = boardSize - 1;

        for (int q = -boardWidth; q < boardSize; q++) {
            int rLowBound = -boardWidth - q;
            int rHighBound = boardWidth;

            if (q > 0) {
                rLowBound = -boardWidth;
                rHighBound = boardWidth - q;
            }

            for (int r = rLowBound; r < rHighBound + 1; r++) {
                VectorHex posHex = new VectorHex(q, r);
                Vector2 cartCoord = HexToCart(posHex);
                GameObject newTile = Instantiate(boardTile, board.transform);
                newTile.transform.position = new Vector3(cartCoord.x, 0f, cartCoord.y);
                TileBehavior tb = newTile.GetComponentInChildren<TileBehavior>();
                tb.Initialize(posHex);
                tiles.Add(newTile);

            }
        }
/*
        for (int i = 0; i < boardSize; i++)
        {
           
            

        }
        */
        UIManager.Instance.Initialize();
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
