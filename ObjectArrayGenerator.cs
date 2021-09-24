using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectArrayGenerator : SpatialArrayGenerator
{

    public List<ObjectElement> arrayspec = new List<ObjectElement>();

    void Start()
    {

    }
}

[System.Serializable]

public class ObjectElement : SpatialElement
{
    //e.g., a cube

    public Transform objectprefab;

    public ObjectElement(Transform newobjectprefab, Vector2 newcentre, float newradius, float newheight)
    {
        objectprefab = newobjectprefab;
        centre = newcentre;
        radius = newradius;
        height = newheight;
    }

}
