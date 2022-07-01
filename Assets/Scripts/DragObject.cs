using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    public bool released;
    private bool isChecked;

    private void Update()
    {

        released = false;

        if (gameObject.GetComponent<Rigidbody>().angularDrag != 0 && isChecked == false)
        {
            released = true;
            isChecked = true;
        }

        if (gameObject.GetComponent<Rigidbody>().angularDrag == 0 && isChecked == true)
        {
            isChecked = false;
        }
    }
}
