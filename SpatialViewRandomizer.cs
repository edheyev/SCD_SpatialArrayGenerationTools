using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialViewRandomizer : MonoBehaviour {

    PRandStream randstream;

    uint _noise_seed;
    uint _array_generator_seed;
    uint _viewpoint_randomizer_seed;
    uint _conditions_randomizer_seed;

    public void Start()
    {
        randstream = new PRandStream(220);
        Debug.Log("Random Number Checks...");
        Debug.Log("SetRandomizer 100");
        SetRandomizer(100);
        Debug.Log("Noise Seed: " + _noise_seed);
        Debug.Log("Noise Seed with offset 1: " + GetNoiseSeed(1));
        Debug.Log("Noise Seed with offset 2: " + GetNoiseSeed(2));
        Debug.Log("Noise Seed with offset 3: " + GetNoiseSeed(3));
        Debug.Log("Noise Seed with offset 4: " + GetNoiseSeed(4));
        Debug.Log("Array Generator Seed: " + _array_generator_seed);
        Debug.Log("Array Generator Seed with offset 1: " + GetArrayGeneratorSeed(1));
        Debug.Log("Array Generator Seed with offset 2: " + GetArrayGeneratorSeed(2));
        Debug.Log("Array Generator Seed with offset 3: " + GetArrayGeneratorSeed(3));
        Debug.Log("Array Generator Seed with offset 4: " + GetArrayGeneratorSeed(4));
        Debug.Log("Viewpoint Randomizer Seed: " + _viewpoint_randomizer_seed);
        Debug.Log("Conditions Randomizer Seed: " + _conditions_randomizer_seed);
        Debug.Log("SetRandomizer 200");
        SetRandomizer(200);
        Debug.Log("Noise Seed: " + _noise_seed);
        Debug.Log("Noise Seed with offset 1: " + GetNoiseSeed(1));
        Debug.Log("Noise Seed with offset 2: " + GetNoiseSeed(2));
        Debug.Log("Noise Seed with offset 3: " + GetNoiseSeed(3));
        Debug.Log("Noise Seed with offset 4: " + GetNoiseSeed(4));
        Debug.Log("Array Generator Seed: " + _array_generator_seed);
        Debug.Log("Array Generator Seed with offset 1: " + GetArrayGeneratorSeed(1));
        Debug.Log("Array Generator Seed with offset 2: " + GetArrayGeneratorSeed(2));
        Debug.Log("Array Generator Seed with offset 3: " + GetArrayGeneratorSeed(3));
        Debug.Log("Array Generator Seed with offset 4: " + GetArrayGeneratorSeed(4));
        Debug.Log("Viewpoint Randomizer Seed: " + _viewpoint_randomizer_seed);
        Debug.Log("Conditions Randomizer Seed: " + _conditions_randomizer_seed);
        Debug.Log("SetRandomizer 100");
        SetRandomizer(100);
        Debug.Log("Noise Seed: " + _noise_seed);
        Debug.Log("Noise Seed with offset 1: " + GetNoiseSeed(1));
        Debug.Log("Noise Seed with offset 2: " + GetNoiseSeed(2));
        Debug.Log("Noise Seed with offset 3: " + GetNoiseSeed(3));
        Debug.Log("Noise Seed with offset 4: " + GetNoiseSeed(4));
        Debug.Log("Array Generator Seed: " + _array_generator_seed);
        Debug.Log("Array Generator Seed with offset 1: " + GetArrayGeneratorSeed(1));
        Debug.Log("Array Generator Seed with offset 2: " + GetArrayGeneratorSeed(2));
        Debug.Log("Array Generator Seed with offset 3: " + GetArrayGeneratorSeed(3));
        Debug.Log("Array Generator Seed with offset 4: " + GetArrayGeneratorSeed(4));
        Debug.Log("Viewpoint Randomizer Seed: " + _viewpoint_randomizer_seed);
        Debug.Log("Conditions Randomizer Seed: " + _conditions_randomizer_seed);
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        Debug.Log("Random from Range [0 1 2 3]:" + randstream.Range(0, 4));
        for (int i=0;i<100;i++)
        {
            Debug.Log("Random float [0-1]:" + randstream.Value());
        }

    }

    public void SetRandomizer(uint masterseed)
    {
        randstream.SetSeed(masterseed);
        _noise_seed = randstream.Next();
        _array_generator_seed = randstream.Next();
        _viewpoint_randomizer_seed= randstream.Next();
        _conditions_randomizer_seed = randstream.Next();
    }

    public uint GetNoiseSeed()
    {
        return _noise_seed;
    }

    public uint GetArrayGeneratorSeed()
    {
        return _array_generator_seed;
    }

    public uint GetViewpointRandomizerSeed()
    {
        return _viewpoint_randomizer_seed;
    }

    public uint GetConditionsRandomizerSeed()
    {
        return _conditions_randomizer_seed;
    }

    public uint GetNoiseSeed(uint offset)
    {
        if (offset == 0)
        {
            return _noise_seed;
        }
        else
        {
            uint tempseed = randstream.GetSeed();
            randstream.SetSeed(_noise_seed + offset);//jump to a new point in the pseudo random stream
            uint offsetseedvalue = randstream.Next();
            randstream.SetSeed(tempseed); //reset seed to previous value
            return offsetseedvalue;
        }
    }

    public uint GetArrayGeneratorSeed(uint offset)
    {
        if (offset == 0)
        {
            return _array_generator_seed;
        }
        else
        {
            uint tempseed = randstream.GetSeed();
            randstream.SetSeed(_array_generator_seed + offset);//jump to a new point in the pseudo random stream
            uint offsetseedvalue = randstream.Next();
            randstream.SetSeed(tempseed); //reset seed to previous value
            return offsetseedvalue;
        }
    }

    public uint GetViewpointRandomizerSeed(uint offset)
    {
        if (offset == 0)
        {
            return _viewpoint_randomizer_seed;
        }
        else
        {
            uint tempseed = randstream.GetSeed();
            randstream.SetSeed(_viewpoint_randomizer_seed + offset);//jump to a new point in the pseudo random stream
            uint offsetseedvalue = randstream.Next();
            randstream.SetSeed(tempseed); //reset seed to previous value
            return offsetseedvalue;
        }
    }

    public uint GetConditionsRandomizerSeed(uint offset)
    {
        if (offset == 0)
        {
            return _conditions_randomizer_seed;
        }
        else
        {
            uint tempseed = randstream.GetSeed();
            randstream.SetSeed(_conditions_randomizer_seed + offset);//jump to a new point in the pseudo random stream
            uint offsetseedvalue = randstream.Next();
            randstream.SetSeed(tempseed); //reset seed to previous value
            return offsetseedvalue;
        }
    }

    public int GetRandomIndex(uint seed, int maxrange)
    {
        uint tempseed = randstream.GetSeed();
        randstream.SetSeed(seed);//jump to a new point in the pseudo random stream      
        int outval = randstream.Range(0, maxrange);
        randstream.SetSeed(tempseed); //reset seed to avoid undesired side-effects
        return outval;
    }

    public int GetRandomIndexWithSkip(uint seed, int maxrange,int skip)
    {
        uint tempseed = randstream.GetSeed();
        randstream.SetSeed(seed);//jump to a new point in the pseudo random stream        
        int outval = randstream.RangeWithSkip(0, maxrange, skip);
        randstream.SetSeed(tempseed); //reset seed to avoid undesired side-effects
        return outval;
    }

}

