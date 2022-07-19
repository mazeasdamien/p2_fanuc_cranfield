using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InractionModeManager : MonoBehaviour
{
    public enum Mode { Controller, Hand, Tracking };

    Mode mode;

    public List<GameObject> HandObj;
    public List<GameObject> ControllerObj;
    public List<GameObject> TrackingObj;

    private void Start()
    {
        mode = Mode.Controller;
    }

    void Update()
    {
        if (mode == Mode.Controller)
        {
            foreach (GameObject go in HandObj)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in ControllerObj)
            {
                go.SetActive(true);
            }

            foreach (GameObject go in TrackingObj)
            {
                go.SetActive(false);
            }
        }
        else if (mode == Mode.Hand)
        {
            foreach (GameObject go in HandObj)
            {
                go.SetActive(true);
            }

            foreach (GameObject go in ControllerObj)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in TrackingObj)
            {
                go.SetActive(false);
            }
        }
        else if (mode == Mode.Tracking)
        {
            foreach (GameObject go in HandObj)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in ControllerObj)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in TrackingObj)
            {
                go.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            mode = Mode.Controller;
            Debug.Log(mode.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {

            mode = Mode.Hand;
            Debug.Log(mode.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {

            mode = Mode.Tracking;
            Debug.Log(mode.ToString());
        }
    }
}