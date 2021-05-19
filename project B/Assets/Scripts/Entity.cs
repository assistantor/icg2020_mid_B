using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    #region Events
    public delegate void ScoreEvent(int score);
    public delegate void EntityEvent(Entity entity);

    public event ScoreEvent OnEntitySetScore = (s) => { };

    public event EntityEvent OnEntityIn = (e) => { };
    public event EntityEvent OnEntityOut = (e) => { };
    public event EntityEvent OnEntityAway = (e) => { };
    #endregion

    bool isIn50 = false;
    bool isIn100 = false;
    bool isIn200 = false;
    bool isRepeat = false;
    bool isOutside = false;
    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("in: " + other.name);

        if (other.name == "Base Cube (50)")
        {
            isIn50 = true;
            OnEntitySetScore(50);
        }
        if (other.name == "Second Cube (100)")
        {
            isIn100 = true;
            OnEntitySetScore(100);
        }
        if (other.name == "Third Cube (200)")
        {
            isIn200 = true;
            OnEntitySetScore(200);
        }
        OnEntityIn(this);
        CheckScore();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Base Cube (50)")
        {
            isIn50 = true;
        }
        if (other.name == "Second Cube (100)")
        {
            isIn100 = true;
        }
        if (other.name == "Third Cube (200)")
        {
            isIn200 = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("out: " + other.name);
        if (!isRepeat)
        {
            //Debug.Log("Not repeating!");
            if (other.name == "Base Cube (50)")
            {
                isIn50 = false;
                OnEntitySetScore(-50);
            }
            if (other.name == "Second Cube (100)")
            {
                isIn100 = false;
                OnEntitySetScore(-100);
            }
            if (other.name == "Third Cube (200)")
            {
                isIn200 = false;
                OnEntitySetScore(-200);
            }

            if (!isIn50 && !isIn100 && !isIn200) OnEntityOut(this);
        }
        else
        {
            //Debug.Log("Reset repeat");
            if (other.name == "Base Cube (50)")
            {
                isIn50 = false;
            }
            if (other.name == "Second Cube (100)")
            {
                isIn100 = false;
                if (isIn50) OnEntitySetScore(-50);
            }
            if (other.name == "Third Cube (200)")
            {
                isIn200 = false;
                if (isIn100) OnEntitySetScore(-100);
            }
            isRepeat = false;
        }

    }
    private void CheckScore()
    {
        if (isIn50 && isIn100 && isIn200)
        {
            Debug.Log("error");
        }
        if (isIn100 && isIn200)
        {
            //Debug.Log("Repeating 100, 200  --> -100");
            isRepeat = true;
            OnEntitySetScore(-100);
        }
        else if (isIn50 && isIn100)
        {
            //Debug.Log("Repeating 50, 100  --> -50");
            isRepeat = true;
            OnEntitySetScore(-50);
        }
    }
    private void FixedUpdate()
    {
        if (this.transform.position.y < -10)
        {
            isOutside = true;
            Invoke("Destory", 2f);
        }
    }
    private void Destory()
    {
        OnEntityAway(this);
        GameObject.Destroy(this);
    }
}