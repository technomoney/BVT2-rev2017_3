using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using HeathenEngineering.Events;
using HeathenEngineering.Scriptable;

namespace HeathenEngineering.UIX
{
    [AddComponentMenu("Heathen/UIX/Keyboard/Keyboard")]
    public class Keyboard : MonoBehaviour
    {
        //public bool autoLinkHID = false;
        //public bool autoTargetInputfields = true;
        [ShowOnly]
        public KeyboardState state = KeyboardState.Normal;
        //public KeyboardKey keyPrototype;
        //public RectTransform keyContainer;
        //public Serialization.KeyboardTemplate workingTemplate;
        //public Serialization.KeyboardTemplate selectedTemplate;
        [HideInInspector]
        public List<KeyboardKey> keys = new List<KeyboardKey>();
        //public RectTransform headerRowTransform;
        //public List<RectTransform> rowTransforms;
        //private GameObject previousLinkedGameObject;
        //public GameObject linkedGameObject;
        //public List<Component> linkedBehaviours;
        //public Component linkedBehaviour;
        //public List<string> fields;
        //public string field;
        public event RoutedEvent<KeyboardKey> KeyboardKeyPressed;
        /// <summary>
        /// The point at which the character will be inserted; set to -1 to ignore thus always entering at the end
        /// This value will automaticly advance unless set to -1
        /// </summary>
        //public int insertPoint = -1;
        //public int selectionLength = 0;
        [HideInInspector]
        public KeyboardKey ActiveKey;
        private KeyboardState statePrevious = KeyboardState.ShiftedAltGr;
        //private UnityEngine.UI.InputField lastInputField;
        //private UnityEngine.UI.InputField _inputField;

        // Use this for initialization
        void Start()
        {
            UpdateKeyLinks();
        }

        void Update()
        {
            if (AltGrKeysHeld.Count > 0 || (useAltGrToggle && altGrToggle))
            {
                if ((ShiftKeysHeld.Count > 0 || (useShiftToggle && shiftToggle)) || capsLockToggle)
                    state = KeyboardState.ShiftedAltGr;
                else
                    state = KeyboardState.AltGr;
            }
            else if ((ShiftKeysHeld.Count > 0 || (useShiftToggle && shiftToggle)) || capsLockToggle)
                state = KeyboardState.Shifted;
            else
                state = KeyboardState.Normal;
            
            if (statePrevious != state)
                UpdateState();
        }

        ///// <summary>
        ///// Targets the indicated input field for the keyboard. This will also 'select' the InputField in Unity UI
        ///// </summary>
        ///// <param name="target"></param>
        //public void SetInputTarget(UnityEngine.UI.InputField target)
        //{
        //    _inputField = target;

        //    if (_inputField != null)
        //    {
        //        _inputField.Select();
        //        lastInputField = _inputField;
        //        if (_inputField.selectionAnchorPosition > _inputField.selectionFocusPosition)
        //        {
        //            insertPoint = _inputField.selectionFocusPosition;
        //            selectionLength = _inputField.selectionAnchorPosition - _inputField.selectionFocusPosition;
        //        }
        //        else if (_inputField.selectionFocusPosition > _inputField.selectionAnchorPosition)
        //        {
        //            insertPoint = _inputField.selectionAnchorPosition;
        //            selectionLength = _inputField.selectionFocusPosition - _inputField.selectionAnchorPosition;
        //        }
        //        else
        //        {
        //            insertPoint = _inputField.selectionAnchorPosition;
        //            selectionLength = 0;
        //        }
        //    }

        //    if (_inputField != null && EventSystem.current.currentSelectedGameObject != linkedGameObject)
        //    {
        //        lastInputField = _inputField;
        //        linkedGameObject = EventSystem.current.currentSelectedGameObject;
        //        linkedBehaviour = lastInputField;
        //        field = "text";
        //    }



        //    //We know the input field, we know it is active and we are supposed to track point
        //    //if (lastInputField != null && EventSystem.current.currentSelectedGameObject == linkedGameObject && insertPoint > -1)
        //    //    insertPoint = lastInputField.caretPosition;
        //}

        /// <summary>
        /// Registeres a KeybaordKey to the keyboard allowing events to be tracked for the key
        /// </summary>
        /// <param name="key"></param>
        public void RegisterKey(KeyboardKey key)
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);
                key.keyboard = this;
                key.pressed += keyPressed;
                key.isDown += keyIsDown;
                key.isUp += keyIsUp;
            }
        }

        /// <summary>
        /// Updates the keyboards key links re-registering all keys
        /// </summary>
        public void UpdateKeyLinks()
        {
            foreach (KeyboardKey key in keys)
            {
                key.pressed -= keyPressed;
            }

            keys.Clear();

            foreach (KeyboardKey key in GetComponentsInChildren<KeyboardKey>())
            {
                RegisterKey(key);
            }
        }

        /// <summary>
        /// Updates the keybaords modifier key status
        /// </summary>
        private void UpdateState()
        {
            statePrevious = state;
            switch (state)
            {
                case KeyboardState.Normal:
                    foreach (KeyboardKey key in keys)
                    {
                        if (key.keyGlyph.normal != null)
                            key.keyGlyph.normal.gameObject.SetActive(true);
                        if (key.keyGlyph.shifted != null)
                            key.keyGlyph.shifted.gameObject.SetActive(false);
                        if (key.keyGlyph.altGr != null)
                            key.keyGlyph.altGr.gameObject.SetActive(false);
                        if (key.keyGlyph.shiftedAltGr != null)
                            key.keyGlyph.shiftedAltGr.gameObject.SetActive(false);
                    }
                    break;
                case KeyboardState.Shifted:
                    foreach (KeyboardKey key in keys)
                    {
                        if (key.keyGlyph.normal != null)
                            key.keyGlyph.normal.gameObject.SetActive(false);
                        if (key.keyGlyph.shifted != null)
                            key.keyGlyph.shifted.gameObject.SetActive(true);
                        if (key.keyGlyph.altGr != null)
                            key.keyGlyph.altGr.gameObject.SetActive(false);
                        if (key.keyGlyph.shiftedAltGr != null)
                            key.keyGlyph.shiftedAltGr.gameObject.SetActive(false);
                    }
                    break;
                case KeyboardState.AltGr:
                    foreach (KeyboardKey key in keys)
                    {
                        if (key.keyGlyph.normal != null)
                            key.keyGlyph.normal.gameObject.SetActive(false);
                        if (key.keyGlyph.shifted != null)
                            key.keyGlyph.shifted.gameObject.SetActive(false);
                        if (key.keyGlyph.altGr != null)
                            key.keyGlyph.altGr.gameObject.SetActive(true);
                        if (key.keyGlyph.shiftedAltGr != null)
                            key.keyGlyph.shiftedAltGr.gameObject.SetActive(false);
                    }
                    break;
                case KeyboardState.ShiftedAltGr:
                    foreach (KeyboardKey key in keys)
                    {
                        if (key.keyGlyph.normal != null)
                            key.keyGlyph.normal.gameObject.SetActive(false);
                        if (key.keyGlyph.shifted != null)
                            key.keyGlyph.shifted.gameObject.SetActive(false);
                        if (key.keyGlyph.altGr != null)
                            key.keyGlyph.altGr.gameObject.SetActive(false);
                        if (key.keyGlyph.shiftedAltGr != null)
                            key.keyGlyph.shiftedAltGr.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        /// <summary>
        /// Press the current active key
        /// </summary>
        public void PressKey()
        {
            if (ActiveKey != null)
                ActiveKey.Press();
        }
        /// <summary>
        /// Activate the selected key and press it
        /// </summary>
        /// <param name="key"></param>
        public void PressKey(KeyboardKey key)
        {
            if (key != null)
            {
                EventSystem.current.SetSelectedGameObject(key.gameObject);
                ActiveKey = key;
                key.Press();
            }
        }
        /// <summary>
        /// Find the key with the matching code and press it
        /// </summary>
        /// <param name="code"></param>
        public void PressKey(KeyCode code)
        {
            foreach (KeyboardKey key in keys)
            {
                if (key.keyGlyph.code == code)
                {
                    PressKey(key);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Occures when a key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        public void keyPressed(object sender, KeyboardKey key)
        {
            if (key.keyGlyph.code == KeyCode.CapsLock)
                capsLockToggle = !capsLockToggle;

            if (useShiftToggle &&
                (key.keyGlyph.code == KeyCode.LeftShift || key.keyGlyph.code == KeyCode.RightShift))
                shiftToggle = !shiftToggle;

            if (useAltGrToggle &&
                (key.keyGlyph.code == KeyCode.AltGr))
                altGrToggle = !altGrToggle;

            if (KeyboardKeyPressed != null)
                KeyboardKeyPressed(sender, key);
        }

        #region Multi-touch code
        private bool capsLockToggle = false;
        [Tooltip("If off shift must be pressed and held to modify output. If on shift will behave like a toggle switch")]
        public BoolReference useShiftToggle = new BoolReference(false);
        private bool shiftToggle = false;
        [Tooltip("If off Alt Gr must be pressed and held to modify output. If on Alt Gr will behave like a toggle switch")]
        public BoolReference useAltGrToggle = new BoolReference(false);
        private bool altGrToggle = false;
        private List<KeyboardKey> AltGrKeysHeld = new List<KeyboardKey>();
        private List<KeyboardKey> ShiftKeysHeld = new List<KeyboardKey>();

        /// <summary>
        /// Occures when a key is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        private void keyIsDown(object sender, KeyboardKey key)
        {
            //For modifiers consider a down movement a press if the board is not in this state
            if (key.keyType == KeyboardKeyType.Modifier)
            {
                //If we arent in AltGr mode for the AltGr key then press AltGr
                if (key.keyGlyph.code == KeyCode.AltGr && !AltGrKeysHeld.Contains(key))
                {
                    AltGrKeysHeld.Add(key);
                }
                else if ((key.keyGlyph.code == KeyCode.LeftShift || key.keyGlyph.code == KeyCode.RightShift) && !ShiftKeysHeld.Contains(key))
                {
                    ShiftKeysHeld.Add(key);
                }
            }
        }

        /// <summary>
        /// Occures when a key was down and is released 'up'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        private void keyIsUp(object sender, KeyboardKey key)
        {
            //For modifiers consider a down movement a press if the board is not in this state
            if (key.keyType == KeyboardKeyType.Modifier)
            {
                //If we arent in AltGr mode for the AltGr key then press AltGr
                if (key.keyGlyph.code == KeyCode.AltGr && AltGrKeysHeld.Contains(key))
                {
                    AltGrKeysHeld.Remove(key);
                }
                else if ((key.keyGlyph.code == KeyCode.LeftShift || key.keyGlyph.code == KeyCode.RightShift) && ShiftKeysHeld.Contains(key))
                {
                    ShiftKeysHeld.Remove(key);
                }
            }
        }
        #endregion
    }
}
