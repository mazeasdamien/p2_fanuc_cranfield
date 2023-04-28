﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VarjoExample
{
    public class Hand : MonoBehaviour
    {
        public Transform xrRig;
        public GameObject pointcloud;
        public GameObject PCorigin;
        public GameObject tempo;

        Controller controller;

        public List<Interactable> contactedInteractables = new List<Interactable>();
        private bool triggerDown;
        private FixedJoint fixedJoint = null;
        private Interactable currentInteractable;
        private Rigidbody heldObjectBody;

        public MovetoManager dropped;

        void Awake()
        {
            controller = GetComponent<Controller>();
            fixedJoint = GetComponent<FixedJoint>();
        }

        // Update is called once per frame
        void Update()
        {
            if (controller.triggerButton)
            {
                if (!triggerDown)
                {
                    triggerDown = true;
                    Pick();
                }
            }
            else if (!controller.primary2DAxisClick && triggerDown)
            {
                triggerDown = false;
                Drop();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Pickable") || other.gameObject.CompareTag("Fracture"))
            {
                contactedInteractables.Add(other.gameObject.GetComponent<Interactable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Pickable") || other.gameObject.CompareTag("Fracture"))
            {
                contactedInteractables.Remove(other.gameObject.GetComponent<Interactable>());
            }
        }

        public void Pick()
        {
            pointcloud.transform.SetParent(tempo.transform);
            foreach (Interactable i in contactedInteractables)
            {
                i.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            currentInteractable = GetNearestInteractable();

            if (!currentInteractable)
            {
                return;
            }

            // Drop interactable if already held
            if (currentInteractable.activeHand)
            {
                currentInteractable.activeHand.Drop();
            }

            // Attach
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            fixedJoint.connectedBody = heldObjectBody;

            // Set active hand
            currentInteractable.activeHand = this;
        }

        public void Drop()
        {
            pointcloud.transform.SetParent(PCorigin.transform);

            foreach (Interactable i in contactedInteractables)
            {
                i.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            dropped.sendDropped = true;

            if (!currentInteractable)
                return;

            // Detach
            fixedJoint.connectedBody = null;

            // Apply velocity
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            heldObjectBody.velocity = xrRig.TransformVector(controller.DeviceVelocity);
            heldObjectBody.angularVelocity = xrRig.TransformDirection(controller.DeviceAngularVelocity);

            // Clear
            currentInteractable.activeHand = null;
            currentInteractable = null;

            //Invoke("FollowPC",3);
        }
        /*
        void FollowPC()
        {
            pointcloud.transform.SetParent(PCorigin.transform);
        }
        */
        private Interactable GetNearestInteractable()
        {
            Interactable nearest = null;
            float minDistance = float.MaxValue;
            float distance = 0.0f;

            foreach (Interactable interactable in contactedInteractables)
            {
                if (interactable)
                {
                    distance = (interactable.transform.position - transform.position).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = interactable;
                    }
                }
            }
            return nearest;
        }
    }
}