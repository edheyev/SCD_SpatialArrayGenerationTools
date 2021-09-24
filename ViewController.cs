using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour {

    //The ViewController class provides a list of possible viewpoints from which scenes/views will be rendered
    //It also provides methods that check the visibility/occlusion of elements in the scene from different viewpoints

    //public List<Viewpoint> viewpoints = new List<Viewpoint>();
    //public float viewpointRadius=212.0f;

    public EnvironmentSpecifications env;
    public SCDExperimentController exCo;
    public float viewpointElevation = 0f;
    public float viewpointRange = Mathf.PI / 3.0f; //default to 150 degrees
    public int nViewpoints;
    public Viewpoint[,] TestViewpoints;
    //public List<float> anglesOfRotation;
    public GameObject teleportOrigin;
    public int currentview = 0;
    float direction;
    public bool VREnabled = false;
    GameObject vrCam;
    int vp = 0;
    float randomRotation = 0;
    float distAway = 1.6f;
    Vector3 startPos;
    Vector3 center = new Vector3(0, 0, 0);

    float distFromCenter;

    PRandStream internal_rng = new PRandStream();
    //[Range(0, 180)]
    //public int observationArc;
    // Use this for initialization
    private void Awake()
    {

        exCo = GameObject.Find("ExperimentController").GetComponent<SCDExperimentController>();
        currentview = Mathf.FloorToInt((nViewpoints - 1) / 2.0f); //centres currentview at the start
        teleportOrigin = GameObject.Find("TeleportOriginDesktop");
        vrCam = GameObject.Find("VRCamera");
        startPos = new Vector3(0, 0, env.environmentRadius + distAway);
    }

    public void InitialiseViewpoints(float [] viewpointsToTestArray)
    {
        distFromCenter = env.environmentRadius * distAway;
        nViewpoints = viewpointsToTestArray.Length;
        TestViewpoints = new Viewpoint[nViewpoints, 2];
        currentview = Mathf.FloorToInt((nViewpoints - 1) / 2.0f); //centres currentview at the start
        for (int i = 0; i < nViewpoints; i++) //this loop will calculate view angles and add viewpoints into multi-d array where each viewpoint can be stored (+ and -). it can be referenced as follows.
                                              //TestViewpoints is a list of Viewpoint. Viewpoint datatype (defined below) contains a vector 2 denoting x and y coordinate. TestViewpoints[0,0] is vp1 TestViewpoints[0,1] is the mirror of this. TestViewpoints[1,0] is the first selected vp, TestViewpoints[1,1] is the mirror of this. etc for length nViewpoints
        {
            //calculate plus and minus angle
            for (int j = 0; j <= 1; j++)
            {
                if (j == 0) //calculate clockwise rotation and store in array
                {
                    Viewpoint v = new Viewpoint();
                    float thisAngleRads = Mathf.Deg2Rad * viewpointsToTestArray[i];
                    v.location.y = -1 * (Mathf.Cos(thisAngleRads) * (env.environmentRadius + distAway));//TRIG to work out x and y. 3f is the distance from the array. this should be changed for different array sizes
                    v.location.x = Mathf.Sin(thisAngleRads) * (env.environmentRadius + distAway);
                    v.angle = viewpointsToTestArray[i];
                    //Debug.Log("vp initialise: " + v.angle);


                    TestViewpoints[i, j] = v;
                }
                else if (j == 1) //calculate anticlockwise rotation and store in array
                {
                    Viewpoint v = new Viewpoint();
                    v.location.y = TestViewpoints[i, 0].location.y;// second array item is reflection of first
                    v.location.x = TestViewpoints[i, 0].location.x * -1;
                    v.angle = viewpointsToTestArray[i];
                    TestViewpoints[i, j] = v;
                }

            }
        }
        //debug
        /*
        for (int i = 0; i <= nViewpoints - 1; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //Debug.Log(TestViewpoints.GetLength(0) + " " + TestViewpoints.GetLength(1));
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(TestViewpoints[i, j].location.x, 1.5F, TestViewpoints[i, j].location.y);
            }
        }
        */
        currentview = 0;
        MoveView(1);
    }


    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            //RotateViewpoints();
            for(int i = 0; i<=1; i++)
            {
                Debug.Log(TestViewpoints[i, 0].angle);
            }

        }
	}

    public Viewpoint[,] RotateViewpoints() // DEPRECIATED - now included in the scale viewpoints function. (in a much simpler way)
    {
        Debug.Log("ROTATING" + vp);
        //call this after Testviewpoints list is populated to add random rotation to 0 degree position in the array.

        //float rotationAmount = internal_rng.Range((-1 *  Mathf.PI), (Mathf.PI)); //rotation amount is plus or minus a random amount based on the number of objects in the array.
        Viewpoint[,] outlist = TestViewpoints;
        float rotationAmount = 0;// Mathf.PI/2;
        //Debug.Log(TestViewpoints.Length);

        for (int i = 0; i < 2; i++)
        {
            //convert
            Debug.Log(TestViewpoints[i, 1].location.x + "1");
            TestViewpoints[i, 1].location.x = (TestViewpoints[i, 1].location.x * Mathf.Cos(rotationAmount)) - (TestViewpoints[i, 1].location.y * Mathf.Sin(rotationAmount));
            TestViewpoints[i, 1].location.y = (TestViewpoints[i, 1].location.x * Mathf.Sin(rotationAmount)) + (TestViewpoints[i, 1].location.y * Mathf.Cos(rotationAmount));

            TestViewpoints[i, 0].location.x = (TestViewpoints[i, 0].location.x * Mathf.Cos(rotationAmount)) - (TestViewpoints[i, 0].location.y * Mathf.Sin(rotationAmount));
            TestViewpoints[i, 0].location.y = (TestViewpoints[i, 0].location.x * Mathf.Sin(rotationAmount)) + (TestViewpoints[i, 0].location.y * Mathf.Cos(rotationAmount));
            Debug.Log(TestViewpoints[i, 1].location.x + "2");
        }
        return outlist;

    }

    public void ScaleAndRotateViewpoint(Viewpoint vpToScale, float envScaleMod)
    {
        startPos.z = (distAway * envScaleMod);

        //randomRotation = 0;

        //this function is specific to the scale experiment 2 (very large scale changes.) when given a viewpoint, a scaling, a direction and a random integer that is consistant with this trial (stim 1 + stim 2). it will first apply the rotation
        // before then scaling the viewpoint relative to the participants standing height and the scale modifier provided.

        //Debug.Log(randomRotation + " " + vp);
        float randomRotationRads = randomRotation * Mathf.Deg2Rad;
        float viewChangeRads = vpToScale.angle * Mathf.Deg2Rad;

        //Debug.Log("ranrads = " + randomRotation + " vc = " + TestViewpoints[currentview,0].angle + " startz" + startPos.z);
        //Debug.Log(testPlusRand +" " +  clockwise);
        /*
        if (vp == 0 && vpToScale.angle == 135) //this little logic gate stops problems of clockwise/anticlockwise/ going over 180 degrees. it seems to work as is
        {
            if (testPlusRand > 180)
            {
                testPlusRand = 360 - testPlusRand;
                clockwise = !clockwise;
                Debug.Log("ammended" + testPlusRand);
                if (clockwise)
                {
                    direction = 0.2f;
                }
                else if (!clockwise)
                {
                   direction = 0.7f;
                }
            }
        }
        */
        //float thisAngleRads = Mathf.Deg2Rad * (testPlusRand); //apply rotation

        float currentheight = 1.5f;
        if (VREnabled == true)
        {
            currentheight = vrCam.transform.localPosition.y; //get standing/seated height
            //currentheight = exCo.participantHeight;
        }
        // -----------calculate position plus random rotation
        float pos1x = (Mathf.Cos(randomRotationRads)) * (startPos.x - center.x) - Mathf.Sin(randomRotationRads) * (startPos.z - center.z) + center.x;
        float pos1z = (Mathf.Sin(randomRotationRads)) * (startPos.x - center.x) + Mathf.Cos(randomRotationRads) * (startPos.z - center.z) + center.z;
        TestViewpoints[0,0].location = new Vector2(pos1x, pos1z);
        TestViewpoints[0,1].location = new Vector2(pos1x, pos1z);

        //calculate cw and acc

        float pos2x = (Mathf.Cos(viewChangeRads)) * (pos1x - center.x) - Mathf.Sin(viewChangeRads) * (pos1z - center.z) + center.x;
        float pos2z = (Mathf.Sin(viewChangeRads)) * (pos1x - center.x) + Mathf.Cos(viewChangeRads) * (pos1z - center.z) + center.z;
        TestViewpoints[currentview, 0].location = new Vector2(pos2x, pos2z);

        float pos3x = (Mathf.Cos(viewChangeRads * -1)) * (pos1x - center.x) - Mathf.Sin(viewChangeRads * -1) * (pos1z - center.z) + center.x;
        float pos3z = (Mathf.Sin(viewChangeRads * -1)) * (pos1x - center.x) + Mathf.Cos(viewChangeRads * -1) * (pos1z - center.z) + center.z;
        TestViewpoints[currentview, 1].location = new Vector2(pos3x, pos3z);

        //TestViewpoints[currentview, 0].location.y = -1 * (Mathf.Cos(thisAngleRads) * (1.8f * envScaleMod)); // = r cos(theta)
        //  TestViewpoints[currentview, 0].location.x = Mathf.Sin(thisAngleRads) * ( 1.8f * envScaleMod); // = r sin(theta)
        //Debug.Log(TestViewpoints[currentview, 1].location);
        // Debug.Log("yeppercw" + testPlusRand);
        // } else
        //{
        //  TestViewpoints[currentview, 1].location.y = -1 * (Mathf.Cos(thisAngleRads * -1) * ( 1.8f * envScaleMod));  // = r cos(theta)
        // TestViewpoints[currentview, 1].location.x = (Mathf.Sin(thisAngleRads * -1) * ( 1.8f * envScaleMod));  // = r sin(theta)
        // Debug.Log(TestViewpoints[currentview, 0].location);
        // Debug.Log("yepperacw" + testPlusRand);
        //}
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); //0 cw
        cube.transform.position = new Vector3(TestViewpoints[0,0].location.x, 0.5f, TestViewpoints[0, 0].location.y);
        Destroy(cube, 2);
        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube); //0 ccw
        cube2.transform.position = new Vector3(TestViewpoints[0, 1].location.x, 0.5f, TestViewpoints[0, 1].location.y);
        Destroy(cube2, 2);

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); //135 cw
        sphere.transform.position = new Vector3(TestViewpoints[currentview, 0].location.x, 0.5f, TestViewpoints[currentview,0].location.y);
        Destroy(sphere, 2);

        GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere); //135ccw
        sphere2.transform.position = new Vector3(TestViewpoints[currentview, 1].location.x, 0.5f, TestViewpoints[currentview, 1].location.y);
        Destroy(sphere2, 2);

    */

        viewpointElevation = (currentheight * envScaleMod) - currentheight;
        if (VREnabled == false && viewpointElevation == 0)
        {
            viewpointElevation = 0.8f;
        }

            QualitySettings.shadowDistance = 10 * envScaleMod; //important - large changes in scale messes up the shadow quallity. needs manual control here.
    }





    public void MoveView(float environmentScaleMod)
    {
        //this function applies scaling and rotation via the other functions and then actually moves the participant to the new vp and makes them look at the centre - this section has some VR specific elements.
       if(vp == 0)
        {
            //randomRotation = Random.Range(-180, 180);//(-1 * Mathf.PI), (Mathf.PI)); //rotation amount is plus or minus a random amount based on the number of objects in the array.
            randomRotation = 180;
            direction = Random.value;
            vp = 1;
            //Debug.Log("vp initialise: " + direction);
        }
        else
        {
            vp = 0;
        }



        float lookat;
        if (VREnabled == true)
        {
            lookat = viewpointElevation;
        }
        else
        {
            //viewpointElevation = 1.5f;
            //lookat = 0;// env.arenafloor.transform.position.y + 0.5f;
        }

        //the +1.5f below denotes the viewpoint height above the floor
        // Debug.Log("vp checkup: " + vp + " " + direction);

        ScaleAndRotateViewpoint(TestViewpoints[currentview, 1], environmentScaleMod);

        if (direction > 0.5) //anticlockwise
        {

            //ScaleAndRotateViewpoint(TestViewpoints[currentview, 0], environmentScaleMod, false, randomRotation);



            //teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 0].location.x, env.viewHeight, TestViewpoints[currentview, 0].location.y);
            //teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 0].lookat.x, env.arenafloor.transform.position.y +0.5f, TestViewpoints[currentview, 0].lookat.y)); // these define th height and position of viewpoints.
            //Debug.Log("vp el" + viewpointElevation);
            teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 0].location.x, viewpointElevation, TestViewpoints[currentview, 0].location.y);


            if (VREnabled == true)
            {
                teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 0].lookat.x, viewpointElevation, TestViewpoints[currentview, 0].lookat.y));
            }
            else
            {
                teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 0].lookat.x, 0.3f, TestViewpoints[currentview, 0].lookat.y));

            }

        }
        else //clockwise
        {

                //ScaleAndRotateViewpoint(TestViewpoints[currentview, 1], environmentScaleMod, true, randomRotation);

        //teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 1].location.x, env.viewHeight, TestViewpoints[currentview, 1].location.y);
        //teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 1].lookat.x, env.arenafloor.transform.position.y + 0.5f, TestViewpoints[currentview, 1].lookat.y));

        Debug.Log("vp el" + viewpointElevation);
            teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 1].location.x, viewpointElevation, TestViewpoints[currentview, 1].location.y);
            if (VREnabled == true)
            {
                teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 1].lookat.x, viewpointElevation, TestViewpoints[currentview, 1].lookat.y));
            }
            else
            {
                teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 1].lookat.x, 0.3f, TestViewpoints[currentview, 1].lookat.y)); //centre of table
            }
        }
    }
}

[System.Serializable]
public class Viewpoint{
    public Vector2 location;
    public Vector2 lookat = new Vector2(0, 0);
    public float angle;
}
