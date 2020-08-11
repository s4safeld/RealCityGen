using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
        private CharacterController myCC;
        private Transform groundCursor;
        private Transform camTransform;
        private bool initialized = false;
        private float movementSpeed;
        private float rotationSpeed;

        public void Initialise()
        {
            groundCursor = giAlternative.groundCursor;
            camTransform = Camera.main.transform;
            myCC = GetComponent<CharacterController>();
            initialized = true;
            movementSpeed = giAlternative.MovementSpeed;
            rotationSpeed = giAlternative.RotationSpeed;
            if (!groundCursor)
            {
                groundCursor = Instantiate(new GameObject(), new Vector3(transform.position.x, 0, transform.position.y), Quaternion.identity, transform).transform;
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            if (initialized)
            {
                BasicMovement();
                BasicRotation();
                Debug.DrawLine(groundCursor.transform.position, transform.position, Color.white);
            }
            
        }

        void BasicMovement()
        {

            if (Input.GetKey(KeyCode.W))
            {
                myCC.Move(transform.forward * Time.deltaTime * movementSpeed);
                groundCursor.position = new Vector3(transform.position.x,0,transform.position.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
                groundCursor.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            if (Input.GetKey(KeyCode.S))
            {
                myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
                groundCursor.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            if (Input.GetKey(KeyCode.D))
            {
                myCC.Move(transform.right * Time.deltaTime * movementSpeed);
                groundCursor.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                myCC.Move(transform.up * Time.deltaTime * movementSpeed);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                myCC.Move(-transform.up * Time.deltaTime * movementSpeed);
            }
        }

        void BasicRotation()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed;
                transform.Rotate(new Vector3(0, mouseX, 0));
                groundCursor.Rotate(new Vector3(0, mouseX, 0));
                //Debug.Log(camTransform.localRotation.x);
            if (camTransform.localRotation.x < 0.7 && camTransform.localRotation.x > -0.7)
                {
                    camTransform.Rotate(new Vector3(-mouseY, 0, 0));
                }
                else
                {
                    if (camTransform.localRotation.x < 0.7)
                    {
                        camTransform.Rotate(new Vector3(0.1f, 0, 0));
                    }
                    else
                    {
                        camTransform.Rotate(new Vector3(-0.1f, 0, 0));
                    }

                }
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camTransform.Rotate(new Vector3(Mathf.Clamp(-1, -0.7f, 0.7f) * Time.deltaTime * rotationSpeed, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(-1, -10000, 10000) * Time.deltaTime * rotationSpeed, 0));
                groundCursor.rotation = transform.rotation;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                camTransform.Rotate(new Vector3(Mathf.Clamp(1, -0.7f, 0.7f) * Time.deltaTime * rotationSpeed, 0, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(1, -10000, 10000) * Time.deltaTime * rotationSpeed, 0));
                groundCursor.rotation = transform.rotation;
            }

        }
    
}
