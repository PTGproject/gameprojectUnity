using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unit;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    // selection box
    public RectTransform selectionBox;
    private Vector2 startPos;

    [SerializeField]public List<Unit> selectedUnits  = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            startPos = Mouse.current.position.ReadValue();

            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }

            selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // TrySelect(Mouse.current.position.ReadValue());
            ReleaseSelectionBox();
            startPos = Mouse.current.position.ReadValue();
            SelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            selectedUnits = new List<Unit>();
            UpdateSelectionBox(Mouse.current.position.ReadValue());
        }
    }

    private void SelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if (!unit.hasAuthority) { return; }
        selectedUnits.Add(unit);

        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }
    }

    // called when we are creating a selection box
    void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy && selectionBox.sizeDelta.x > 15 && selectionBox.sizeDelta.y > 15)
            selectionBox.gameObject.SetActive(true);
        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
    }


    // called when we release the selection box
    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);
        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);


        var units = GameObject.FindObjectsOfType(typeof(Unit));
        foreach (Unit unit in units)
        {

            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)

            {
                if (unit.GetComponent<UnitStats>().isBuilding) { continue; }
                if (unit.hasAuthority)
                {
                    selectedUnits.Add(unit);
                }
            }

        }
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}