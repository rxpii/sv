using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResUnit : MonoBehaviour {

    public static readonly int ATT_HARV = 0;
    public static readonly int ATT_DEF = 1;
    public static readonly int ATT_OFF = 2;
    public static readonly int MAX_ATT = 6;

    // unit attributes
    public int attHarvest { get; private set; }
    public int attDefense { get; private set; }
    public int attOffense { get; private set; }

    public VectorHex posHex;
    public TileBehavior tile;
    public UnitGroup group;
    public PlayerController owner;

    private Renderer rend;

	// Use this for initialization
	void Start () {
        
	}

    public void Initialize(Vector3 attributes, TileBehavior tile, PlayerController owner)
    {
        rend = GetComponentInChildren<Renderer>();
        SetAttributes(attributes);
        this.tile = tile;
        this.posHex = tile.posHex;
        this.owner = owner;
        
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void SetAttributes(Vector3 attributes)
    {
        attHarvest = (int) attributes.x;
        attDefense = (int) attributes.y;
        attOffense = (int) attributes.z;
        UIManager.Instance.UpdateModeUI();
    }

    public void SubscribeToGroup(UnitGroup group)
    {
        this.group = group;
        rend.material.color = group.groupColor;
    }

    public void UnsubscribeFromGroup() {
        if (group != null) {
            this.group = null;
        }
    }
}
