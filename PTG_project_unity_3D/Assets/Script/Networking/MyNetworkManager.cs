using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }

  
  
}
