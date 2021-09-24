using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//this script does a number of things - theprefab array generator assembles lists of prefabs with locations and other information into the array spec list. this is then spawned wit the spawn objects script.
//This script also contains the tableTopElement class which extends the abstract class of spatial element to include features useful to the spawning of prefabs - namely an associated game object.


    // this script has been expanded from the topo array generator to also include prefab initialisationa nd spawning. it did not seem necessary to have two scripts to bote define the array and (2) spawn. The array spawing function could also have been included within the table object class but I wanted
    //to keep the array spawning method linked to lists of table elements rather than individual table elements - so that it was more similar to the mountains code.


    // to prepare prefabs for the experiment you need to position them at 0 0 0 and attach an empty game object to them called "bottom". position this at the point at which the prefab should be touching the floor.


public enum TableElementProfile { TABLE_ELEMENT_SPHERE, TABLE_ELEMENT_CUBE, TABLE_ELEMENT_CONE, TABLE_ELEMENT_CYLINDER, TABLE_ELEMENT_TAURUS, TABLE_ELEMENT_CROSS, TABLE_ELEMENT_TETRAHEDRON, TABLE_ELEMENT_OCTOHEDRON, TABLE_ELEMENT_STARPRISM, TABLE_ELEMENT_STEPPYRAMID };

public class PrefabArrayGenerator : SpatialArrayGenerator
{
    public enum TableState { TS_INITIALIZE, TS_SPAWN, TS_IDLE }

    public TableState state = TableState.TS_INITIALIZE;
    public List<TabletopElement> topofeatures;
    public Vector2 centre;
    public int noise_seed;
    public EnvironmentSpecifications env;
    GameObject lab;
    public Renderer rend;
    [Range(0, 200)]
    public float PrefabPercentageSize;
    public List<TabletopElement> distinctelements = new List<TabletopElement>(); // this is the list that should be populated in the inspector with possible objects.
    public List<TabletopElement> arrayspec;
    //public TopoArray topoarray;
    private void Start()
    {
        lab = GameObject.Find("Lab");
        //this calculates the size of prefabs by drawing a sphere around them.
        for (int i = 0; i < distinctelements.Count; i++)
        {
            rend = distinctelements[i].prefabtype.GetComponent<Renderer>();
            distinctelements[i].height = distinctelements[i].radius = (rend.bounds.extents.magnitude / 100) * PrefabPercentageSize;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            //UpdateArray();
        }
        switch (state)
        {
            case TableState.TS_SPAWN:
                {

                    state = TableState.TS_IDLE;
                    break;
                }

        }
    }

    public float GetRadius(GameObject tabletopelement)
    {
        float objRadius = 2.5f;

        return objRadius;
    }

    public override SpatialElement DistinctElement(int i)
    {
        Debug.Log("index" + 1 + "distinct elements array size" + distinctelements.Count);

        SpatialElement s = new SpatialElement();
        s.centre = distinctelements[i].centre;
        s.height = distinctelements[i].height;
        s.index = i;
        s.radius = distinctelements[i].radius;

        return s;
    }

    public override void UpdateArray(SpatialScene s)
    {
        // Debug.Log("Attempting to render new scene");
        arrayspec = new List<TabletopElement>();
        for (int i = 0; i < s.features.Count; i++)
        {
            arrayspec.Add(distinctelements[s.features[i].index].Copy());
            arrayspec[i].centre = s.features[i].centre + centre;
        }
        SpawnObjects(arrayspec);
    }


    public void SpawnObjects(List<TabletopElement> arrayspec)
    {
        state = TableState.TS_SPAWN;
        topofeatures = new List<TabletopElement>(arrayspec);
        GameObject[] destroyItems;
        destroyItems = GameObject.FindGameObjectsWithTag("clone");
        for (int i = 0; i < destroyItems.Length; i++)
        {
            Destroy(destroyItems[i].gameObject);
        }

        for (int i = 0; i < topofeatures.Count; i++)
        {
            GameObject objBottom = topofeatures[i].prefabtype.transform.Find("bottom").gameObject; // these two lines find the distance that the prefab needs to move so that it appears to stand on the surface. it does this by using an empty game object that is places as a child to the prefab and positioned at the bottom of the object.
            float objHeightOffset = Mathf.Sqrt( Mathf.Pow(((objBottom.transform.localPosition.y / 100) * PrefabPercentageSize),2));

            GameObject spawn = Instantiate(topofeatures[i].prefabtype); // instances prefab of choice
            topofeatures[i].instance = spawn;
            arrayspec[i].instance = spawn;
            //Debug.Log(objHeightOffset + "object height offset");
            spawn.transform.parent = lab.transform;
            
            spawn.transform.position = new Vector3(topofeatures[i].centre.x, env.arenafloor.transform.position.y + objHeightOffset, topofeatures[i].centre.y);
            spawn.transform.localScale = new Vector3((spawn.transform.localScale.x / 100) * PrefabPercentageSize, (spawn.transform.localScale.y / 100) * PrefabPercentageSize, (spawn.transform.localScale.z / 100) * PrefabPercentageSize);//moves object to location.
        }



        state = TableState.TS_IDLE;
    }
}

    [System.Serializable]

public class TabletopElement : SpatialElement
{
    public TableElementProfile profile;

    public GameObject prefabtype;
    public GameObject instance;

    public TabletopElement(TableElementProfile newprofile, Vector2 newcentre, float newradius, float newheight, GameObject newprefabtype)
    {
        //explicit constructor
        prefabtype = newprefabtype;
        profile = newprofile;
        centre = newcentre;
        radius = newradius;
        //radius = GetRadius(prefabtype);
        height = newheight;
    }

    public TabletopElement Copy()
    {
        TabletopElement temp = new TabletopElement(profile, centre, radius, height, prefabtype);
        return temp;
    }



}
