using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Canvas mainUI = null;
    [SerializeField] private float _gold;
    [SerializeField] private Text _goldText= null;

    public float Gold
    {
        get => _gold;
        set => _gold = value;
    }
    [SerializeField] private int _numberOfUnits;
    [SerializeField] private Text _numberOfUnitsText = null;

    public int NumberOfUnits
    {
        get => _numberOfUnits;
        set => _numberOfUnits = value;
    }
    [SerializeField] private float _passiveGoldIncome;

    public float PassiveGoldIncome
    {
        get => _passiveGoldIncome;
        set => _passiveGoldIncome = value;
    }


    #region Server



    #endregion

    #region Client

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Gold = 1000;
        NumberOfUnits = 0;
        PassiveGoldIncome = 2.5f;
        mainUI.gameObject.SetActive(true);
    }

    void Update()
    {
        _goldText.text = $"Gold: {Math.Round(Gold,0)}";

        NumberOfUnits = 0;
        var units = FindObjectsOfType<Unit>();
        foreach (var unit in units)
        {
            if (unit.hasAuthority)
                NumberOfUnits++;
        }
        _numberOfUnitsText.text = $"Units: {NumberOfUnits}";

        Gold += PassiveGoldIncome * Time.deltaTime;
    }

    #endregion
}
