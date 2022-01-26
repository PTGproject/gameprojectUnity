using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UnitStats : NetworkBehaviour
{
    [SerializeField] [SyncVar] private float _maxHp;
    [SerializeField] [SyncVar] private float _currentHealth;
    [SerializeField] [SyncVar] private float _dmg;
    [SerializeField] [SyncVar] private float _trainingSpeed;
    [SerializeField] [SyncVar] private float _attackSpeed;
    [SerializeField] [SyncVar] private float _trainingCost;
    [SerializeField] private double _radiusSqrt;
    [SerializeField] [SyncVar] private List<Unit> _unitInRange = new List<Unit>();
    [SerializeField] private Transform _unitTransform = null;
    [SerializeField] private CircleCollider2D _attackRange = null;
    [SerializeField] public Canvas healthCanvas = null;
    private Coroutine _trainingCoroutine;
    public bool isBuilding = false;

    [SerializeField] private Slider _sliderBar = null;

    public GameObject healthBar;

    
    public float MAXHP
    {
        get
        {
            return _maxHp;
        }
        set
        {
            _maxHp = value;
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
                MAXHP = 10;
                TrainingSpeed = 2;
                AttakcSpeed = 1;
                TrainingCost = 100;
                break;
            case "UnitArcher(Clone)":
                AttakcSpeed = 2;
                Dmg = 2;
                MAXHP = 5;
                TrainingSpeed = 4;
                TrainingCost = 200;
                break;
            case "UnitStronghold(Clone)":
                MAXHP = 100;
                Dmg = 0;
                AttakcSpeed = 1000;
                isBuilding = true;
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
        _currentHealth -= dmg;

        healthBar.GetComponent<HealthBar>().SetHealth(_currentHealth);
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

        _currentHealth = _maxHp;
        healthBar.GetComponent<HealthBar>().SetMaxHealth(_maxHp);
    }
    public void Update()
    {
        //if (Mouse.current.rightButton.wasPressedThisFrame)
        //{
        //    TakeDmg(2.0f);
        //}
            

        UpdateList();
        AddUnitInRange();
        _trainingCoroutine ??= StartCoroutine(AttackRoutine());
        if (isClient)
        {
            if (_currentHealth <= 0)
            {
                CmdDestroyUnit();
            }
        }
        RemoveUnit();
        
    }

   
}
