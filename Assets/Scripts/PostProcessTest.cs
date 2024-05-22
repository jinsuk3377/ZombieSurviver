using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using System.Drawing;

public class PostProcessTest : MonoBehaviour
{
    ColorGrading color2;
    public enum State
    {
        a,
        b,
        c
    }
    public static State state;
    PostProcessVolume color;
    // Start is called before the first frame update
    void Start()
    {
        color= GetComponent<PostProcessVolume>();
        color2 = color.profile.GetSetting<ColorGrading>();
        color2.saturation.value = 100;
    }

    // Update is called once per frame
    void Update()
    {
        color2.saturation.value = Mathf.Lerp(color2.saturation.value, -100, Time.deltaTime);
    }
}
