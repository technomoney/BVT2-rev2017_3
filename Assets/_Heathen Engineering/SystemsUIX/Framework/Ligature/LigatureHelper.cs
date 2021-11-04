using UnityEngine;

namespace HeathenEngineering.UIX
{
    [AddComponentMenu("Heathen/UIX/Ligature/Ligature Helper")]
    [RequireComponent(typeof(UnityEngine.UI.InputField))]
    public class LigatureHelper : MonoBehaviour
    {
        /// <summary>
        /// List of ligatures to test for
        /// </summary>
        public LigatureLibrary library;

        private UnityEngine.UI.InputField field;

        private void Start()
        {
            field = GetComponent<UnityEngine.UI.InputField>();
            field.onValueChanged.AddListener(ParseAll);
        }

        public void ParseAll(string value)
        {
            if (library == null)
                return;

            var r = library.ParseAll(value);
            if (value != r)
                field.text = r;
        }
    }
}