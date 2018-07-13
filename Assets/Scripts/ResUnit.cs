using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResUnit : MonoBehaviour {

    private static readonly int MAX_ATT = 6;

    // unit attributes
    public int attHarvest { get; private set; }
    public int attDefense { get; private set; }
    public int attOffense { get; private set; }

    public VectorHex posHex;
    public UnitGroup group;
    
    private Renderer rend;

	// Use this for initialization
	void Start () {
        
	}
	
    public void Initialize(Vector3 attributes, VectorHex posHex)
    {
        rend = GetComponentInChildren<Renderer>();
        SetAttributes(attributes);
        this.posHex = posHex;
        
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
}
