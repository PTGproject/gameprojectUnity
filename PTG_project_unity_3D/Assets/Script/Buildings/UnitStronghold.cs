using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitStronghold : NetworkBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints = null;
    [SerializeField] private GameObject _spawnPoinGameObject = null;

    #region Server
    [Command]
    public void CmdBuildingLocation()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            GameObject unitSpawn = Instantiate(_spawnPoinGameObject, spawnPoint.position, spawnPoint.rotation);

            NetworkServer.Spawn(unitSpawn, connectionToClient);
        }
    }


    #endregion

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
       CmdBuildingLocation();
    }
    #endregion


}
