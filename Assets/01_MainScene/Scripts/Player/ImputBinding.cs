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

        [Header("Tout les Boutons")]
        public GameObject ToucheOption;

        void Start()
        {
            string json = PlayerPrefs.GetString("inputs");
            Debug.Log(json);
            /*inputsDictionary = new Dictionary<string, char>();
            LoadDefaultInputs();*/
            /*if (!string.IsNullOrEmpty(json))
            {
                InputsData data = JsonUtility.FromJson<InputsData>(json);
                inputsDictionary = new Dictionary<string, char>();
                for (int i = 0; i < data.keys.Count; i++)
                {
                    inputsDictionary[data.keys[i]] = data.values[i];
                }
            }
            else
            {*/
                Debug.Log("No saved inputs found, using defaults.");
                inputsDictionary = new Dictionary<string, char>();
                LoadDefaultInputs();
           // }
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
                        SaveInputs();
                        UpdateUiButton();
                        return;
                    }
                }
            }

        }

        public void SaveInputs()
        {
            InputsData data = new InputsData();
            data.keys = new List<string>(inputsDictionary.Keys);
            data.values = new List<char>(inputsDictionary.Values);
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("inputs", json);
            PlayerPrefs.Save();
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
            }
        }

        private void UpdateUiButton()
        {
            this.ToucheOption.transform.Find("LacherButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["DropItem"]).ToString();
            //this.ToucheOption.transform.Find("CourirButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["Sprint"]).ToString();
            this.ToucheOption.transform.Find("EmoteButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["Emote"]).ToString();
            this.ToucheOption.transform.Find("InteractButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["Interact"]).ToString();
            this.ToucheOption.transform.Find("RealoadButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["RealoadWeapon"]).ToString();
            this.ToucheOption.transform.Find("OpenInvButton").Find("Text").GetComponent<TextMeshProUGUI>().text = ((KeyCode)inputsDictionary["ToggleInventory"]).ToString();
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