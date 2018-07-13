using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour {

    public Material matIdle;
    public Material matHighlight;
    public GameObject pf_ResUnit;

    public VectorHex posHex;

    private Renderer rend;
    private GameObject unit;
    

    public bool empty { get; private set; }

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.material = matIdle;
        empty = true;
	}

    public void Initialize(VectorHex posHex)
    {
        this.posHex = posHex;
    }

    private void OnMouseEnter()
    {
        rend.material = matHighlight;
    }

    private void OnMouseExit()
    {
        rend.material = matIdle;
    }

    public ResUnit SpawnUnit(Vector3 attributes)
    {
        GameObject newUnit = Instantiate(pf_ResUnit, transform.parent.transform);
        newUnit.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        ResUnit ru = newUnit.GetComponent<ResUnit>();
        ru.Initialize(attributes, posHex);

        if (unit != null)
        {
            Destroy(unit);
            unit = newUnit;
        }

        empty = false;

        return ru;
    }
}
