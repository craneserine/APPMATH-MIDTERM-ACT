using UnityEngine;

public class TweenUtils : MonoBehaviour
{
    public static float EaseIn(float t)
    {
        return t * t;
    }
   
    public static float EaseOut(float t)
    {
        return 1 - (1 -t) * (1- t) ;
    }

    public static float NormalizeTime(float t, float maxTime)
    {
        return Mathf.Clamp01(t/maxTime);
    }

    public static float EaseInOut(float t)
    {
        return t < 0.5f ? EaseIn(t) : EaseOut(t);
    }

    public static float EaseInCustom(float t, int exponent)
    {
        return Mathf.Pow(t, exponent);
    }
   
    // New easing functions:
   
    public static float EaseInCubic(float t)
    {
        return t * t * t; // t^3
    }

    public static float EaseOutCirc(float t)
    {
        return Mathf.Sqrt(1 - (t - 1) * (t - 1)); // sqrt(1 - (t-1)^2)
    }

    public static float EaseInQuad(float t)
    {
        return t * t; // t^2 (same as EaseIn but explicit)
    }
   
}