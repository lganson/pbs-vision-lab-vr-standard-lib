using System.Collections.Generic;
using Standard_Library;
using TMPro;
using UnityEngine;
namespace Main_Menu
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ExperimentDropdown : MonoBehaviour
    {
        private TMP_Dropdown dropdown;
        TaskSequence[] sequences;
        private readonly List<TaskSequence> filteredSequences = new List<TaskSequence>();
        public void Start() 
        {
            dropdown = GetComponent<TMP_Dropdown>();
            sequences = Resources.LoadAll<TaskSequence>("Task Sequences");
            //Can add any info about the experiment here. For example if we wanted to see if someone has already completed an experiment we could add a lookup and add a little checkmark
            
            foreach (var sequence in sequences)
            {
                #if UNITY_EDITOR
                filteredSequences.Add(sequence);
                #else
                if (!sequence.editorOnly)
                {
                    filteredSequences.Add(sequence);
                }
                #endif
            }

            foreach (var sequence in filteredSequences)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(sequence.sequenceName));
            }
            dropdown.RefreshShownValue();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            if(sequences.Length <= 0) throw new System.Exception("There are no task sequences.");
            OnDropdownValueChanged(dropdown.value);
        }

        public void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
        private void OnDropdownValueChanged(int value)
        {
            GlobalTaskSequencer.GetInstance().SetActiveSequence(filteredSequences[value]);
        }
    }
}