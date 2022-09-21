using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyInspector : MonoBehaviour
{
    public Enemy inspectedEnemy;

    public Canvas inspectorCanvas;
    public TextMeshProUGUI text;

    public void Start()
    {
        DisableCanvas();
    }

    public void Update()
    {
        // When pressed try to inspect an enemy
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Raycast from mouse point
            RaycastHit hit;
            Ray inspectorRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(inspectorRay, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
            {
                inspectedEnemy = hit.collider.gameObject.GetComponent<Enemy>();
                EnableCanvas();
            }
        }

        // If inspected enemy disables then hide inspector
        if (inspectedEnemy != null && !inspectedEnemy.gameObject.activeSelf)
        {
            inspectedEnemy = null;
            DisableCanvas();
        }
    }

    // Enable canvas and set text
    public void EnableCanvas()
    {
        inspectorCanvas.enabled = true;
        text.text = "Inspecting " + inspectedEnemy.gameObject.name + 
            "\nHealth: " + inspectedEnemy.stats.health +
            "\nAttack Power: " + inspectedEnemy.stats.attackPower +
            "\nSpeed: " + inspectedEnemy.stats.speed;
    }

    // Hide canvas
    public void DisableCanvas()
    {
        inspectorCanvas.enabled = false;
    }
}
