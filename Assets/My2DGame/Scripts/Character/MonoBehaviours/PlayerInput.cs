using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    
    public class PlayerInput : InputComponent, IDataPersister
    {

        public static PlayerInput Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private static PlayerInput s_Instance;

        public bool HaveControl 
        {
            get
            {
                return m_HaveControl;
            }
        }

        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu);
        public InputButton Interact = new InputButton(KeyCode.E, XboxControllerButtons.Y);
        public InputButton MeleeAttack = new InputButton(KeyCode.K, XboxControllerButtons.X);
        public InputButton RangeAttack = new InputButton(KeyCode.O, XboxControllerButtons.B);
        public InputButton Jump = new InputButton(KeyCode.Space, XboxControllerButtons.A);
        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizonta);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical);
        [HideInInspector]
        public DataSetting dataSetting;

        protected bool m_HaveControl = true;
        protected bool m_DebugMenuIsOpen = false;

        void Awake()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
            }
        }

        void Onenable()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
            }
        }

        #region InputComponent 实现
        
        protected override void GetInputs(bool fixedUpdateHappened)
        {
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            MeleeAttack.Get(fixedUpdateHappened, inputType);
            RangeAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);

            Horizontal.Get(inputType);
            Vertical.Get(inputType);

            if(Input.GetKeyDown(KeyCode.F12))
            {
                m_DebugMenuIsOpen = !m_DebugMenuIsOpen;
            }
        }

        public override void GainControl()
        {
            m_HaveControl = true;

            GainControl(Pause);
            GainControl(Interact);
            GainControl(MeleeAttack);
            GainControl(RangeAttack);
            GainControl(Jump);
            GainControl(Horizontal);
            GainControl(Vertical);
        }

        public override void ReleaseControl(bool resetValue = true)
        {
            m_HaveControl = false;

            ReleaseControl(Pause, resetValue);
            ReleaseControl(Interact, resetValue);
            ReleaseControl(MeleeAttack, resetValue);
            ReleaseControl(RangeAttack, resetValue);
            ReleaseControl(Jump, resetValue);
            ReleaseControl(Horizontal, resetValue);
            ReleaseControl(Vertical, resetValue);
        }

        #endregion

        public void DisableMeleeAttacking()
        {
            MeleeAttack.Disable();
        }

        public void EnableMeleeAttacking()
        {
            MeleeAttack.Enable();
        }

        public void DisableRangeAttacking()
        {
            RangeAttack.Disable();
        }

        public void EnableRangeAttacking()
        {
            RangeAttack.Enable();
        }

        #region DataPersister 实现
        public DataSetting GetDataSetting()
        {
            return dataSetting;
        }

        public void SetDataSetting(string dataTag, DataSetting.PersistenceType persistenceType)
        {
            if(dataSetting == null)
            {
                dataSetting = new DataSetting();
            }

            dataSetting.dataTag = dataTag;
            dataSetting.persistenceType = persistenceType;
        }

        public void LoadData(Data data)
        {
            Data<bool, bool> playerInputData = (Data<bool, bool>)data;

            if (playerInputData.value0)
                MeleeAttack.Enable();
            else
                MeleeAttack.Disable();

            if (playerInputData.value1)
                RangeAttack.Enable();
            else
                RangeAttack.Disable();
        }

        public Data SaveData()
        {
            return new Data<bool, bool>(MeleeAttack.Enabled, RangeAttack.Enabled);
        }
        #endregion
    }

}