using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitType : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner = null;
    [SerializeField] private List<Image> _queueList = null;
    [SerializeField] private List<Sprite> _spritesList = null;
    [SerializeField] private Slider _sliderBar = null;
    [SerializeField] private Text _sliderBarText = null;
    [SerializeField] private List<UnitStats> _unitStats = new List<UnitStats>();
    // [SerializeField] private Text time = null;
    private float _unitTrainingPercent =0;
    private Coroutine _trainingCoroutine;
    private float _timeMil = 0;

    private int _uType = 0;

    // Start is called before the first frame update

    #region Server



    #endregion

    #region Client
    private void Start()
    {
        foreach (var unitStatse in _unitStats)
        {
            unitStatse.UnitInit(unitStatse.gameObject.name +"(Clone)");

        }
    }

    private void Update()
    {
        int queueCount = _unitSpawner.GetCount();
        for (int i = 0; i < _queueList.Count; i++)
        {

            _queueList[i].sprite = null;
        }

        for (int i = 0; i < queueCount; i++)
        {
            _queueList[i].sprite = _spritesList[_unitSpawner.GetUnit(i)];
        }

        if (_unitSpawner.IsQueued())
        {
            _timeMil += Time.deltaTime;
            _unitTrainingPercent = _timeMil / (_unitStats[_unitSpawner.GetFirst()].TrainingSpeed);
            _sliderBar.value = _unitTrainingPercent;
            _sliderBarText.text = Math.Round(_unitTrainingPercent * 100) + "%";
            _sliderBar.gameObject.SetActive(true);
           
        }
        else
        {
            _sliderBar.gameObject.SetActive(false);
        }
       
       
    }

    public void Choose()
    {

        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "Peasant":
                _uType = 0;
                break;
            case "Archer":
                _uType = 1;
                break;
            default:
                Debug.Log("nic nie robie");
                break;
        }
        var player  = NetworkClient.localPlayer.gameObject.GetComponent<PlayerManager>();
        if (player.Gold - (_unitStats[_uType].TrainingCost) < 0)
        {
            Debug.Log("Nie sta� Cie biedaku");
            return;
        }
        if (_unitSpawner.GetCount() >= _queueList.Count)
        {
            Debug.Log("Kolejka pe�na");
            return;
        }

        player.Gold -= (_unitStats[_uType].TrainingCost);
        _unitSpawner.AddToQueue(_uType);
        _trainingCoroutine ??= StartCoroutine(TrainingUnit());


    }

    private IEnumerator TrainingUnit()
    {
        Debug.Log("Poczatek");
        while (_unitSpawner.IsQueued())
        {

            yield return new WaitForSeconds(_unitStats[_unitSpawner.GetFirst()].TrainingSpeed);
            if (_unitSpawner.IsQueued())
            {
                _unitSpawner.CmdUnitType(_unitSpawner.GetFirst());
                _unitSpawner.CmdSpawnUnit();
                _unitSpawner.RemoveFromQueue(_unitSpawner.GetFirst());
                _timeMil = 0;
                _unitTrainingPercent = 0;
                Debug.Log("Srodek");
            }
        }
        _trainingCoroutine = null;
    }

    #endregion

}
