using UnityEngine;

namespace Mechanics
{
    public class SwipeMecLast : Ragdoll
    {
        [Header("Swipe Variables")]
        public bool posSwipe = true; // if the game only use swipe for position set initial value true ,  //for rotation set initial value false    
        public float clampMaxVal; // min value will be minus of max. 
        public float mouseDamp = 600;
        public Transform obj;

        [Header("Others")]
        private float startPosX;
        private float deltaMousePos;

        bool isTouchScreen;

        [HideInInspector] public Vector3 desiredPos = Vector3.zero;
        public virtual void Start()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                isTouchScreen = true;
                Input.multiTouchEnabled = false;
            }
            else
                isTouchScreen = false;
        }

        public void VariableAdjust(Transform objTocontrol, float lerpMultiplier, float clampMax, float dampValue, bool posActive) // start ta çağır
        {
            clampMaxVal = clampMax;
            posSwipe = posActive;
            obj = objTocontrol;
            mouseDamp = dampValue;
        }

        public void Swipe() // her frameda çalışıyor
        {
            if (isTouchScreen)
                TouchControl();
            else
                MouseControl();
        }
        void MouseControl()
        {
            if (Input.GetMouseButtonDown(0))
                ResetValues();
            else if (Input.GetMouseButton(0))
                ControlOnHold();
        }

        void TouchControl()
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    ResetValues();
                    break;

                case TouchPhase.Moved:
                    ControlOnHold();
                    break;
            }
        }

        void ControlOnHold()
        {
            deltaMousePos = Input.mousePosition.x - startPosX;
            PositionMethod2();
        }

        public void ResetValues() => startPosX = Input.mousePosition.x;

        void PositionMethod2() 
        {
            float xPos = obj.position.x;
            xPos = Mathf.Lerp(xPos, xPos + (mouseDamp * (deltaMousePos / Screen.width)), Time.deltaTime);
            xPos = Mathf.Clamp(xPos, -clampMaxVal, clampMaxVal);
            obj.position = new Vector3(xPos, obj.position.y, obj.position.z);
            ResetValues();
        }
    }
}

