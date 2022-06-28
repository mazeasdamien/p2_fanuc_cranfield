using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDLN.CameraControllers
{
    public class CameraOrbit : MonoBehaviour
    {
        public GameObject target;
        public float distance = 5f;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20;
        public float yMaxLimit = 80;

        public float dragSpeed = 2;
        private Vector3 dragOrigin;

        float x = 0.0f;
        float y = 0.0f;

        void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        float prevDistance;

        void LateUpdate()
        {
            if (!IsPointerOverUIObject())
            {
                if (distance < 1) distance = 1;
                distance -= Input.GetAxis("Mouse ScrollWheel") * 2;
                if (target && (Input.GetMouseButton(1)))
                {
                    var pos = Input.mousePosition;
                    float dpiScale;
                    if (Screen.dpi < 1) _ = 1;
                    if (Screen.dpi < 200) dpiScale = 1;
                    else dpiScale = Screen.dpi / 200f;

                    if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

                    // comment out these two lines if you don't want to hide mouse curser or you have a UI button 
                    //Cursor.visible = false;
                    //Cursor.lockState = CursorLockMode.Locked;

                    x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                    y = ClampAngle(y, yMinLimit, yMaxLimit);
                    var rotation = Quaternion.Euler(y, x, 0);
                    var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                    transform.SetPositionAndRotation(position, rotation);

                }
                else
                {
                    // comment out these two lines if you don't want to hide mouse curser or you have a UI button 
                    //Cursor.visible = true;
                    //Cursor.lockState = CursorLockMode.None;
                }

                if (Math.Abs(prevDistance - distance) > 0.001f)
                {
                    prevDistance = distance;
                    var rot = Quaternion.Euler(y, x, 0);
                    var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                    transform.SetPositionAndRotation(po, rot);
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(2)) return;

            Vector3 posi = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(posi.x * dragSpeed, 0, posi.y * dragSpeed);
            target.transform.Translate(move,Space.World);
            transform.Translate(move, Space.World);
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}