using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour {

    public Material matIdle;
    public Material matHover;
    public Material matSelected;
    public GameObject pf_ResUnit;

    public VectorHex posHex;

    private Renderer rend;
    public ResUnit unit { get; private set; }

    public bool selected { get; private set; }

    public bool empty { get; private set; }

	// Use this for initialization
	void Start () {
        rend = GetComponentInChildren<Renderer>();
        rend.material = matIdle;
        empty = true;
        selected = false;
	}

    public void Initialize(VectorHex posHex)
    {
        this.posHex = posHex;
    }

    private void OnMouseEnter()
    {
        if (!selected)
            rend.material = matHover;
    }

    private void OnMouseExit()
    {
        if (!selected)
            rend.material = matIdle;
    }

    public ResUnit SpawnUnit(Vector3 attributes, PlayerController owner)
    {
        if (unit != null) {
            unit.SetAttributes(attributes);
        }
        else {
            GameObject newUnit = Instantiate(pf_ResUnit, transform.parent.transform);
            newUnit.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            ResUnit ru = newUnit.GetComponent<ResUnit>();
            ru.Initialize(attributes, this, owner);
            unit = ru;
            empty = false;
        }

        return unit;
    }

    public void RemoveUnit() {
        unit = null;
        empty = true;
    }

    public void OnSelect() {
        selected = true;
        rend.material = matSelected;
    }

    public void OnDeselect() {
        selected = false;
        rend.material = matIdle;
    }

    // tiles are equal if their positions are equal
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        TileBehavior other = (TileBehavior)obj;

        return posHex.Equals(other.posHex);
    }
}
