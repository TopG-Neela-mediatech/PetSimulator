using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    float val = 0;

    private void Update()
    {
        val = Input.GetAxisRaw("Horizontal");
        if (val != 0)
        {
            Debug.Log("Horizontal: " + val);
        }
    }
}
