using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneGame : MonoBehaviour
{
    #region Events
    public delegate void MessageEvent(string message);
    public delegate void EntityEvent(GameObject entity);

    public event MessageEvent OnMessageAdded = (m) => { };

    public event EntityEvent OnEntitySelected = (e) => { };
    public event EntityEvent OnEntityDeselected = (e) => { };
    public event EntityEvent OnEntityAttatched = (e) => { };
    public event EntityEvent OnEntityDetatched = (e) => { };
    #endregion

    const float TROLLEY_FORWARD_LIMIT = -17f;
    const float TROLLEY_BACKWARD_LIMIT = -1f;
    const float TROLLEY_MOVE_SPEED_LIMIT = 4f;
    const float TROLLEY_JOINT_LENGTH_MAXIMUM = 14f;
    const float TROLLEY_JOINT_LENGTH_MINIMUM = 0.5f;
    const float HOOK_MOVE_SPEED = 5f;
    const float JIB_ROTATE_SPEED = 30f;
    const float ATTACH_DISTANCE = 3f;

    float trolleyJointLength = 1f;
    /*
    [SerializeField] TowerCrane m_TowerCrane;
    public TowerCrane TowerCrane { get { return m_TowerCrane; } }
    */


    [SerializeField] Camera[] m_Cameras = new Camera[2];

    CameraEntity m_ActiveCamera;
    public CameraEntity ActiveCamera { get { return m_ActiveCamera; } }

    public void SetActiveCamera(int index)
    {
        for(int i = 0; i < m_Cameras.Length; i++)
        {
            m_Cameras[i].enabled = i == index;
            m_Cameras[i].GetComponent<AudioListener>().enabled = i == index;
        }
        m_ActiveCamera = m_Cameras[index].GetComponent<CameraEntity>();
    }

    GameObject m_DetectedObject;

    ConfigurableJoint m_JointForObject;
    public ConfigurableJoint JointForObject { get { return m_JointForObject; } }

    PrimitivesGenerator m_Generator;
    public PrimitivesGenerator Generator { get { return m_Generator; } }

    [SerializeField] GameObject m_Jib;
    [SerializeField] GameObject m_Trolley;
    [SerializeField] GameObject m_Hook;

    [SerializeField] ConfigurableJoint[] m_TrolleyJoints = new ConfigurableJoint[4];

    [SerializeField] GameObject m_JointBody;
    [SerializeField] LineRenderer m_CableToHook;
    [SerializeField] LineRenderer m_CableToObject;
    [SerializeField] LineRenderer m_DetectAssistance;


    public void GenerateObject()
    {
        m_Generator = new PrimitivesGenerator(15, 20, 15);
    }
    public void RotateJib(string dir)
    {
        switch (dir)
        {
            case "counterclockwise":
                // Rotate counterclockwise.
                m_Jib.transform.Rotate(Vector3.down * JIB_ROTATE_SPEED * Time.deltaTime);
                break;
            case "clockwise":
                // Rotate clockwise.
                m_Jib.transform.Rotate(Vector3.up * JIB_ROTATE_SPEED * Time.deltaTime);
                break;
            default:
                Debug.Log("Jib: wrong rotation!");
                break;
        }
    }
    public void MoveTrolley(string dir)
    {
        switch (dir)
        {
            case "forward":
                // Move forward.
                m_Trolley.GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * 4f);
                if(m_Trolley.GetComponent<Rigidbody>().velocity.magnitude > TROLLEY_MOVE_SPEED_LIMIT)
                {
                    m_Trolley.GetComponent<Rigidbody>().velocity = m_Trolley.GetComponent<Rigidbody>().velocity.normalized * TROLLEY_MOVE_SPEED_LIMIT;
                }
                break;
            case "backward":
                // Move backward.
                m_Trolley.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 4f);
                if (m_Trolley.GetComponent<Rigidbody>().velocity.magnitude > TROLLEY_MOVE_SPEED_LIMIT)
                {
                    m_Trolley.GetComponent<Rigidbody>().velocity = m_Trolley.GetComponent<Rigidbody>().velocity.normalized * TROLLEY_MOVE_SPEED_LIMIT;
                }
                break;
            default:
                Debug.Log("Trolley: wrong direction!");
                break;
        }
    }
    public void TrolleyPositionCorrector()
    {
        //Debug.Log(m_Trolley.GetComponent<Rigidbody>().velocity.magnitude);
        if (m_Trolley.transform.localPosition.x != 0f)
        {
            m_Trolley.transform.Translate(new Vector3(- m_Trolley.transform.localPosition.x, 0, 0));
        }
        if (m_Trolley.transform.localPosition.y < TROLLEY_FORWARD_LIMIT)
        {
            m_Trolley.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_Trolley.transform.Translate(new Vector3(0, TROLLEY_FORWARD_LIMIT - m_Trolley.transform.localPosition.y, 0));
        }else if(m_Trolley.transform.localPosition.y > TROLLEY_BACKWARD_LIMIT)
        {
            m_Trolley.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_Trolley.transform.Translate(new Vector3(0, TROLLEY_BACKWARD_LIMIT - m_Trolley.transform.localPosition.y, 0));
        }
    }
    public void MoveHook(string dir)
    {
        switch (dir)
        {
            case "upward":
                // Move upward.
                trolleyJointLength = Mathf.Max(trolleyJointLength - HOOK_MOVE_SPEED * Time.deltaTime, TROLLEY_JOINT_LENGTH_MINIMUM);
                break;
            case "downward":
                // Move downward.
                trolleyJointLength = Mathf.Min(trolleyJointLength + HOOK_MOVE_SPEED * Time.deltaTime, TROLLEY_JOINT_LENGTH_MAXIMUM); ;
                break;
            default:
                Debug.Log("Hook: wrong direction!");
                break;
        }
        //Debug.Log(trolleyJointLength);
        UpdateTrolleyJoint(trolleyJointLength);
    }

    void UpdateTrolleyJoint(float jointLength)
    {
        for(int i = 0; i < m_TrolleyJoints.Length ; i++)
        {
            var limit = m_TrolleyJoints[i].linearLimit;
            limit.limit = jointLength;
            m_TrolleyJoints[i].linearLimit = limit;
        }
    }

    public void DetectObjects()
    {
        Ray ray = new Ray(m_Hook.transform.position, Vector3.down);
        RaycastHit hit; //Store the raycast result.

        if (Physics.Raycast(ray, out hit, ATTACH_DISTANCE))
        {
            if (m_DetectedObject == hit.collider.gameObject)
            {
                return;
            }
            RecoverDetectedObject();
            Entity entity = hit.collider.GetComponent<Entity>();

            if (entity != null)
            {
                m_DetectedObject = hit.collider.gameObject;
                OnEntitySelected(m_DetectedObject);
            }
        }
        else
        {
            RecoverDetectedObject();
        }
    }
    public void DetectAssistance()
    {
        CancelInvoke();
        m_DetectAssistance.enabled = true;
        m_DetectAssistance.SetPosition(0, m_Hook.transform.position);
        m_DetectAssistance.SetPosition(1, new Vector3(m_Hook.transform.position.x, 0, m_Hook.transform.position.z));
        Invoke("DetectAssistanceOff", 0.1f);
    }
    void DetectAssistanceOff()
    {
        m_DetectAssistance.enabled = false;
    }
    public void RecoverDetectedObject()
    {
        if (m_DetectedObject != null)
        {
            OnEntityDeselected(m_DetectedObject);
            m_DetectedObject = null;
        }
    }
    public void AttachOrDetachObject()
    {
        if (m_JointForObject == null)
        {
            if (m_DetectedObject != null)
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
                joint.connectedAnchor = new Vector3(0f, 16f, 0f);
                joint.anchor = new Vector3(0f, -1.5f, 0f);

                joint.connectedBody = m_DetectedObject.GetComponent<Rigidbody>();

                m_JointForObject = joint;

                OnEntityAttatched(m_DetectedObject);
                m_DetectedObject = null;
            }
        }
        else
        {
            OnEntityDetatched(m_JointForObject.gameObject);
            GameObject.Destroy(m_JointForObject);
            m_JointForObject = null;
        }
    }
    public void UpdateCable()
    {
        m_CableToHook.SetPosition(0, m_Trolley.transform.position);
        m_CableToHook.SetPosition(1, m_Hook.transform.position);

        m_CableToObject.enabled = (m_JointForObject != null);

        if (m_CableToObject.enabled)
        {
            m_CableToObject.SetPosition(0, m_Hook.transform.position);
            var connectedBodyTransform = m_JointForObject.connectedBody.transform;
            m_CableToObject.SetPosition(1, connectedBodyTransform.TransformPoint (m_JointForObject.connectedAnchor));
        }
    }
}
