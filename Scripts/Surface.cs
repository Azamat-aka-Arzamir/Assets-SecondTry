using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    [field:SerializeField]
    public bool Hard { get;private set; }
    [field:SerializeField]
    public int id { get; private set; }
    static int count;
    public Surface()
    {
        id = count;
        count++;
    }
}
