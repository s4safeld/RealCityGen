using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
        private CharacterController myCC;

        private PlayerMovement pm;

        Transform camTransform;


        public float movementSpeed;
        public float rotationSpeed;

        private void Start()
        {
            camTransform = Camera.main.transform;
            pm = this;
            myCC = GetComponent<CharacterController>();
        }
        

        // Update is called once per frame
        void Update()
        {
            BasicMovement();
            BasicRotation(); 
        }

        void BasicMovement()
        {

            if (Input.GetKey(KeyCode.W))
            {
                myCC.Move(transform.forward * Time.deltaTime * movementSpeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                myCC.Move(transform.right * Time.deltaTime * movementSpeed);
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
                camTransform.Rotate(new Vector3(Mathf.Clamp(-1, -0.7f, 0.7f), 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(-1, -10000, 10000), 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                camTransform.Rotate(new Vector3(Mathf.Clamp(1, -0.7f, 0.7f), 0, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, Mathf.Clamp(1, -10000, 10000), 0));
            }

        }
    
}
