using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviTest : MonoBehaviour
{
    enum TestEnum
    {
        a,
        b,
        c
    }

    NavMeshAgent agent;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, target.position) > 0.3f)
        {
            agent.SetDestination(target.position);
        }

        EventTest[] eventTexts = { new EventChildren01(), new EventChildren02(), new EventChildren03() };
        eventTexts[0].parentAction += () => Test(TestEnum.a);
    }

    void Test(TestEnum _enum)
    {

    }
}
