using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MountainViewType { MVT_SAMPLE, MVT_TARGET, MVT_CONFIGURAL, MVT_SPATIAL, MVT_ELEMENTAL };

public class TableViewGenerator : SpatialViewGenerator
{

    // The Mountain Scene Generator class should generate "views" comprised of multiple topographic features
    // in specific arrangements (cf. The Four Mountains Test).

    // A "view" is comprised of:
    // i) Place (unique combination of topographic features and "noise")
    // ii) Conditions: changeable lighting, weather and vegetation cues
    // iii) Viewpoint: changeable camera location

    // The same place always combines the same unique topographic features in the same locations with the same "noise".
    // A different place has a distinct arrangement of topographic features AND distinct "noise".

    // For a given target place (and view), it is possible to generate one or more comparison places (and view).
    // Comparison places have similar topographic elements to the target, but are guaranteed to be distinct in different ways:
    // i) configural foil: two features are swapped (the order around the origin is changed)
    // ii) spatial foil: one or more features is moved, while the order around the origin is preserved
    // iii) elemental foil: one or more features is replaced by a distinctive alternative.

    // Target/comparison views can be generated with the same conditions, or with different conditions.

    // Target/comparison views can be generated with a variety of viewpoints.
    // The VisibilityTester class is used to determine valid viewpoints from which all distinctive features can be seen.

    //public List<MountainView> views;
    //public TopoArrayGenerator topoarraygenerator; //the TopoArrayGenerator is set in the Unity Editor and includes a list of distinctive 
    //topographical elements that are used to generate the required views
    //public ConditionsController conditionscontroller; //The ConditionsController is set in the Unity Editor and includes a list of distinctive non-spatial conditions
    //(time of day, weather, vegetation colors etc.)

    //This class is accessed by updating the view list, which in turn calls the SpatialViewGenerator for much of the "heavy lifting" - populating the list with valid
    //MountainViews which contain all the information needed to render a valid mountain landscape to a terrain (done using  the TopoArray object).

    //The normal process would be to add various views to the list, and then, when ready call the topoarray build functions.

    //PrefabArrayGenerator prefabarraygenerator;

    void Start()
    {
        
        //maxconditions = conditionscontroller.conditions.Count;
        maxdistinctfeatures = arraygenerator.ndistinctelements;
        Debug.Log("Initializing Spatial View Generator");
        //Debug.Log("ViewController views: " + viewcontroller.viewpoints.Count);
        //Debug.Log("ConditionsController conditions: " + conditionscontroller.conditions.Count);
        //prefabarraygenerator = GameObject.Find("PrefabArrayGenerator").GetComponent<PrefabArrayGenerator>();
        maxviews =  viewcontroller.nViewpoints-1; //maybe not -1
        Debug.Log(maxviews + "maxviews");
    }

    public void BuildViewList(List<MountainViewType> types)
    {
        SpatialViewGenerator svg = new SpatialViewGenerator();

        foreach (MountainViewType item in types)
        {
            SpatialViewType spatialitem = (SpatialViewType)item;
            svg.AppendView(spatialitem);
        }
        List<SpatialView> spatialviews = svg.GetViews();
        foreach (SpatialView view in spatialviews)
        {
            views.Add((TableView)view);
        }
    }

    public void ClearViewList()
    {
        views.Clear();
    }

}

[System.Serializable]
public class TablePlace : SpatialScene
{
    public new List<TabletopElement> features;

    public TablePlace(SpatialScene scene)
    {
        foreach (SpatialElement f in scene.features)
        {
            features.Add((TabletopElement)f);   
        }
    }
}

[System.Serializable]
public class TableConditions : SpatialConditions
{
    //public string datetime; //System.DateTime time; //used to set the lighting, time/date in conjunction with Ultimate Sky
    //public float overcast;
    //public float gloominess;
    ///public float fogdensity;
    //public Material material;

    //although not being used now this space can be used to change differetn conditional elements of the scene - for example lighting or distal objects. 
}

[System.Serializable]
public class TableView : SpatialView
{
    public TablePlace place;
    public new TableConditions conditions;

    public TableView (SpatialView view)
    {
        place = (TablePlace)view.scene;
        //conditions = (MountainConditions)view.conditions;
    }
}