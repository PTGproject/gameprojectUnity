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
    [SerializeField] [SyncVar] private float _attackSpeed;
    [SerializeField] [SyncVar] private float _trainingCost;
    [SerializeField] private double _radiusSqrt;
    [SerializeField] [SyncVar] private List<Unit> _unitInRange = new List<Unit>();
    [SerializeField] private Transform _unitTransform = null;
    [SerializeField] private CircleCollider2D _attackRange = null;
    private Coroutine _trainingCoroutine;

    
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
    public float TrainingCost
    {
        get
        {
            return _trainingCost;
        }
        set
        {
            _trainingCost = value;
        }
    }
    public float AttakcSpeed
    {
        get
        {
            return _attackSpeed;
        }
        set
        {
            _attackSpeed = value;
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
                AttakcSpeed = 1;
                TrainingCost = 100;
                break;
            case "UnitArcher(Clone)":
                AttakcSpeed = 2;
                Dmg = 2;
                HP = 5;
                TrainingSpeed = 4;
                TrainingCost = 200;
                break;
        }
    }

    private void AddUnitInRange()
    {
        var units = FindObjectsOfType(typeof(Unit));
        if (hasAuthority)
        {
           
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
        else
        {
            foreach (Unit unit in units)
            {

                if (!unit.hasAuthority)
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

    }
    public void RemoveUnit()
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

    public void Attack()
    {
        if (_unitInRange.Count == 0)
        {
            return;
        }

        var unitToDamage = _unitInRange.First();
        unitToDamage.GetComponent<UnitStats>().TakeDmg(Dmg);
    }
    public void TakeDmg(float dmg)
    {
        HP -= dmg;
    }
    [Command]
    public void CmdDestroyUnit()
    {
        Destroy(gameObject);
        RpcDestroyUnit();
    }

    [ClientRpc]
    public void RpcDestroyUnit()
    {
        Destroy(gameObject);
    }

    public void UpdateList()
    {
        _unitInRange = _unitInRange.Where(x=>x != null).ToList();
    }
    IEnumerator AttackRoutine()
    {
        while (_unitInRange.Count!=0)
        {
            Attack();
            yield return new WaitForSeconds(AttakcSpeed);
        }
        _trainingCoroutine = null;
    }
    public void Start()
    {
        UnitInit(gameObject.name);
        _radiusSqrt = Math.Pow(_attackRange.radius * _attackRange.transform.localScale.x * _unitTransform.localScale.x, 2);
    }
    public void Update()
    {
        UpdateList();
        AddUnitInRange();
        _trainingCoroutine ??= StartCoroutine(AttackRoutine());
        if (isClient)
        {
            if (HP <= 0)
            {
                CmdDestroyUnit();
            }
        }
        RemoveUnit();
        
    }

   
}
