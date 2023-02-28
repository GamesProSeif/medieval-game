using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManger : MonoBehaviour
{

    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private float rayRange;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform raySource;

    public Transform mainSelection;
    private Material defaultMat;

    private void Start()
    {
        raySource = Camera.main.transform;
        layerMask = LayerMask.GetMask("Item");
        mainSelection = null;
    }
    void Update()
    {
        if(mainSelection != null)
        {
            var SelectionRenderer = mainSelection.GetComponent<Renderer>();
            SelectionRenderer.material = defaultMat;
            mainSelection = null;
        }

        RaycastHit hit;
        
        if (Physics.Raycast(raySource.position, raySource.TransformDirection(Vector3.forward), out hit, rayRange, layerMask))
        {
           
            Debug.DrawRay(raySource.position, raySource.TransformDirection(Vector3.forward)* hit.distance, Color.green );
            
            var Selection = hit.transform;
            var SelectionRenderer = Selection.GetComponent<Renderer>();
            defaultMat = SelectionRenderer.material;

            if (Selection.CompareTag("Item"))
            {
               
                {
                   
                    if(SelectionRenderer != null)
                    {
                        SelectionRenderer.material = highlightedMaterial;
                    }
                    mainSelection = Selection;

                }
            }

        }

    }
}
