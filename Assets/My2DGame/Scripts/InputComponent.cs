using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
    public abstract class InputComponent : MonoBehaviour
    {
        public enum InputType
        {
            MouseAndKeyBoard,
            Controller
        }

        public enum XboxControllerButtons
        {
            None,
            A,
            B,
            X,
            Y,
            Leftstick,
            Rightstick,
            View,
            Menu,
            LeftBumper,
            RightBumper,
        }

        public enum XboxControllerAxes
        {
            None,
            LeftstickHorizonta,
            LeftstickVertical,
            DpadHorizonta,
            DpadVertical,
            RightstickHorizontal,
            RightstickVertical,
            LeftTrigger,
            RightTrigger
        }

        
        [Serializable]
        public class InputButton
        {
            public KeyCode key;
            public XboxControllerButtons controllerButton;
            public bool Down{get; protected set;}
            public bool Held{get; protected set;}
            public bool Up{get; protected set;}
            public bool Enabled
            {
                get
                {
                    return m_Enabled;
                }   
            }

            [SerializeField]
            protected bool m_Enabled = true;
            protected bool m_GettingInput = true;

            bool m_AfterFixedUpdateDown;
            bool m_AfterFixedUpdateHeld;
            bool m_AfterFixedUpdateUp;

            protected static readonly Dictionary<int, string> k_ButtonsToName = new Dictionary<int, string>()
            {
                {(int)XboxControllerButtons.A, "A"},
                {(int)XboxControllerButtons.B, "B"},
                {(int)XboxControllerButtons.X, "X"},
                {(int)XboxControllerButtons.Y, "Y"},
                {(int)XboxControllerButtons.Leftstick, "Leftstick"},
                {(int)XboxControllerButtons.Rightstick, "Rightstick"},
                {(int)XboxControllerButtons.LeftBumper, "Left Bumper"},
                {(int)XboxControllerButtons.RightBumper, "Right Bumper"}
            };

            public InputButton(KeyCode key, XboxControllerButtons controllerButton)
            {
                this.key = key;
                this.controllerButton = controllerButton;
            }

            public void Get(bool fixedUpdateHappened, InputType inputType)
            {
                if(!m_Enabled)
                {
                    Down = false;
                    Held = false;
                    Up = false;
                    return ;
                }

                if(!m_GettingInput)
                {
                    return ;
                }

                if(inputType == InputType.Controller)
                {
                    if(fixedUpdateHappened)
                    {
                        Down = Input.GetButtonDown(k_ButtonsToName[(int)controllerButton]);
                        Held = Input.GetButton(k_ButtonsToName[(int)controllerButton]);
                        Up = Input.GetButtonUp(k_ButtonsToName[(int)controllerButton]);

                        m_AfterFixedUpdateDown = Down;
                        m_AfterFixedUpdateHeld = Held;
                        m_AfterFixedUpdateUp = Up;
                    }else
                    {
                        Down = Input.GetButtonDown(k_ButtonsToName[(int)controllerButton]) || m_AfterFixedUpdateDown;
                        Held = Input.GetButton(k_ButtonsToName[(int)controllerButton]) || m_AfterFixedUpdateHeld;
                        Up = Input.GetButtonUp(k_ButtonsToName[(int)controllerButton]) || m_AfterFixedUpdateHeld;

                        m_AfterFixedUpdateDown |= Down;
                        m_AfterFixedUpdateHeld |= Held;
                        m_AfterFixedUpdateUp |= Up;
                    }
                }else
                {
                    if(fixedUpdateHappened)
                    {
                        Down = Input.GetKeyDown(key);
                        Held = Input.GetKey(key);
                        Up = Input.GetKeyUp(key);

                        m_AfterFixedUpdateDown = Down;
                        m_AfterFixedUpdateHeld = Held;
                        m_AfterFixedUpdateUp = Up;
                    }else
                    {
                        Down = Input.GetKeyDown(key) || m_AfterFixedUpdateDown;
                        Held = Input.GetKey(key) || m_AfterFixedUpdateHeld;
                        Up = Input.GetKeyUp(key) || m_AfterFixedUpdateUp;

                        m_AfterFixedUpdateDown |= Down;
                        m_AfterFixedUpdateHeld |= Held;
                        m_AfterFixedUpdateUp |= Up;
                    }
                }
            }

            public void Enable()
            {
                m_Enabled = true;
            }

            public void Disable()
            {
                m_Enabled = false;
            }

            public void GainControl()
            {
                m_GettingInput = true;
            }

            public IEnumerator ReleaseControl(bool resetValues)
            {
                m_GettingInput = false;

                if(!resetValues)
                {
                    yield break;
                }

                if(Down)
                    Up = true;
                Down = false;
                Held = false;

                m_AfterFixedUpdateDown = false;
                m_AfterFixedUpdateHeld = false;
                m_AfterFixedUpdateUp = false;

                yield return null;
                Up = false;
            }

        }

        [Serializable]
        public class InputAxis
        {
            public KeyCode positive;
            public KeyCode negative;
            public XboxControllerAxes controllerAxis;
            public float Value{get; protected set;}
            public bool ReceivingInput{get; protected set;}
            public bool Enabled
            {
                get
                {
                    return m_Enabled;
                }
            }

            protected bool m_Enabled = true;
            protected bool m_GettingInput = true;

            protected readonly static Dictionary<int, string> k_AxisToName = new Dictionary<int, string>()
            {
                {(int)XboxControllerAxes.LeftstickHorizonta, "Leftstick Horizontal"},
                {(int)XboxControllerAxes.LeftstickVertical, "Leftstick Vertical"},
                {(int)XboxControllerAxes.DpadHorizonta, "Dpad Horizontal"},
                {(int)XboxControllerAxes.DpadVertical, "Dpad Vertical"},
                {(int)XboxControllerAxes.RightstickHorizontal, "Rightstick Horizontal"},
                {(int)XboxControllerAxes.RightstickVertical, "Rightstick Vertical"},
                {(int)XboxControllerAxes.LeftTrigger, "Left Trigger"},
                {(int)XboxControllerAxes.RightTrigger, "Right Trigger"},
            };

            public InputAxis(KeyCode positive, KeyCode negative, XboxControllerAxes controllerAxis)
            {
                this.positive = positive;
                this.negative = negative;
                this.controllerAxis = controllerAxis;
            }   

            public void Get(InputType inputType)
            {
                if(!m_Enabled)
                {
                    Value = 0f;
                    return ;
                }

                if(!m_GettingInput)
                {
                    return ;
                }

                bool positiveHeld = false;
                bool negativeHeld = false;

                if(inputType == InputType.Controller)
                {
                    float value = Input.GetAxis(k_AxisToName[(int)controllerAxis]);
                    positiveHeld = value > Single.Epsilon;
                    negativeHeld = value < - Single.Epsilon;
                }else if(inputType == InputType.MouseAndKeyBoard)
                {
                    positiveHeld = Input.GetKey(positive);
                    negativeHeld = Input.GetKey(negative);
                }

                if(positiveHeld == negativeHeld)
                {
                    Value = 0;
                }else if(positiveHeld)
                {
                    Value = 1f;
                }else 
                {
                    Value = -1f;
                }

                ReceivingInput = positiveHeld || negativeHeld;
            }

            public void Enable()
            {
                m_Enabled = true;
            }

            public void Disable()
            {
                m_Enabled = false;
            }

            public void GainControl()
            {
                m_GettingInput = true;
            }

            public void ReleaseControl(bool resetValues)
            {
                m_GettingInput = false;
                if(resetValues)
                {
                    Value = 0;
                    ReceivingInput = false;
                }
            }

        }
    
        public InputType inputType = InputType.MouseAndKeyBoard;

        bool m_FixedUpdateHappeded;

        void Update()
        {
            GetInputs(m_FixedUpdateHappeded || Mathf.Approximately(Time.deltaTime, 0f));
            m_FixedUpdateHappeded = false;
        }

        void FixedUpdate()
        {
            m_FixedUpdateHappeded = true;
        }

        protected abstract void GetInputs(bool fixedUpdateHappened);

        public abstract void GainControl();
        public abstract void ReleaseControl(bool resetValue = true);

        protected void GainControl(InputButton inputButton)
        {
            inputButton.GainControl();
        }

        protected void GainControl(InputAxis inputAxis)
        {
            inputAxis.GainControl();
        }

        protected void ReleaseControl(InputButton inputButton, bool resetValues)
        {
            StartCoroutine(inputButton.ReleaseControl(resetValues));
        }

        protected void ReleaseControl(InputAxis inputAxis, bool resetValues)
        {
            inputAxis.ReleaseControl(resetValues);
        }
    
    }
    
}
