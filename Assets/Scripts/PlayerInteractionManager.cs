using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : Singleton<PlayerInteractionManager> {

    private readonly int MODE_NONE = 0;
    private readonly int MODE_CREATE = 1;
    private readonly int MODE_CHANGE = 2;

    private Camera camera;
    private TileBehavior selectedTile;
    private List<int> selectedAtt;
    private int currentMode = 0;
    private int currentAtt = 0;
    private List<PlayerController> players;
    public PlayerController currentPlayer { get; set; }

    // Use this for initialization
    void Start () {
        camera = Camera.main;
        players = new List<PlayerController>();
        currentMode = 0;

        // local player 0
        players.Add(new PlayerController());
        // local player 1
        players.Add(new PlayerController());

        currentPlayer = players[0];
    }
	
	// Update is called once per frame
	void Update () {

        // determine the current mode
        if (InputManager.CreateUnit()) {
            Debug.Log("END: " + InputManager.CreateUnit() + " " + currentMode);
            if (currentMode == MODE_NONE) {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    GameObject other = hit.collider.gameObject;
                    if (other.tag == Tags.BoardTile) {
                        TileBehavior tb = other.GetComponent<TileBehavior>();
                        if (tb.empty) {
                            selectedTile = tb;
                            EnterCreateMode();
                        }
                    }
                }
            }
            // handle attribute selection end
            else if (currentMode == MODE_CREATE) {
                if (selectedAtt[ResUnit.ATT_HARV] + selectedAtt[ResUnit.ATT_DEF] + selectedAtt[ResUnit.ATT_OFF] >= ResUnit.MAX_ATT) {

                    CullAlloc();
                    Vector3 attributes = new Vector3(selectedAtt[ResUnit.ATT_HARV], selectedAtt[ResUnit.ATT_DEF], selectedAtt[ResUnit.ATT_OFF]);
                    UnitManager.Instance.CreateUnit(attributes, selectedTile, currentPlayer);
                    currentMode = MODE_NONE;
                }
            }
        }
        else if (InputManager.ChangeUnit()) {
            if (currentMode == MODE_NONE)
                EnterChangeMode();
        }

        if (InputManager.SwitchPlayer()) {
            if (currentPlayer == players[0])
                ChangeCurrentPlayer(players[1]);
            else
                ChangeCurrentPlayer(players[0]);
            Debug.Log("CURRENT PLAYER: " + currentPlayer.playerID);
        }

        // handle attribute selection
        if (currentMode == MODE_CREATE) {
            int attAlloc = 0;
            bool inputReceived = false;
            if (InputManager.Allocate_0()) {
                attAlloc = 0;
                inputReceived = true;
            }
            if (InputManager.Allocate_1()) {
                attAlloc = 1;
                inputReceived = true;
            }
            if (InputManager.Allocate_2()) {
                attAlloc = 2;
                inputReceived = true;
            }
            if (InputManager.Allocate_3()) {
                attAlloc = 3;
                inputReceived = true;
            }
            if (InputManager.Allocate_4()) {
                attAlloc = 4;
                inputReceived = true;
            }
            if (InputManager.Allocate_5()) {
                attAlloc = 5;
                inputReceived = true;
            }
            if (InputManager.Allocate_6()) {
                attAlloc = 6;
                inputReceived = true;
            }

            if (inputReceived) {
                selectedAtt.Insert(0, attAlloc);
                CullAlloc();
            }
        }
    }

    private void EnterCreateMode() {
        Debug.Log("ENTERED CREATE");
        currentMode = MODE_CREATE;
        selectedAtt = new List<int>() { 0, 0, 0 };
        currentAtt = 0;
    }

    private void EnterChangeMode() {

    }

    // truncates attribute allocation so selectedAtt's magnitude is 6
    private void CullAlloc() {
        selectedAtt[0] = Mathf.Clamp(selectedAtt[ResUnit.ATT_HARV], 0, ResUnit.MAX_ATT);
        selectedAtt[1] = Mathf.Clamp(selectedAtt[ResUnit.ATT_DEF], 0, ResUnit.MAX_ATT - selectedAtt[ResUnit.ATT_HARV]);
        selectedAtt[2] = Mathf.Clamp(selectedAtt[ResUnit.ATT_OFF], 0, ResUnit.MAX_ATT - selectedAtt[ResUnit.ATT_HARV] - selectedAtt[ResUnit.ATT_DEF]);
    }

    // tentative. Change later to support multiple players
    public void ChangeCurrentPlayer(PlayerController newPlayer) {
        currentPlayer = newPlayer;
    }
}
