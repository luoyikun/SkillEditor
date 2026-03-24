using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestStruct : MonoBehaviour
{
    Dictionary<int,B> dic = new Dictionary<int,B>();
    // Start is called before the first frame update
    void Start()
    {
        B b = new B();
        b.c = 10;


        dic[1] = b;
        //(dic[1]).c = 20;
        if (dic.TryGetValue(1, out B bStr))
        {
            bStr.c = 20;
        }

        Debug.Log(dic[1].c);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    struct B
    {
        public int c;
    }

    class A
    {
        public B b;
    }
}
