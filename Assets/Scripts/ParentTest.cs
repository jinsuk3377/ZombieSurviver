using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTest : MonoBehaviour
{

    public GameObject parent;
    public GameObject child;
    // Start is called before the first frame update
    void Start()
    {
        child.transform.SetParent(parent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
