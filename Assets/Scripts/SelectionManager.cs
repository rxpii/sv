using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : Singleton<SelectionManager> {

    public float selectionBoxBorderThickness = 2f;
    public Color selectionBoxBorderColor;
    public Color selectionBoxColor;

    private new Camera camera;
    public List<TileBehavior> selectedTiles { get; private set; }
    private bool mouseDown;
    private Vector3 selectionPosInit;
    private Vector3 selectionPosCurrent;

    // Use this for initialization
    void Start () {
        camera = gameObject.GetComponent<Camera>();
        selectedTiles = new List<TileBehavior>();
        mouseDown = false;
    }
	
	// Update is called once per frame
	void Update () {
        SelectionFSM();
    }

    private void OnGUI() {
        if (mouseDown) {
            Rect selectionRect = Utils.GetScreenRect(selectionPosInit, selectionPosCurrent);
            Utils.DrawScreenRect(selectionRect, selectionBoxColor);
            Utils.DrawScreenRectBorder(selectionRect, selectionBoxBorderThickness, selectionBoxBorderColor);
        }

    }

    // finite state machine to handle unit selection
    private void SelectionFSM() {
        // MOUSE CLICK DOWN
        if (InputManager.PrimaryMouseButtonDown()) {
            selectionPosInit = Input.mousePosition;
        }

        // MOUSE HOLD DOWN
        if (InputManager.PrimaryMouseButton()) {
            if (!mouseDown) {
                mouseDown = true;
                if (!InputManager.SelectionAdd()) {
                    DeselectAll();
                }
            }
            selectionPosCurrent = Input.mousePosition;

            if (!selectionPosInit.Equals(selectionPosCurrent)) {
                bool initialClear = !InputManager.SelectionAdd();
                HandleSelectionMultiple(initialClear:initialClear);
            }
        }

        // MOUSE CLICK UP
        if (InputManager.PrimaryMouseButtonUp()) {
            mouseDown = false;

            // check for normal click without mouse dragging
            if (selectionPosInit.Equals(selectionPosCurrent)) {
                bool initialClear = !InputManager.SelectionAdd();
                //Debug.Log(initialClear);
                HandleSelectionSingle(initialClear:initialClear);
            }
        }

        // DESELECTION
        if (InputManager.Deselect()) {
            DeselectAll();
        }
    }

    private void HandleSelectionMultiple(bool initialClear=true) {
        if (initialClear) {
            DeselectAll();
        }

        Bounds currentBounds = Utils.GetViewportBounds(Camera.main, selectionPosInit, selectionPosCurrent);
        //List<GameObject> objectsInBounds = new List<GameObject>();

        foreach (GameObject tile in BoardManager.Instance.tiles) {
            if (currentBounds.Contains(Camera.main.WorldToViewportPoint(tile.transform.position))) {
                SelectSingle(tile.GetComponentInChildren<TileBehavior>());
            }
        }

        //return objectsInBounds;
    }

    private void HandleSelectionSingle(bool initialClear=true) {
        Debug.Log(initialClear);
        if (initialClear) {
            DeselectAll();
        }

        GameObject selectedObj = GetObjectUnderMouse();
        if (selectedObj == null)
            return;
        
        switch (selectedObj.tag) {
            case "board_tile":
                TileBehavior tile = selectedObj.GetComponentInChildren<TileBehavior>();
                if (tile.selected) {
                    DeselectSingle(tile);
                }
                else {
                    SelectSingle(tile);
                }
                break;
            case "res_unit":
                // DEBUGGING
                List<UnitGroup> neighbors = UnitManager.Instance.grid.GridSearchGroupNeighborGroups(selectedObj.GetComponentInParent<ResUnit>().posHex);
                List<VectorHex> posNeighbors = new List<VectorHex>();
                foreach (UnitGroup group in neighbors) {
                    foreach (ResUnit unit in group.units) {
                        posNeighbors.Add(unit.posHex);
                    }
                }
                UIManager.Instance.UpdateMarkers(posNeighbors);
                break;
            default:
                break;

        }
    }
    
    private void SelectSingle(TileBehavior tile) {
        if (!selectedTiles.Contains(tile)) {
            selectedTiles.Add(tile);
        }

        tile.OnSelect();
    }

    private void DeselectSingle(TileBehavior tile) {

        if (selectedTiles.Contains(tile)) {
            selectedTiles.Remove(tile);
        }

        tile.OnDeselect();
    }

    private void DeselectAll() {
        for (int i = selectedTiles.Count - 1; i >= 0; i--) {
            DeselectSingle(selectedTiles[i]);
        }
    }

    private GameObject GetObjectUnderMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            return hit.collider.gameObject;
        }

        return null;
    }
}
