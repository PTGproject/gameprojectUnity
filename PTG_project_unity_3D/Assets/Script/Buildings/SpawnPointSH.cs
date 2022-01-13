using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnPointSH : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private Material _red;
    [SerializeField] private Material _green;
    [SerializeField] private MeshRenderer _material = null;
    [SerializeField] private GameObject _barrackGameObject = null;

    public bool IsOccupied
    {
        get => _isOccupied;
        set
        {
            if (_isOccupied == value) return;
            _isOccupied = value;
            _material.material = _isOccupied ? _red : _green;
        }
    }



    #region Server
    [Command]
    public void CmdBarrackSpawn()
    {

        GameObject unitSpawn = Instantiate(_barrackGameObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
        RpcDisableBuildingArea();
        NetworkServer.Spawn(unitSpawn, connectionToClient);
        
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
        if (IsOccupied)
        {
            return;
        }
        var player = NetworkClient.localPlayer.gameObject.GetComponent<PlayerManager>();
        var cost = 250f;
        if (player.Gold - cost < 0)
        {
            Debug.Log("Biedak");
            return;
        }
        CmdBarrackSpawn();
        player.Gold -= cost;
        
        IsOccupied = true;
       
    }
    [ClientRpc]
    void RpcDisableBuildingArea()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
