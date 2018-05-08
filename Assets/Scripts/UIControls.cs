﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyEditor {
	public class UIControls : MonoBehaviour {

		private Editor editorScript;

		// Level Settings
		public Button loadLevelBtn;
		public Button saveLevelBtn;
		public Button newLayerBtn;

		// Layer Settings 
		public InputField nameField;
		public Slider depthPositionSlider;
		public Slider parallaxWeightSlider;
		
		void Start()
		{
			editorScript = GetComponent<Editor>();
		}

		void Update()
		{
			// nameField.OnSubmit() {}
		}

	}
}

