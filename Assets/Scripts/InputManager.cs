using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages player inputs. Handles the unit attribute allocation system
public static class InputManager {

    public static bool PrimaryMouseButtonDown() {
        return Input.GetMouseButtonDown(0);
    }

    public static bool PrimaryMouseButtonUp() {
        return Input.GetMouseButtonUp(0);
    }

    public static bool PrimaryMouseButton() {
        return Input.GetMouseButton(0);
    }

    public static bool Deselect() {
        return Input.GetButtonDown("deselect");
    }

    public static bool SelectionAdd() {
        return Input.GetButton("selection_add");
    }

    public static bool CreateUnit()
    {
        return Input.GetButtonDown("create_unit");
    }

    public static bool ChangeUnit()
    {
        return Input.GetButtonDown("change_unit");
    }

    public static bool Allocate_0() {
        return Input.GetButtonDown("allocate_0");
    }

    public static bool Allocate_1()
    {
        return Input.GetButtonDown("allocate_1");
    }

    public static bool Allocate_2()
    {
        return Input.GetButtonDown("allocate_2");
    }

    public static bool Allocate_3()
    {
        return Input.GetButtonDown("allocate_3");
    }

    public static bool Allocate_4()
    {
        return Input.GetButtonDown("allocate_4");
    }

    public static bool Allocate_5()
    {
        return Input.GetButtonDown("allocate_5");
    }

    public static bool Allocate_6()
    {
        return Input.GetButtonDown("allocate_6");
    }

    public static bool InfoHarvest()
    {
        return Input.GetButton("info_harvest");
    }

    public static bool InfoOffense()
    {
        return Input.GetButton("info_offense");
    }

    public static bool InfoDefense()
    {
        return Input.GetButton("info_defense");
    }

    public static bool SwitchPlayer() {
        return Input.GetButtonDown("switch_player");
    }
}
