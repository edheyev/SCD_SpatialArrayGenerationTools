using UnityEngine;
using System.Collections;

public class StaircaseController : MonoBehaviour {

    public int nup=1;
    public int ndown=1;
    public int nreversals;
    public int numTrialsPerformed = 0;
    public int maxTrials = 50;
    public System.Collections.Generic.List<uint> seedPool; //list containing valid random seeds. This is to match spatial 
    public int seedPoolStartNum = 0;
    public int defaultSeedPoolSize; //ie about how many trials per staircase
    int seedPoolGrowthRate = 10; //if you run out of seeds how many new seeds do you want to add to the seed pool
    int seedPoolCurrentMax;

    public System.Collections.Generic.List<float> levels; //list of stimulus intensity levels, ordered from low to high
    //public int referencelevel;
    public string levelunits;
    public bool beginlow = true;
    public enum StaircaseDirection {STAIRCASE_UP, STAIRCASE_DOWN, STAIRCASE_NONE}; //down means toward lower intensity, smaller differences, lower index in levels, more difficult
    public System.Collections.Generic.List<int> stepsizes; 
    public int currentstepsize;

    public int currentlevel;
    public int perceived_counter; // count consecutive trials where the experimental stimulus was perceived (or for PSE paradigm, perceived as being more intense)
    public int not_perceived_counter; //count consecutive trials where the experimental stimulus was not perceived (or for PSE paradigm, perceived as being less intense)
    public int currentreversals;
    private StaircaseDirection nexttarget;

    public float viewAngleChangeDegrees;
    public float arrayScaleMultiplier;
   
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Initialize()
    {
        if (beginlow)
        {
            currentlevel = 0;
            currentlevel = 18;
            nexttarget = StaircaseDirection.STAIRCASE_UP;
        }
        else
        {
            currentlevel = levels.Count - 1;
            currentlevel = 18;
            nexttarget = StaircaseDirection.STAIRCASE_DOWN;
        }
        
        if (stepsizes.Count != (nreversals - 1))
        {
            Debug.Log("Stepsizes not specified for expected reversals, using smallest step for all trials");
           // stepsizes.Clear();
            for (int i = 0; i < (nreversals - 1); i++)
            {
               // stepsizes.Add(1);
            }
        }
        defaultSeedPoolSize = 70;
        for (int i = seedPoolStartNum; i < seedPoolStartNum + defaultSeedPoolSize; i++)
        {
          //  Debug.Log(i + "seeds");
            seedPool.Add((uint)i);
        }

        seedPoolCurrentMax = seedPoolStartNum + defaultSeedPoolSize;


        currentstepsize = stepsizes[currentreversals];
    }

    public void ProcessResponse(bool perceived)
    {
        numTrialsPerformed++;
        if (perceived)
        {
            //threshold paradigm - the experimental stimulus intensity should be reduced - move down the staircase
            //PSE paradigm - the experimental stimulus is perceived as more intense than the reference stimulus - its intensity should be reduced, move down the staircase
            Debug.Log("Stimulus perceived, or perceived as more intense than reference.");
            perceived_counter++;
            not_perceived_counter = 0;
            if (perceived_counter>=ndown && currentlevel != 0) //repeat current stimulus intensity unless ndown consecutive perceived responses have been counted !! - ed added && currentlevel != 0 so that no changes happen when at the bottom of the SC
            {
                Debug.Log("Move down staircase - next stimulus should be less intense.");
                //currentlevel -= currentstepsize;        //moving this below process rev    
                
                if (nexttarget != StaircaseDirection.STAIRCASE_DOWN)
                {
                    //this is a reversal
                    ProcessReversal();
                }
                currentlevel -= currentstepsize;
                nexttarget = StaircaseDirection.STAIRCASE_DOWN;
                perceived_counter = 0;
            }
            else
            {
                Debug.Log("Intensity will be repeated on next trial until consecutive 'percieved' responses reaches " + ndown);
            }
        }
        else
        {
            //threshold paradigm - the experimental stimulus is not detected - its intensity should be increased - move up the staircase
            //PSE paradigm - the experimental stimulus is perceived as less intense than the reference stimulus - its intensity should be increased, move up the staircase

            Debug.Log("Stimulus not perceived, or as less intense than reference.");
            not_perceived_counter++;
            perceived_counter = 0;
            if (not_perceived_counter>=nup && currentlevel != levels.Count -1) //repeat current stimulus intensity unless nup consecutive not perceived responses have been counted !! - ed added && currentlevel != levels.Count-1 so that no changes happen when at the top of the SC
            {
                Debug.Log("Move up staircase - next stimulus should be more intense.");
                //currentlevel += currentstepsize;      //moving this below process rev
                if (nexttarget!= StaircaseDirection.STAIRCASE_UP)
                {
                    //this is a reversal
                    ProcessReversal();
                }
                currentlevel += currentstepsize;
                nexttarget = StaircaseDirection.STAIRCASE_UP;                
                not_perceived_counter = 0;
            }
            else
            {
                Debug.Log("Intensity will be repeated on next trial until consecutive 'not percieved' responses reaches " + nup);
            }
        }

        if (currentlevel<0)      // !!!!! here is the problem - this check is made after reversal is processed (above). so reversals are still processed when participants are in the buffer zones of the floor and ceiling. If they get an initial one wrong and then 3 right they  get 2 reversals and skip the biggest step sizes.  
        {
            currentlevel = 0;            
            Debug.Log("Warning - currentlevel set to minimum specified intensity; can't go lower.");
        }

        if (currentlevel > levels.Count-1)
        {
            currentlevel = levels.Count-1;            
            Debug.Log("Warning - currentlevel set to maximum specified intensity; can't go higher.");
        }
    }

    void ProcessReversal()
    {
        /*
        if (currentlevel == 0 || currentlevel == levels.Count - 1)
        {
            //dont change the reversals
        }
        else
        {
            currentreversals++;
        }
        */
        currentreversals++;
        if (currentreversals < (nreversals-1))
        {
            currentstepsize = stepsizes[currentreversals];
            Debug.Log(this.name + " reversal: " + currentreversals + "/" + nreversals + ", new stepsize: " + currentstepsize);
        }
        else
        {
            Debug.Log(this.name + " reversals completed.");
        }        
    }

    public float GetCurrentIntensity()
    {
        return levels[currentlevel];
    }       

    public uint GetValidSeed()
    {
        //choose valid seed
        int seedPoolRandLoc = Random.Range(0, seedPool.Count);
        uint chosenSeed = seedPool[seedPoolRandLoc]; //select random controller from list
        seedPool.RemoveAt(seedPoolRandLoc);//remove chosen seed
        
        //if empty ad x more to pool
        if(seedPool.Count == 0)
        {
            for(int i = seedPoolCurrentMax + 1; i <= seedPoolCurrentMax + seedPoolGrowthRate; i++)
            {
                seedPool.Add((uint)i);
            }
            seedPoolCurrentMax += seedPoolGrowthRate;
        }

        return chosenSeed;
    }

}
