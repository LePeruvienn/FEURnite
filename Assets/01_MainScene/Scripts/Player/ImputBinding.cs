using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Starter.ThirdPersonCharacter
{
    public class InputBinding : MonoBehaviour
    {
        [SerializeField] private InputInfos[] baseInputs;
        private Dictionary<string, char> inputsDictionary;

        private string bindingAxis = "";

        [Header("Boutons Drop")]
        public GameObject ToucheOption;
        private bool DropItem;

        void Start()
        {
            inputsDictionary = new Dictionary<string, char>();
            LoadDefaultInputs();
            UpdateUiButton();
        }

        void Update()
        {
            if (!string.IsNullOrEmpty(bindingAxis) && Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(keyCode))
                    {
                        inputsDictionary[bindingAxis] = (char)keyCode;
                        bindingAxis = "";
                        UpdateUiButton();
                        return;
                    }
                }
            }

        }

        public void Bind(string axis)
        {
            bindingAxis = axis;
        }

        private void LoadDefaultInputs()
        {   
            foreach (InputInfos input in baseInputs)
            {
                inputsDictionary.Add(input.Name, input.Key);
                Debug.Log("Adding IMPUT ");
                Debug.Log(input.Name);
            }
            Debug.Log("inputsDictionary" + inputsDictionary["DropItem"]);
        }

        private void UpdateUiButton()
        {

            this.ToucheOption.transform.Find("DropItemButton").Find("DropItemText").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["DropItem"]).ToString();
        }

        public Dictionary<string, char> getInputDico()
        {
            Debug.Log(inputsDictionary + "c'est le get Dico");
            return inputsDictionary;
        }

        private class InputsData
        {
            public List<string> keys;
            public List<char> values;
        }
    }

    [System.Serializable]
    public struct InputInfos
    {
        [SerializeField] private string inputName;
        [SerializeField] private char key;

        public string Name => inputName;
        public char Key => key;
    }
}