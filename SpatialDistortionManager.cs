using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialDistortionManager : MonoBehaviour {
    // this spatial distortion manager converts staircase level into spatial distortion information
    public EnvironmentSpecifications env;
    public float standard_distortion_radius = 60; //absolute distance that feature will be moved following spatial distortion
    public float ran1, ran2;
    private void Awake()
    {
        //standard_distortion_radius = env.environmentRadius * 0.49f;
        //standard_distortion_radius = 1.3f;

    }
    public float AbsoluteDistortionFromStandard(float distortion_level)
    {
        Debug.Log("dmanager _ " + distortion_level + "__" + standard_distortion_radius);
        return standard_distortion_radius * distortion_level;
    }
    public float NormalisedRandomDistortion(float mean, float sd)//float minValue, float maxValue, float mean, float standardDeviation)
    {
      float s = 0;
      //float spare = 0;

      bool foundgaussian = false;
      //bool hasSpare = false;
      while (foundgaussian == false)
      {
        while(s>=1 || s ==0){
          ran1 = 2.0f * Random.Range(0f, 1.0f) - 1f;
          ran2 = 2.0f * Random.Range(0f, 1.0f) - 1f;
          s = ran1 * ran1 + ran2 * ran2;
        }
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
        //spare = ran1 * ran2;
        //hasSpare = true;

        foundgaussian = true;
      }
    return mean + sd * ran1 * s;
  }
}
