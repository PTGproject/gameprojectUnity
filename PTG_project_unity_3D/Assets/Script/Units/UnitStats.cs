using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UnitStats : NetworkBehaviour
{
    [SerializeField] [SyncVar] private float _hp;
    [SerializeField] [SyncVar] private float _dmg;
    [SerializeField] [SyncVar] private float _trainingSpeed;
    [SerializeField] public List<Unit> _unitInRange = new List<Unit>();
    [SerializeField] private Transform _unitTransform = null;
    [SerializeField] private CircleCollider2D _attackRange = null;

    [SerializeField] private double _radiusSqrt;
    public float HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }
    public float Dmg
    {
        get
        {
            return _dmg;
        }
        set
        {
            _dmg = value;
        }
    }
    public float TrainingSpeed
    {
        get
        {
            return _trainingSpeed;
        }
        set
        {
            _trainingSpeed = value;
        }
    }

    public void UnitInit(string unitName)
    {
        switch (unitName)
        {
            case "UnitPeasant(Clone)":
                Dmg = 1;
                HP = 10;
                TrainingSpeed = 2;
                _radiusSqrt = Math.Pow(_attackRange.radius * _attackRange.transform.localScale.x * _unitTransform.localScale.x, 2);
                break;
            case "UnitArcher(Clone)":
                Dmg = 2;
                HP = 5;
                TrainingSpeed = 4;
                _radiusSqrt = Math.Pow(_attackRange.radius * _attackRange.transform.localScale.x * _unitTransform.localScale.x, 2);
                break;
        }
    }

    public void Start()
    {
        UnitInit(gameObject.name);

    }

    private void AddUnitInRange()
    {

        var units = FindObjectsOfType(typeof(Unit));
        foreach (Unit unit in units)
        {
            if (unit.hasAuthority)
            {
                continue;
            }
            if (!(Math.Pow(unit.transform.position.x - _unitTransform.position.x, 2) +
                  Math.Pow(unit.transform.position.z - _unitTransform.position.z, 2) <
                  _radiusSqrt))
            {
                continue;
            }

            if (_unitInRange.Find(x => x.netId == unit.netId) == null)
            {
                _unitInRange.Add(unit);
            }


        }
    }

    void RemoveUnit()
    {
        var tempUnits = new List<Unit>();
        _unitInRange.CopyTo(tempUnits);
        foreach (var unit in tempUnits)
        {
            if (!(Math.Pow(unit.transform.position.x - _unitTransform.position.x, 2) +
                  Math.Pow(unit.transform.position.z - _unitTransform.position.z, 2) <
                  _radiusSqrt))
            {
                _unitInRange.Remove(unit);
            }
        }
    }



    #region Server
    [Command]
    public void CmdDestroyUnit()
    {
        DestroyUnit();
    }

    [Command]
    public void CmdAttack()
    {
        Attack();
    }
    #endregion

    #region Client
    public void Update()
    {
        if (hasAuthority)
        {
            AddUnitInRange();
        }
        if(isServer)
        {
            RpcAttack();
            RpcDestroyUnit();
        }

        if (isClient)
        {
            CmdAttack();
            CmdDestroyUnit();
        }

        RemoveUnit();
    }

    [ClientRpc]
    void RpcDestroyUnit()
    {
        DestroyUnit();
    }

    void DestroyUnit()
    {
        /*if (HP <= 0)
        {
            Destroy(gameObject);
        }*/
    }
    [ClientRpc]
    void RpcAttack()
    {
        Attack();
    }
    void Attack()
    {
        if (_unitInRange.Count == 0)
        {
            return;
        }

        var unitToDamage = _unitInRange.First();
        unitToDamage.GetComponent<UnitStats>().HP = -1;
    }
    #endregion
}
