using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController {

    private static int playerCounter = 0;

    public Color color { get; private set; }
    public int playerID { get; private set; }


    public PlayerController() {
        playerID = playerCounter;
        playerCounter++;
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void PerformCreate() {

    }

    private void PerformChange() {

    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType())
            return false;

        PlayerController other = (PlayerController)obj;

        return playerID == other.playerID;
    }
}
