using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitType : MonoBehaviour
{
    [SerializeField] private UnitSpawner unitSpawner = null;
    private Coroutine trainingCoroutine;

    private int uType = 0;

    // Start is called before the first frame update

    #region Server



    #endregion

    #region Client
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick
            .AddListener(Choose );

    }
    
    private void Choose()
    {

        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "Peasant":
                uType = 0;
                break;
            case "Archer":
                uType = 1;
                break;
            default:
                Debug.Log("nic nie robie");
                break;
        }
        unitSpawner.queue.Add(uType);
        trainingCoroutine ??= StartCoroutine(TrainingUnit());


    }

    private IEnumerator TrainingUnit()
    {
        Debug.Log("Poczatek");
        while (unitSpawner.queue.Count > 0)
        {

            yield return new WaitForSeconds(unitSpawner.queue.First() * 2f + 2f);
            if (unitSpawner.queue.Count > 0)
            {
                unitSpawner.CmdUnitType(unitSpawner.queue.First());
                unitSpawner.CmdSpawnUnit();
                unitSpawner.queue.Remove(unitSpawner.queue.First());
                Debug.Log("Srodek");
            }
        }
        trainingCoroutine = null;
    }

    #endregion

}
