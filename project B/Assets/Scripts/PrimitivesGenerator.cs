using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitivesGenerator
{
    GameObject m_CubePrefab = Resources.Load<GameObject>("Cube");
    GameObject m_SpherePrefab = Resources.Load<GameObject>("Sphere");
    GameObject m_CarrotPrefab = Resources.Load<GameObject>("Carrot");

    Vector2 m_Dimention;
    const int SCALE_LIMIT = 5;

    List<Entity> m_Entities = new List<Entity>();
    public List<Entity> Entities { get { return m_Entities; } }

    Color leafColor;
    public Color getLeafColor { get {return m_Entities[0].GetComponent<MeshRenderer>().material.color;} }

    // Start is called before the first frame update
    public PrimitivesGenerator(int min, int max, int baseLength)
    {
        m_Dimention = new Vector2(baseLength - 1, baseLength - 1);
        GeneratePrimitives(m_CarrotPrefab, Random.Range(min, max));
        //GeneratePrimitives(m_SpherePrefab, Random.Range(min, max));
        //GeneratePrimitives(m_CubePrefab, Random.Range(min, max));
    }

    void GeneratePrimitives (GameObject primitives, int count)
    {
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {
            pos = new Vector3(
                Random.Range(-m_Dimention.x, m_Dimention.x),
                5f,
                Random.Range(-m_Dimention.y, m_Dimention.y));
            
            if(pos.x > 4 && pos.z > 4)
            {
                i--;
                continue;
            }

            var primitiveIns = GameObject.Instantiate(primitives);
            primitiveIns.transform.localPosition = pos;

            float scale = 0.05f*Random.Range(2, SCALE_LIMIT);
            primitiveIns.transform.localScale = new Vector3(scale, scale, scale);
            primitiveIns.name = primitives.name + " " + (i + 1);

            m_Entities.Add(primitiveIns.GetComponent<Entity>());
        }
    }

}
