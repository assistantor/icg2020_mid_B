using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCrane : MonoBehaviour
{
    const float MOVE_SPEED = 2f;
    const float ROTATE_SPEED = 2f;
    const float ATTACH_DISTANCE = 3f;
    GameObject m_DetectedObject;
    
    ConfigurableJoint m_JointForObject;
    public ConfigurableJoint JointForObject { get { return m_JointForObject; } }

    [SerializeField] GameObject m_Jib;
    [SerializeField] GameObject m_Trolley;
    [SerializeField] GameObject m_Hook;

    [SerializeField] GameObject m_JointBody;
    [SerializeField] LineRenderer m_Cable;

    Camera m_ActiveCamera;

    #region Events
    public delegate void CraneMessageEvent(string message);

    public event CraneMessageEvent OnMessageAdded = (m) => { };

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RotateJib(string dir)
    {
        switch (dir)
        {
            case "counterclockwise":
                // Rotate counter clockwise.
                return;
            case "clockwise":
                // Rotate clockwise.
                return;
            default:
                return;
        }
    }
    public void MoveTrolley(string dir)
    {
        switch (dir)
        {
            case "forward":
                // Move forward.
                return;
            case "backward":
                // Move backward.
                return;
            default:
                return;
        }
    }
    
    public void DetectObjects()
    {
        Ray ray = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit; //Store the raycast result.

        if(Physics.Raycast (ray, out hit, ATTACH_DISTANCE))
        {
            if(m_DetectedObject == hit.collider.gameObject)
            {
                return;
            }
            RecoverDetectedObject();
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            
            if( renderer != null)
            {
                renderer.material.color = Color.yellow;
                m_DetectedObject = hit.collider.gameObject;
            }   
        }
        else
        {
            RecoverDetectedObject();
        }
    }
    public void RecoverDetectedObject()
    {
        if (m_DetectedObject != null)
        {
            m_DetectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
            m_DetectedObject = null;
        }
    }
    public void AttachOrDetachObject()
    {
        if(m_JointForObject == null)
        {
            if(m_DetectedObject != null)
            {
                var joint = m_JointBody.AddComponent<ConfigurableJoint>();
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;

                var limit = joint.linearLimit;
                limit.limit = 4;

                joint.linearLimit = limit;

                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = new Vector3(0f, 0.5f, 0f);
                joint.anchor = new Vector3(0f, 0f, 0f);

                joint.connectedBody = m_DetectedObject.GetComponent<Rigidbody>();

                m_JointForObject = joint;

                m_DetectedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                m_DetectedObject = null;
            }
        }
        else
        {
            m_JointForObject.connectedBody.GetComponent<MeshRenderer>().material.color = Color.white;
            GameObject.Destroy(m_JointForObject);
            m_JointForObject = null;
        }
    }
    public void UpdateCable()
    {
        m_Cable.enabled = (m_JointForObject != null);
        
        if (m_Cable.enabled)
        {
            m_Cable.SetPosition(0, this.transform.position);
            m_Cable.SetPosition(1, m_JointForObject.GetComponent<ConfigurableJoint>().connectedBody.transform.position);
        }
    }
}
