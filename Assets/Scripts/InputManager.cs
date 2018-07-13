using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages player inputs. Handles the unit attribute allocation system
public static class InputManager {

	public static bool CreateUnit()
    {
        return Input.GetButtonDown("create_unit");
    }

    public static bool ChangeUnit()
    {
        return Input.GetButtonDown("change_unit");
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
}
