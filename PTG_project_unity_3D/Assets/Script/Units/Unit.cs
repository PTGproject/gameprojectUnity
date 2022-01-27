using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    [Header("Scripts")]
    [SerializeField] private UnitMovement unitMovement = null;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    #region Server

    #endregion

    #region Client

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    #endregion
}
