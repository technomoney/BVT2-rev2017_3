using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    [System.Serializable]
    public struct HotKeySet
    {
        public bool shift;
        [UnityEngine.Serialization.FormerlySerializedAs("cntrl")]
        public bool ctrl;
        public bool alt;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        public bool cmd;
#endif
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		public bool win;
#endif
        public KeyCode keyCode;

        public bool MatchesInput()
        {
            return Input.GetKeyDown(keyCode) &&
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                        (!cmd || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) &&
#endif
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				        (!win || Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows)) &&
#endif
                        (!shift || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                        (!ctrl || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                        (!alt || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));

        }

        public bool MatchesEvent()
        {
            return Event.current.keyCode == keyCode &&
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                        (!cmd || Event.current.command) &&
#endif
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				        (!win || Event.current.command) &&
#endif
                        (!shift || Event.current.shift) &&
                        (!ctrl || Event.current.control) &&
                        (!alt || Event.current.alt);
        }
    }
}