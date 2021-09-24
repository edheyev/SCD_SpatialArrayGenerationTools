using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpatialViewType { SVT_SAMPLE, SVT_TARGET, SVT_CONFIGURAL, SVT_SPATIAL, SVT_ELEMENTAL };

public class SpatialViewGenerator : MonoBehaviour
{

    // The Spatial View Generator class should generate "views" comprised of multiple elements
    // in specific arrangements (cf. The Four Mountains Test).

    // The Spatial View Generator is intended as an abstract class from which concrete classes can be derived.
    // These might, for example, describe a terrain with an array of topographical features - mountains, or an array of objects.
    // The idea is that much of the logic required to generate comparable scenes is shared,
    // whatever  the elements are.

    // A "View" is comprised of:
    // i) Scene (unique spatial arrangement of distinct elements)
    // ii) Conditions: changeable non-spatial properties
    // iii) Viewpoint: changeable camera location

    // A given scene will always combine the same distinctive elements in the same locations,
    // but with, potentially, different conditions and viewpoints.

    // For a given target scene (or view), it is possible to generate one or more comparison scenes (or view).
    // Comparison scenes have similar elements to the target, but are guaranteed to be distinct in different ways:
    // i) configural foil: two elements are swapped (the order around the origin is changed)
    // ii) spatial foil: one or more elements is moved, while the order around the origin is preserved
    // iii) elemental foil: one or more elements is replaced by a distinctive alternative.

    // Target/comparison views can be generated with the same conditions, or with different conditions.

    // Target/comparison views can be generated with a variety of viewpoints.
    // The VisibilityTester class is used to determine valid viewpoints from which all distinctive elements can be seen.

    public List<SpatialView> views=new List<SpatialView>();
    public SpatialArrayGenerator arraygenerator; // used to generate the abstract spatial arrays
    public ConditionsController conditionscontroller; //used to determine how many (abstract) conditions settings are available
    public ViewController viewcontroller; //used to determine how many (abstract) viewpoints are available
    public SpatialViewRandomizer randomizer; //this should be used to generate all random numbers involved in stimulus construction in a reproducible way
    public NoiseBuffer noisebuffer;


    public uint randSeed; //this is the master seed which controls the randomizer - the randomizer handles the seed in such a way that even when the master seed is simply incremented there
                         //should be no dependencies among the random stimulus elements it controls (other than those deliberately specified)
                         //for example, we can deliberately create a target which is topographically identical to an existing sample
                         //or we can deliberately create a spatial foil which is topographically similar to the sample but with reproducible, random, spatial changes to
                         //specific stimulus elements

    //camel case is used for these public variables for nice display in Unity editor

    public int nElements;
    public float maxRadius;
    public float sampleMaxAbsRadius;
    public float sampleMinAbsRadius;
    public float sampleMaxDeltaAngle;
    public float sampleMaxDeltaRadius;
    public float sampleMinDeltaAngle;
    public float sampleMinDeltaRadius;
    public int sampleProtectElement;
    public int configNToSwap;
    public float spatialDistortionLevel;
    float spatialMaxAbsRadius; //deprecated along with GenerateSpatialFoilOld
    float spatialMinAbsRadius;
    float spatialMaxDeltaAngle;
    float spatialMaxDeltaRadius;
    float spatialMinDeltaAngle;
    float spatialMinDeltaRadius;
    public int spatialProtectElement = -1;
    public int elementalNToChange;

    public int maxconditions;
    public int maxviews;
    public int maxdistinctfeatures;

    public EnvironmentSpecifications env;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClearViews()
    {
        views.Clear();
    }

    public List<SpatialView> GetViews()
    {
        return views;
    }

    public void AppendView(SpatialViewType type)//*******
    {

        switch (type)
        {
            case SpatialViewType.SVT_SAMPLE:
                {
                    AppendSample(randSeed, nElements, maxRadius);
                    break;
                }
            case SpatialViewType.SVT_TARGET:
                {
                    AppendTarget(views[0]);
                    break;
                }
            case SpatialViewType.SVT_CONFIGURAL:
                {
                    AppendConfiguralFoil(views[0], configNToSwap); //note this assumes the sample image is (already defined as) views[0];
                    break;
                }
            case SpatialViewType.SVT_SPATIAL:
                {
                    AppendSpatialFoil(views[0], spatialDistortionLevel, spatialProtectElement); //note this assumes the sample image is (already defined as) views[0];
                    break;
                }
            case SpatialViewType.SVT_ELEMENTAL:
                {
                    AppendElementalFoil(views[0], elementalNToChange); //note this assumes the sample image is (already defined as) views[0];
                    break;
                }
        }
    }

    void AppendSample(uint seed, int nelements = 4, float maxradius = 200.0f)
    {
        SpatialView temp = GenerateSample(seed, nelements, maxradius);
        SpatialView newview = new SpatialView(temp);


        views.Add(newview);

    }

    void AppendTarget(uint seed, int nelements = 4, float maxradius = 200.0f)
    {
        SpatialView newview = new SpatialView();
        newview = GenerateSample(seed, nelements, maxradius);
        views.Add(newview);
    }

    void AppendTarget(SpatialView sampleview)
    {
        SpatialView newview = new SpatialView();
        newview = GenerateTarget(sampleview);
        views.Add(newview);
    }

    void AppendConfiguralFoil(SpatialView targetview, int ntoswap = 2)
    {
        SpatialView newview = new SpatialView();
        newview = GenerateConfiguralFoil(targetview, ntoswap);
        views.Add(newview);
    }

    void AppendSpatialFoilOld(SpatialView targetview, float maxabsradius = 0.0f, float minabsradius = 0.0f, float maxdeltaangle = 0.0f, float maxdeltaradius = 0.0f, float mindeltaangle = 0.0f, float mindeltaradius = 0.0f, int protectelement = -1)
    {
        //Deprecated
        SpatialView newview = new SpatialView();
        newview = GenerateSpatialFoilOld(targetview, maxabsradius,minabsradius,maxdeltaangle,maxdeltaradius,mindeltaangle,mindeltaradius,protectelement);
        views.Add(newview);
    }

    void AppendSpatialFoil(SpatialView targetview, float distortion_level, int protectelement = -1)
    {
        //Deprecated
        SpatialView newview = new SpatialView();
        newview = GenerateSpatialFoil(targetview, distortion_level, protectelement);   ///problem here
        views.Add(newview);
    }

    void AppendElementalFoil(SpatialView targetview, int ntochange = 1)
    {
        SpatialView newview = new SpatialView();
        GenerateElementalFoil(targetview, ntochange);
        views.Add(newview);
    }

    SpatialView GenerateSample(uint seed, int nelements = 4, float maxradius = 200.0f)
    {
        SpatialView outview = new SpatialView();
        outview.type = SpatialViewType.SVT_SAMPLE;
        randomizer.SetRandomizer(randSeed);
        outview.noise_seed = (int)randomizer.GetNoiseSeed();
        arraygenerator.seed = randomizer.GetArrayGeneratorSeed();
        List<SpatialElement> temp = new List<SpatialElement>();

        arraygenerator.nelements = nelements;

        //temp=arraygenerator.GenerateSampleArray(sampleMaxAbsRadius,sampleMinAbsRadius,sampleMaxDeltaAngle,sampleMaxDeltaRadius,sampleMinDeltaAngle,sampleMinDeltaRadius,sampleProtectElement); //edit here for input of environemntal specs module
        Debug.Log("environemntparams= " + env.maxAbsRadius + "  " +  env.minAbsRadius + "  " + env.maxDeltaAngle + "  " + env.maxDeltaRadius + "  " + env.minDeltaAngle + "  " + env.minDeltaRadius + "  " + sampleProtectElement);
        temp = arraygenerator.GenerateSampleArray(env.maxEnvironmentBoundsRadius, env.maxAbsRadius, env.minAbsRadius, env.maxDeltaAngle, env.maxDeltaRadius, env.minDeltaAngle, env.minDeltaRadius, sampleProtectElement = -1, true, env._scaleMultiplier); //edit here for input of environemntal specs module
        foreach (SpatialElement s in temp)
        {
            outview.scene.features.Add(s);
        }

        outview.conditions = randomizer.GetRandomIndex(randomizer.GetConditionsRandomizerSeed(),maxconditions); //note the zero offset argument; the conditions selection will the raw conditions randomizer seed with no offset
        outview.viewpoint = 0; //index 0 is the original position in front of the array
        randomizer.GetRandomIndex(randomizer.GetViewpointRandomizerSeed(),maxviews); //note the zero offset argument; the view selection will use the raw viewpoint randomizer seed with no offset
        Debug.Log("gen sample");
        return outview;
    }

    /* Generally we should make our target based on the sample view, rather than relying on the randomizer to reproduce its topology
     * This function needs to be fixed if it is to be used in that way
     *
     * SpatialView GenerateTarget(uint seed, int nelements = 4, float maxradius = 200.0f)
    {
        SpatialView outview = new SpatialView();
        outview.type = SpatialViewType.SVT_TARGET;
        randomizer.SetRandomizer(randSeed);
        outview.noise_seed = (int)randomizer.GetNoiseSeed();
        arraygenerator.seed = (int)randomizer.GetArrayGeneratorSeed();
        List<SpatialElement> temp = new List<SpatialElement>();
        temp = arraygenerator.GenerateSampleArray(sampleMaxAbsRadius, sampleMinAbsRadius, sampleMaxDeltaAngle, sampleMaxDeltaRadius, sampleMinDeltaAngle, sampleMinDeltaRadius, sampleProtectElement);
        foreach (SpatialElement s in temp)
        {
            outview.scene.features.Add(s);
        }
        Random.InitState((int)randomizer.GetConditionsRandomizerSeed());
        outview.conditions = Random.Range(0, maxconditions);
        outview.conditions = RandomIntRangeWithSkip(0, maxconditions, outview.conditions); //skip the original conditions and viewpoint - different from sample
        Random.InitState((int)randomizer.GetViewpointRandomizerSeed());
        outview.viewpoint = Random.Range(0, maxviews);
        outview.viewpoint = RandomIntRangeWithSkip(0, maxviews, outview.viewpoint);

        return outview;
    } */

    SpatialView GenerateTarget(SpatialView sampleview)
    {
        SpatialView outview = new SpatialView();
        outview.type = SpatialViewType.SVT_TARGET;
        randomizer.SetRandomizer(randSeed);
        outview.noise_seed = (int)randomizer.GetNoiseSeed(); //note no offset is provided, because the target topography should be identical to that used in the sampleview

        arraygenerator.seed = randomizer.GetArrayGeneratorSeed();
        List<SpatialElement> temp = new List<SpatialElement>();
        //temp = arraygenerator.GenerateSampleArray(sampleMaxAbsRadius, sampleMinAbsRadius, sampleMaxDeltaAngle, sampleMaxDeltaRadius, sampleMinDeltaAngle, sampleMinDeltaRadius, sampleProtectElement);
        temp = arraygenerator.GenerateSampleArray(env.maxEnvironmentBoundsRadius ,env.maxAbsRadius, env.minAbsRadius, env.maxDeltaAngle, env.maxDeltaRadius, env.minDeltaAngle, env.minDeltaRadius, sampleProtectElement, true, env._scaleMultiplier);
        foreach (SpatialElement s in temp)
        {
            outview.scene.features.Add(s);
        }

        outview.conditions = randomizer.GetRandomIndexWithSkip(randomizer.GetConditionsRandomizerSeed((uint)SpatialViewType.SVT_TARGET), maxconditions,sampleview.viewpoint); //note the offset argument is provided and is non-zero; the conditions selection will use a different seed from the sample
        outview.viewpoint = randomizer.GetRandomIndexWithSkip(randomizer.GetViewpointRandomizerSeed((uint)SpatialViewType.SVT_TARGET), maxviews, sampleview.viewpoint); //note the offset argument is provided and is non-zero; the view selection will use a different seed from the sample
        Debug.Log("gen target");
        return outview;
    }

    SpatialView GenerateConfiguralFoil(SpatialView targetview, int ntoswap = 2)
    {
        SpatialView outview = new SpatialView();
        int noise_seed = targetview.noise_seed;
        int noise_seed_offset = (int)SpatialViewType.SVT_CONFIGURAL; //guaranteed to be different from other types
        Debug.Log("gen configural foil");
        return outview;
    }

    SpatialView GenerateSpatialFoilOld(SpatialView targetview, float maxabsradius = 0.0f, float minabsradius = 0.0f, float maxdeltaangle = 0.0f, float maxdeltaradius = 0.0f, float mindeltaangle = 0.0f, float mindeltaradius = 0.0f, int protectelement = -1)
    {
        //DEPRECATED
        SpatialView outview = new SpatialView();
        outview.type = SpatialViewType.SVT_SPATIAL;

        randomizer.SetRandomizer(randSeed);
        outview.noise_seed = (int)randomizer.GetNoiseSeed((int)SpatialViewType.SVT_SPATIAL); //note the offset argument is provided and is non-zero; the noise and array generation will use a different seed from the sample/target
        arraygenerator.seed = randomizer.GetArrayGeneratorSeed((int)SpatialViewType.SVT_SPATIAL);

        List<SpatialElement> temp = new List<SpatialElement>();
        temp = arraygenerator.GenerateSpatialFoilArrayOld(targetview.scene.features, spatialMaxAbsRadius, spatialMinAbsRadius, spatialMaxDeltaAngle, spatialMaxDeltaRadius, spatialMinDeltaAngle, spatialMinDeltaRadius, spatialProtectElement);
        foreach (SpatialElement s in temp)
        {
            outview.scene.features.Add(s);
        }

        outview.conditions = randomizer.GetRandomIndexWithSkip(randomizer.GetConditionsRandomizerSeed((uint)SpatialViewType.SVT_SPATIAL), maxconditions, targetview.conditions); //note the offset argument is provided and is non-zero; the conditions selection will use a different seed from the sample/target
        outview.viewpoint = randomizer.GetRandomIndexWithSkip(randomizer.GetViewpointRandomizerSeed((uint)SpatialViewType.SVT_SPATIAL), maxviews, targetview.viewpoint); //note the offset argument is provided and is non-zero; the view selection will use a different seed from the sample/target
        Debug.Log("gen spatial foilold");
        return outview;
    }

    SpatialView GenerateSpatialFoil(SpatialView targetview, float distortion_level, int protectelement = -1)
    {
        SpatialView outview = new SpatialView();
        outview.type = SpatialViewType.SVT_SPATIAL;

        randomizer.SetRandomizer(randSeed);
        outview.noise_seed = (int)randomizer.GetNoiseSeed((int)SpatialViewType.SVT_SPATIAL); //note the offset argument is provided and is non-zero; the noise and array generation will use a different seed from the sample/target
        arraygenerator.seed = randomizer.GetArrayGeneratorSeed((int)SpatialViewType.SVT_SPATIAL);

        List<SpatialElement> temp = new List<SpatialElement>();

        temp = arraygenerator.GenerateSpatialFoilArray(targetview.scene.features, distortion_level, spatialProtectElement);   //////////////////////////////////////////problem here
        foreach (SpatialElement s in temp)
        {
            outview.scene.features.Add(s);
        }

        outview.conditions = randomizer.GetRandomIndexWithSkip(randomizer.GetConditionsRandomizerSeed((uint)SpatialViewType.SVT_SPATIAL), maxconditions, targetview.conditions); //note the offset argument is provided and is non-zero; the conditions selection will use a different seed from the sample/target
        outview.viewpoint = randomizer.GetRandomIndexWithSkip(randomizer.GetViewpointRandomizerSeed((uint)SpatialViewType.SVT_SPATIAL), maxviews, targetview.viewpoint); //note the offset argument is provided and is non-zero; the view selection will use a different seed from the sample/target
        Debug.Log("gen spatial foil");
        return outview;
    }

    SpatialView GenerateElementalFoil(SpatialView targetview, int ntochange = 1)
    {
        SpatialView outview = new SpatialView();
        int noise_seed = targetview.noise_seed;
        int noise_seed_offset = (int)SpatialViewType.SVT_ELEMENTAL; //guaranteed to be different from other types
        Debug.Log("gen elemental foil");
        return outview;
    }

    public void RenderSpatialScene(int viewindex)
    {
        arraygenerator.UpdateArray(views[viewindex].scene); //Note the arraygenerator should provide a way to render its array (e.g., topography) based on this index
    }

    public void RenderConditions(int viewindex)
    {
        //conditionscontroller.SetConditions(views[viewindex].conditions); //Note the conditions controller should provide a way to render its conditions based on this index
    }

    public void SetViewpoint(int viewindex, float envScaleMod)
    {

        viewcontroller.currentview = views[viewindex].viewpoint;   // views[0] is sample
        //Debug.Log(views[viewindex].viewpoint + " set vp vaI.vp");
        viewcontroller.MoveView(envScaleMod);
    }

    public void RenderView(int viewindex, int viewAngleIndex = -1, float envScaleMod = 1)
    {
        if (viewAngleIndex != -1)
        {
            views[viewindex].viewpoint = viewAngleIndex; // only used in VCE version - when viewpoint is dictated by the experiment controller - depending on active staircases. (each vp has a staircase)
           // Debug.Log(viewAngleIndex + " render view vaI");
        }
        RenderSpatialScene(viewindex);
        RenderConditions(viewindex);
        SetViewpoint(viewindex, envScaleMod);
    }
}

[System.Serializable]
public class SpatialScene
{
    public List<SpatialElement> features = new List<SpatialElement>(); //spatial elements have a centre, radius and height, but other properties are added by the derived class
}

[System.Serializable]
public class SpatialConditions
{
    //Use for any abstract properties associated with Spatial Conditions, otherwise extended by specific rendering class.
}

[System.Serializable]
public class SpatialView
{
    //the members of this class provide all the information needed by SpatialArrayGeneratorClass to generate (or regenerate) a view from scratch
    //these are to be populated by the SpatialViewGenerator class above

    public SpatialViewType type;
    public int viewpoint; //index into a list provided by viewcontroller that specifies location of the camera, world coordinates
    public int conditions; //index into a list provided by conditions controller that specifies e.g., lighting, vegetation, clouds,time of day, year etc.
    public int noise_seed; // a given combination of seed and seed offset will always produce the same noise/random numbers used to produce scene, conditions, etc.
    public int noise_seed_offset; //seed_offset is used in combination with seed to produce distinct, reproducible noise for any comparison views
    public SpatialScene scene = new SpatialScene();


    public SpatialView()
    {

    }

    public SpatialView(SpatialView viewtocopy)
    {
        type = viewtocopy.type;
        viewpoint = viewtocopy.viewpoint;
        noise_seed = viewtocopy.noise_seed;
        noise_seed_offset = viewtocopy.noise_seed_offset;

        foreach (SpatialElement f in viewtocopy.scene.features)
        {
            scene.features.Add(f);
        }
        conditions = viewtocopy.conditions;
    }
}
