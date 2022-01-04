using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private List<GameObject> unitPrefabs = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private Canvas unitCanvas = null;


    [SerializeField]
    [SyncVar]
    private List<int> queue = new List<int>();
    [SerializeField]
    [SyncVar]
    int unitType = 0;

    #region Server

    [Command]
    public void CmdSpawnUnit()
    {
        GameObject unitSpawn = Instantiate(unitPrefabs[unitType], unitSpawnPoint.position, unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitSpawn, connectionToClient);
    }
    [Command]
    public void CmdUnitType(int unitType)
    {
        this.unitType = unitType;
    }
    public void AddToQueue(int unit)
    {
        queue.Add(unit);
    }
    public void RemoveFromQueue(int unit)
    {
        queue.Remove(unit);

    }
    public bool IsQueued()
    {
        return queue.Count > 0;
    }

    public int GetFirst()
    {
        return queue.First();
    }

    public int GetCount()
    {
        return queue.Count;
    }

    public int GetUnit(int place)
    {
        return queue[place];
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }
        unitCanvas.gameObject.SetActive(!unitCanvas.gameObject.activeSelf);
    }


    #endregion
}
