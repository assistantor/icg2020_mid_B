using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitivesPicker : MonoBehaviour
{
    GameObject m_ClickedObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireRayCast();
        }else if (Input.GetMouseButtonUp(0))
        {
            RecoverClickedObject();
        }
    }
    void FireRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Store the raycast result. Note that it's Structure type (not reference)!

        if(Physics.Raycast(ray, out hit))
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;

                m_ClickedObject = hit.collider.gameObject;
            }
        }
    }
    void RecoverClickedObject()
    {
        if (m_ClickedObject != null)
        {
            m_ClickedObject.GetComponent<MeshRenderer>().material.color = Color.white;
            m_ClickedObject = null;
        }
    }
}
