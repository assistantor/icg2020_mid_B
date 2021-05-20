using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneGameScene : MonoBehaviour
{
    Color leafColor;

    // Start is called before the first frame update
    void Awake()
    {
        m_Game.GenerateObject();
        foreach(Entity e in m_Game.Generator.Entities)
        {
            e.OnEntitySetScore += HandleOnEntitySetScore;
            e.OnEntityIn += HandleOnEntityIn;
            e.OnEntityOut += HandleOnEntityOut;
            e.OnEntityAway += HandleOnEntityAway;
            leafColor = e.GetComponent<MeshRenderer>().material.color;
        }
        m_Game.OnMessageAdded += HandleOnMessageAdded;

        m_Game.OnEntitySelected += HandleOnEntitySelected;
        m_Game.OnEntityDeselected += HandleOnEntityDeselected;
        m_Game.OnEntityAttatched += HandleOnEntityAttatched;
        m_Game.OnEntityDetatched += HandleOnEntityDetatched;
    }

    private void HandleOnEntityIn(Entity e)
    {
        e.GetComponent<MeshRenderer>().material.color = Color.green;

        m_GameUI.ShowMessage(string.Format("Great job! <color=green>{0}</color>! is in.", e.name));
    }
    private void HandleOnEntityOut(Entity e)
    {
        e.GetComponent<MeshRenderer>().material.color = leafColor;
        m_GameUI.ScoreOut();
        m_GameUI.ShowMessage(string.Format("Oops! <color=green>{0}</color>! is out.", e.name));
    }
    private void HandleOnEntityAway(Entity e)
    {
        e.GetComponent<MeshRenderer>().material.color = Color.red;
        m_GameUI.ScoreCal(-100);
        m_GameUI.ScoreOut();
        m_GameUI.ShowMessage(string.Format("Oops! <color=green>{0}</color>! is falling out.", e.name));
    }
    private void HandleOnEntitySetScore(int e)
    {
        m_GameUI.ScoreCal(e);
    }

    private void HandleOnEntityDetatched(GameObject e)
    {
        Renderer r = e.GetComponent<ConfigurableJoint>().connectedBody.GetComponent<MeshRenderer>();
        if (r.material.color != Color.green) r.material.color = leafColor;

        m_GameUI.ShowMessage(string.Format("Detatched <color=green>{0}</color>!", e.GetComponent<ConfigurableJoint>().connectedBody.gameObject.name));
    }
    private void HandleOnEntityAttatched(GameObject e)
    {
        e.GetComponent<MeshRenderer>().material.color = Color.cyan;

        m_GameUI.ShowMessage(string.Format("Attatched <color=green>{0}</color>!", e.name));
    }

    private void HandleOnEntityDeselected(GameObject e)
    {
        Renderer r = e.GetComponent<MeshRenderer>();
        if (r.material.color != Color.green) r.material.color = leafColor;
    }
    private void HandleOnEntitySelected(GameObject e)
    {
        Renderer r = e.GetComponent<MeshRenderer>();
        if (r.material.color != Color.green) r.material.color = Color.yellow;
    }

    private void HandleOnMessageAdded(string message)
    {
        m_GameUI.ShowMessage(message);
    }
    private void Start()
    {
        m_Game.SetActiveCamera(0);
    }
    // Update is called once per frame
    void Update()
    {
        // Control tower crane model
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_Game.RotateJib("counterclockwise");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_Game.RotateJib("clockwise");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_Game.MoveTrolley("forward");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            m_Game.MoveTrolley("backward");
        }
        m_Game.TrolleyPositionCorrector();

        // Select camera

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Select camera 1
            if (cameraIndex == 1)
            {
                m_Game.ActiveCamera.ResetCamera();
            }
            cameraIndex = 0;
            m_Game.SetActiveCamera(cameraIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Select camera 2
            cameraIndex = 1;
            m_Game.SetActiveCamera(cameraIndex);
        }

        //  Detect object
        if (m_Game.JointForObject == null)
        {
            m_Game.DetectObjects();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Game.AttachOrDetachObject();
        }

        m_Game.UpdateCable();

        // Detect assistance
        if (Input.GetKey(KeyCode.C))
        {
            m_Game.DetectAssistance();
        }

    }
    private void FixedUpdate()
    {
        // Control tower crane model
        if (Input.GetKey(KeyCode.Z))
        {
            m_Game.MoveHook("downward");
        }
        else if (Input.GetKey(KeyCode.X))
        {
            m_Game.MoveHook("upward");
        }

        // Control camera
        if (cameraIndex == 0)
        {
            // camera 1 (free camera)
            if (Input.GetKey(KeyCode.Q))
            {
                m_Game.ActiveCamera.CameraMove("upward");
            }
            else if (Input.GetKey(KeyCode.E))
            {
                m_Game.ActiveCamera.CameraMove("downward");
            }
            if (Input.GetKey(KeyCode.W))
            {
                m_Game.ActiveCamera.CameraMove("forward");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                m_Game.ActiveCamera.CameraMove("backward");
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_Game.ActiveCamera.CameraMove("leftward");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                m_Game.ActiveCamera.CameraMove("rightward");
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                m_Game.ActiveCamera.CameraRotate("freeReset");
            }else if (Input.GetKey(KeyCode.Mouse0))
            {
                m_Game.ActiveCamera.CameraRotate("freeControl");
            }
        }
        else
        {
            // camera 2 (fixed camera)
            if (Input.GetKey(KeyCode.A))
            {
                m_Game.ActiveCamera.CameraRotate("counterclockwise");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                m_Game.ActiveCamera.CameraRotate("clockwise");
            }
            if (Input.GetKey(KeyCode.W))
            {
                m_Game.ActiveCamera.CameraRotate("upward");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                m_Game.ActiveCamera.CameraRotate("downward");
            }
        }
    }
}
