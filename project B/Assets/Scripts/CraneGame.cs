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
    const float TROLLEY_MOVE_SPEED = 4f;
    const float HOOK_UPWARD_LIMIT = -0.25f;
    const float HOOK_DOWNWARD_LIMIT = -12f;
    const float HOOK_MOVE_SPEED = 5f;
    const float JIB_ROTATE_SPEED = 30f;
    const float ATTACH_DISTANCE = 5f;

    float trolleyPosition = -1f;
    float hookPosition = 0f;
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

    [SerializeField] GameObject m_JointBody;
    [SerializeField] LineRenderer m_CableToHook;
    [SerializeField] LineRenderer m_CableToObject;


    public void GenerateObject()
    {
        m_Generator = new PrimitivesGenerator(10, 15, 15);
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
                trolleyPosition = Mathf.Max(m_Trolley.transform.localPosition.y - TROLLEY_MOVE_SPEED * Time.deltaTime, TROLLEY_FORWARD_LIMIT);
                break;
            case "backward":
                // Move backward.
                trolleyPosition = Mathf.Min(m_Trolley.transform.localPosition.y + TROLLEY_MOVE_SPEED * Time.deltaTime, TROLLEY_BACKWARD_LIMIT);
                break;
            default:
                Debug.Log("Trolley: wrong direction!");
                break;
        }
        m_Trolley.transform.Translate(Vector3.down * (m_Trolley.transform.localPosition.y - trolleyPosition));
    }
    public void MoveHook(string dir)
    {
        switch (dir)
        {
            case "upward":
                // Move upward.
                hookPosition = Mathf.Min(m_Hook.transform.localPosition.z + HOOK_MOVE_SPEED * Time.deltaTime, HOOK_UPWARD_LIMIT);
                break;
            case "downward":
                // Move downward.
                hookPosition = Mathf.Max(m_Hook.transform.localPosition.z - HOOK_MOVE_SPEED * Time.deltaTime, HOOK_DOWNWARD_LIMIT);
                break;
            default:
                Debug.Log("Hook: wrong direction!");
                break;
        }
        m_Hook.transform.Translate(Vector3.back * (m_Hook.transform.localPosition.z - hookPosition));
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
                joint.connectedAnchor = new Vector3(0f, 0.5f, 0f);
                joint.anchor = new Vector3(0f, 0f, 0f);

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
            m_CableToObject.SetPosition(1, m_JointForObject.connectedBody.transform.position);
        }
    }
}
