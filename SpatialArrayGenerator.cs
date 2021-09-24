using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialArrayGenerator : MonoBehaviour
{

    // The Spatial Array Generator class
    // provides methods that can be used to creates an array of distinct (and/or interchangeable) spatial elements
    // whose locations are described in polar coordinates and whose identities are described as an ordered list of integers

    // This is an abstract class, and the idea is that it can be extended to work with different physical objects/heightmaps represented by
    // objects that extend the SpatialElement class.

    // So, for example, you could create a TopoArrayGenerator that extends the SpatialArrayGenerator class and
    // pair this with a TopoElement class that extends the SpatialElement class below, to add specifically
    // topographical properties for the array as a whole (e.g., noise, background hills) and for each element
    // (e.g., profile of specifc hills or valleys to be combined in the array).


    public int nelements = 4;
    public int indextomove = 0;
    public int ndistinctelements = 10;
    public float envScale = 1f;
    public bool alldistinctelements = true;
    Vector2 arraycentre; //this should use the appropriate units
    //public float maxradius = 200.0f; //the edge of the object/topographical feature (specified by its radius) should not go beyond this
    public uint seed = 0; //set this to produce a reproducible array

    public uint nonce = 0;
    public uint objseed = 0;

    PRandStream internal_rng = new PRandStream();
    //PRandStream object_rng = new PRandStream();

    float maxradius;
    float maxEnvBndRadius;
    public virtual SpatialElement DistinctElement(int i)
    {
        //This virtual function will normally be implemented by the derived class which handles a lost of distinct elements
        //potentially including render specific information as well as the basic spatial array info.

        Debug.Log("DistinctElement() called via SpatialArrayGenerator, should be implemented by derived class");
        return null;
    }

    public List<SpatialElement> GenerateSampleArray(float maxEnvBoundsRadius = 0.0f, float maxabsradius = 0.0f, float minabsradius = 0.0f, float maxdeltaangle = 0.0f, float maxdeltaradius = 0.0f, float mindeltaangle = 0.0f, float mindeltaradius = 0.0f, int protectelement = -1, bool rotateArray = true, float scale =1)
    {

        bool check1 = true;
        List<int> order;
        List<Polar2> polarcoords;
        List<SpatialElement> arrayspec = new List<SpatialElement>();
        envScale = scale;
        protectelement = -1;
        while (check1)
        {
            order = DistinctOrder(); //problem HERE
            Debug.Log("order is " + order[0]);
            polarcoords = EvenRingArray(maxabsradius/2);
            if (rotateArray)
            {
                polarcoords = RotateArray(polarcoords);
            }

            polarcoords = PeturbRingArray(polarcoords, maxabsradius, minabsradius, maxdeltaangle, maxdeltaradius, mindeltaangle,  mindeltaradius, protectelement);
            arrayspec.Clear();

            for (int i = 0; i < nelements; i++)
            {
                SpatialElement s = DistinctElement(order[i]);
                //Debug.Log(s.height);
                s.polarCoords = polarcoords[i];
                s.centre = polarcoords[i].ToCartesian();
                //Debug.Log("Generating Sample Array: " + s.index + "  " + s.centre.x + "," + s.centre.y);
                arrayspec.Add(s);
            }
            check1 = CheckCollisions(arrayspec);
            if (check1)
            {
                nonce++;
                Debug.Log("Collsion, looping " + nonce);
            }
        }
        nonce = 0;
        maxradius = maxabsradius;
        maxEnvBndRadius = maxEnvBoundsRadius;
        return arrayspec;
    }

    public List<SpatialElement> GenerateSpatialFoilArrayOld(List<SpatialElement>samplearrayspec, float maxabsradius = 0.0f, float minabsradius = 0.0f, float maxdeltaangle = 0.0f, float maxdeltaradius = 0.0f, float mindeltaangle = 0.0f, float mindeltaradius = 0.0f, int protectelement = -1)
    {
        bool check1 = true;

        List<SpatialElement> arrayspec = new List<SpatialElement>();
        foreach (SpatialElement s in samplearrayspec)
        {
            arrayspec.Add(new SpatialElement());
            arrayspec[arrayspec.Count-1].centre = s.centre;
            arrayspec[arrayspec.Count-1].height = s.height;
            arrayspec[arrayspec.Count-1].radius = s.radius;
            arrayspec[arrayspec.Count-1].index = s.index;
        }

        List<int> candidatestomove =new List<int>();
        for (int i=0;i<arrayspec.Count;i++)
        {
            if (i!=protectelement)
            {
                candidatestomove.Add(i);
            }
        }

        //int indextomove = candidatestomove[Random.Range(0, candidatestomove.Count - 1)];
        int indextomove = candidatestomove[internal_rng.Range(0, candidatestomove.Count)];


        while (check1)
        {
            Polar2 pcoords = new Polar2(samplearrayspec[indextomove].centre);

            //float deltaangle = Random.Range(mindeltaangle, maxdeltaangle);
            float deltaangle = internal_rng.Range(mindeltaangle, maxdeltaangle);
            //float deltaradius = Random.Range(mindeltaradius, maxdeltaradius);
            float deltaradius = internal_rng.Range(mindeltaradius, maxdeltaradius);
            //float sgndeltaangle = (Random.value < 0.5f ? 1 : -1) * 2.0f - 1.0f;
            float sgndeltaangle = (internal_rng.Value() < 0.5f ? 1 : -1) * 2.0f - 1.0f;
            //float sgndeltaradius = (Random.value < 0.5f ? 1 : -1) * 2.0f - 1.0f;
            float sgndeltaradius = (internal_rng.Value() < 0.5f ? 1 : -1) * 2.0f - 1.0f;
            Debug.Log(pcoords.th + " " + pcoords.r);
            pcoords.th = pcoords.th + deltaangle * sgndeltaangle;
            pcoords.r = pcoords.r + deltaradius * sgndeltaradius;
            arrayspec[indextomove].centre = pcoords.ToCartesian();
            Debug.Log(pcoords.th + " " + pcoords.r);
            Debug.Log("Change: " + deltaangle + " " + deltaradius);
            Debug.Log("Attempting to moving mountain for Spatial Foil " + indextomove);
            Debug.Log("old: " + samplearrayspec[indextomove].centre);
            Debug.Log("new: " + arrayspec[indextomove].centre);

            check1 = CheckCollisions(arrayspec);
            if (check1)
            {
                nonce++;
                Debug.Log("Collsion, looping " + nonce);
            }
        }
        nonce = 0;

        return arrayspec;
    }

    public List<SpatialElement> GenerateSpatialFoilArray(List<SpatialElement> samplearrayspec, float distortion_radius, int protectelement = -1)
    {
        //this alternative method of generating spatial foils ensures that one element is moved by distortion_radius (i.e., to a point on a circle around its original location), the point on the circle (angle) is random

        bool check1 = true;    //problem is here
        bool collisioncheck = true;
        bool outsidecheck = true;
        List<SpatialElement> arrayspec = new List<SpatialElement>();
        foreach (SpatialElement s in samplearrayspec)
        {
            arrayspec.Add(new SpatialElement());
            arrayspec[arrayspec.Count - 1].centre = s.centre;
            arrayspec[arrayspec.Count - 1].height = s.height;
            arrayspec[arrayspec.Count - 1].radius = s.radius;
            arrayspec[arrayspec.Count - 1].index = s.index;
        }

        List<int> candidatestomove = new List<int>();
        for (int i = 0; i < arrayspec.Count; i++)
        {
            if (i != protectelement)
            {
                candidatestomove.Add(i);
            }
        }

        //int indextomove = candidatestomove[Random.Range(0, candidatestomove.Count - 1)];

        objseed++;

        //object_rng.SetSeed(objseed);


        //indextomove = candidatestomove[object_rng.Range(0, candidatestomove.Count + 1)]; //changed this to + 1 as it did not include object 3
        indextomove = candidatestomove[internal_rng.Range(0, candidatestomove.Count) ];
        //Debug.LogError(objseed + " " + indextomove + " " + candidatestomove.Count);
        while (check1)
        {
            //check1 = false;
            if(distortion_radius > maxEnvBndRadius)
            {
                Debug.Log("DISTORTION PASSED TO SAG IS LARGER THAN THE POSSIBLE SPAWN AREA. Dist Radius: " + distortion_radius + " Max env Radius: " + maxEnvBndRadius);
                //break;
            }
            if (distortion_radius > maxradius)
            {
                Debug.Log("WARNING DISTORTION BIGGER THAN MAX RADIUS: " + distortion_radius + " Max Radius: " + maxradius);

            }

            Polar2 pcoords = new Polar2(samplearrayspec[indextomove].centre);

            float angle_on_circle = internal_rng.Value() * 2.0f *(Mathf.PI);
            Vector2 current_loc_cartesian = pcoords.ToCartesian();
            float deltax = Mathf.Cos(angle_on_circle) * distortion_radius;
            float deltay = Mathf.Sin(angle_on_circle) * distortion_radius;
            Vector2 new_loc_cartesian = new Vector2(current_loc_cartesian.x + deltax, current_loc_cartesian.y + deltay);


            pcoords = new Polar2(new_loc_cartesian);  //new coordinates including distortion

            //TEMP THING HERE TO MAKE  OBJECTS ALWAYS GO TO CENTRE FIRST
            arrayspec[indextomove].centre = pcoords.ToCartesian();

            collisioncheck = CheckCollisions(arrayspec);         //returns true if colliding
            outsidecheck = CheckOutside(pcoords);                 //returns true if outside

            if (!collisioncheck && !outsidecheck)
            {
                check1 = false;
            }

            if (check1)
            {
                nonce++;
                Debug.Log("Collsion, looping " + nonce); //does this reset locations to original positions?
            }

            //check1 = false;
        }
        nonce = 0;
        return arrayspec;
    }

    public bool CheckCollisions(List<SpatialElement> arrayspec)
    {
        bool check = false;

        for (int i=0; check == false && i<(arrayspec.Count-1); i++)
        {
            for (int j=i+1; check==false && j<arrayspec.Count; j++)
            {
                float qx = arrayspec[i].centre.x - arrayspec[j].centre.x;
                float qy = arrayspec[i].centre.y - arrayspec[j].centre.y;
                float d = Mathf.Sqrt(qx * qx + qy * qy);
                //Debug.Log((arrayspec[i].radius * envScale)+ "radiusssss");
                if ((d<(arrayspec[i].radius * envScale))||(d<(arrayspec[j].radius*envScale)))
                {
                    Debug.Log("Collision Detected");
                    check = true;
                    break;
                }
            }
        }
        return check;
    }

    public bool CheckOutside(Polar2 pcoords)
    {
        bool check = false;
        //Debug.Log(pcoords.r + maxradius + "CHECKOUTSIDE");

        if (pcoords.r > maxradius)// maxEnvBndRadius)// maxradius)
        {
            check = true;
            Debug.Log("Error: Outside Arena");
        }
        else
        {
            check = false;
        }
        return check;
    }



    public List<int> DistinctOrder()
    {

        //uses the specified number of elements (for the current array) and of possible distinct elements to choose from
        //to generate a list of integers, an possible ordering of elements that obeys similar constraints to those used in the
        //Four Mountains Test - the first element is always zero, and the remaining elements are selected to ensure that at least
        //two distinctive elements are added in random order with additional random elements being added at random positions

        //Random.InitState(seed+nonce); //set random seed
        internal_rng.SetSeed(seed + nonce); //set random seed

        List<int> outlist = new List<int>();
        //first element will always be 0
        Debug.Log("starting with zero" + 0);
        //outlist.Add(0);
        //reserve an odd-one-out and its position from [1] to [n-1];
        //int oooindex = Random.Range(1, ndistinctelements - 1);
        int oooindex = internal_rng.Range(1, ndistinctelements - 1);
        //int ooopos = Random.Range(1, nelements);
        int ooopos = internal_rng.Range(1, nelements);
        int currpos = 0;
        //int currpos = 1; //can be uncommented when adding reserve item is needed
        bool freeflag = false;
        while (currpos <= nelements)
        {
            if (currpos == ooopos)
            {
                //Debug.Log("adding reserved odd one out: " + oooindex); // not needed for current table(?)
                //outlist.Add(oooindex); - this
            }
            else
            {
                //if you want all the array items to be distinct (controlled by public bool on this script)
                if (alldistinctelements)
                {
                    if (outlist.Count < ndistinctelements)
                    {
                        int nextindex = internal_rng.RangeWithListSkip(1, ndistinctelements, outlist); // REPLACE BJECT_RNG WITH INTERNAL RNG TO GET BACK TO WHERE IT WAS
                        Debug.Log("choosing distinct elements " + nextindex);
                        outlist.Add(nextindex);
                    }
                    else
                    {
                        Debug.Log("you do not have enough distinct objects");
                        int nextindex = internal_rng.Range(1, ndistinctelements);
                        outlist.Add(nextindex);

                    }


                }
                else if (!freeflag)
                {
                    //choose a random number between 1and ndistinctelements avoiding oooindex
                    //int nextindex = Random.Range(1, ndistinctelements - 1);
                    int nextindex = internal_rng.RangeWithSkip(1, ndistinctelements, oooindex);
                    Debug.Log("avoiding odd one out " + nextindex);
                    outlist.Add(nextindex);
                    freeflag = true;
                }

                //choose freely from the other elements at random
                else
                {
                    //int nextindex = Random.Range(1, ndistinctelements);
                    int nextindex = internal_rng.Range(1, ndistinctelements);
                    Debug.Log("choosing freely " + nextindex);
                    outlist.Add(nextindex);
                }
            }
            currpos++;
        }

        for (int i = 0; i < outlist.Count; i++)
        {
            //Debug.Log(outlist[i]);
        }
        return outlist;
    }

    public List<int> DistinctReorder(List<int> initial)
    {

        //Random.InitState(seed+nonce); //set random seed
        internal_rng.SetSeed(seed + nonce); //set random seed

        //reorder the provided list, so that it is distinct (i.e., swap elements that are different from one another)

        List<int> outlist = new List<int>(initial);
        //int postoswitch = Random.Range(1, nelements);
        int postoswitch = internal_rng.Range(1, nelements);
        List<int> candidatedestination = new List<int>();
        for (int i = 1; i < initial.Count; i++)
        {
            if (i != postoswitch && initial[i] != initial[postoswitch])
            {
                candidatedestination.Add(i);
            }
        }
        //int candidatetopick = Random.Range(0, candidatedestination.Count);
        int candidatetopick = internal_rng.Range(0, candidatedestination.Count);
        //int temp = initial[candidatedestination[candidatetopick]];
        outlist[candidatedestination[candidatetopick]] = initial[postoswitch];
        outlist[postoswitch] = initial[candidatedestination[candidatetopick]];

        Debug.Log("Reordering...");
        for (int i = 0; i < outlist.Count; i++)
        {
            Debug.Log(outlist[i]);
        }
        return initial;
    }

    public List<Polar2> EvenRingArray(float ringradius)
    {

        //generate polar coordinates on a regular polygon with the specified radius and nelements vertices

        List<Polar2> outlist = new List<Polar2>();
        for (int i=0;i<nelements;i++)
        {
            Polar2 newpoint = new Polar2(((float)i / nelements * 2.0f * Mathf.PI + Mathf.PI / 2.0f) % (Mathf.PI * 2.0f),ringradius);
            //Debug.Log(newpoint.th + " " + newpoint.r);
            outlist.Add(newpoint);
        }
        return outlist;
    }

    public List<Polar2> RotateArray(List<Polar2> original)
    {
        float rotationAmount = internal_rng.Range(-1*((2.0f * Mathf.PI) / (2 * nelements)), ((2.0f * Mathf.PI) / (2 * nelements))); //rotation amount is plus or minus a random amount based on the number of objects in the array.
        List<Polar2> outlist = original;
        for (int i = 0; i < nelements; i++)
        {
            outlist[i].th += rotationAmount;
        }
            return outlist;
    }

    public List<Polar2> PeturbRingArray(List<Polar2> original, float maxabsradius = 0.0f, float minabsradius = 0.0f, float maxdeltaangle = 0.0f, float maxdeltaradius = 0.0f, float mindeltaangle = 0.0f, float mindeltaradius = 0.0f, int protectelement = -1)
    {

        //Random.InitState(seed+nonce); //set random seed
        internal_rng.SetSeed(seed + nonce); //set random seed
        //take a provided list of polar coordinates and peturb them while maintaining specified constraints
        //internal_rng.SetSeed(
        List <Polar2> outlist = new List<Polar2>(original);

        if ((maxdeltaangle == 0.0f) && (maxdeltaradius == 0.0f) && (mindeltaangle == 0.0f) && (mindeltaradius == 0.0f))
        {
            return outlist;
        }
        else
        {
            for (int i = 0; i < outlist.Count; i++)
            {
                if (i != protectelement)
                {
                    //float deltaangle = Random.Range(mindeltaangle, maxdeltaangle);
                    float deltaangle = internal_rng.Range(mindeltaangle, maxdeltaangle);
                    //Debug.Log("mindelthet= " + mindeltaangle + "maxdelthet= " + maxdeltaangle);
                    //Debug.Log(deltaangle);
                    //float deltaradius = Random.Range(mindeltaradius, maxdeltaradius);
                    float deltaradius = internal_rng.Range(mindeltaradius, maxdeltaradius); //
                    //Debug.Log("delata angle" + deltaangle + "delta radius" + deltaradius);
                    //Debug.Log(internal_rng.seed);
                    //float anglesign = (float)Random.Range((int)0, (int)2) * 2.0f - 1.0f;
                    float anglesign = (float)internal_rng.Range((int)0, (int)2) * 2.0f - 1.0f; //  + or - ? possible output from this is -1 , 1, 3

                    //float radiussign = (float)Random.Range((int)0, (int)2) * 2.0f - 1.0f;
                    float radiussign = (float)internal_rng.Range((int)0, (int)2) * 2.0f - 1.0f;
                    //Debug.Log("Peturbing " + i + ": th " + (deltaangle * anglesign) % (2.0f * Mathf.PI) + " r " + (deltaradius * radiussign));
                    outlist[i].th = (outlist[i].th + deltaangle * anglesign) % (2.0f * Mathf.PI);
                    outlist[i].r = outlist[i].r + deltaradius * radiussign;

                    if (outlist[i].r > maxabsradius)
                    {
                        Debug.Log("greater than maxabsradius, correcting");
                        Debug.Log(deltaradius);
                        Debug.Log(outlist[i].r);
                        Debug.Log(maxabsradius);
                        outlist[i].r = maxabsradius;
                    }
                    if (outlist[i].r < minabsradius)
                    {

                        Debug.Log("less than minabsradius, correcting");
                        Debug.Log(outlist[i].r);
                        Debug.Log(minabsradius);
                        outlist[i].r = minabsradius;
                    }
                }
            }
        }

        return outlist;
    }

    public virtual void UpdateArray(SpatialScene s)
    {
        //this is normally overridden by the specific rendering generator (e.g., TopoArrayGenerator)
    }
}


[System.Serializable]
public class SpatialElement
{
    //abstract elements to arrange in an array as well as polar coordinates they have a radius and height

    public int index = 0; //this is used to signify distinct elements (e.g., different shapes, objects etc. have different indices).
    public Vector2 centre;
    public float radius = 1.0f;
    public float height = 1.0f;
    public Polar2 polarCoords;
}

public class Polar2
{
    //2D polar coordinates

    public float th;
    public float r;

    public Polar2(float theta, float radius)
    {
        th = theta;
        r = radius;
    }

    public Polar2(Vector2 point)
    {
        th = Mathf.Atan2(point.y, point.x);
        r = point.magnitude;
    }

    public Vector2 ToCartesian()
    {
        return new Vector2(Mathf.Cos(th) * r, Mathf.Sin(th) * r);
    }
}
