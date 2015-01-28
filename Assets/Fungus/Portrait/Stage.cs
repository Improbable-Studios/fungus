using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public enum stageDisplayType
	{
		NULL,
		Show,
		Hide,
		Swap,
		MoveToFront,
		UndimAllPortraits,
		DimNonSpeakingPortraits
	}

	[CommandInfo("Portrait", 
	             "Stage",
	             "Controls the portrait stage. " +
	             "Select [Game Object > Fungus > Portrait > PortraitStage] to create a new PortraitStage.")]
	public class Stage : Command 
	{	
		[Tooltip("Display type")]
		public stageDisplayType display;

		[Tooltip("Stage to display characters on")]
		public PortraitStage portraitStage;

		[Tooltip("PortraitStage to swap with")]
		public PortraitStage replacedPortraitStage;

		[Tooltip("Use Default Settings")]
		public bool useDefaultSettings = true;

		[Tooltip("Fade Duration")]
		public float fadeDuration;
		
		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			// If no display specified, do nothing
			if (display == stageDisplayType.NULL)
			{
				Continue();
				return;
			}
			// Selected "use default Portrait Stage"
			if (portraitStage == null)            // Default portrait stage selected
			{
				portraitStage = GetFungusScript().defaultPortraitStage;;  // Try to get game's default portrait stage
				if (portraitStage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					portraitStage = GameObject.FindObjectOfType<PortraitStage>();
				}
			}
			// If portrait stage does not exist, do nothing
			if (portraitStage == null)
			{
				Continue();
				return;
			}
			// Selected "use default Portrait Stage"
			if (display == stageDisplayType.Swap)            // Default portrait stage selected
			{
				replacedPortraitStage = GetFungusScript().defaultPortraitStage;;  // Try to get game's default portrait stage
				if (replacedPortraitStage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					replacedPortraitStage = GameObject.FindObjectOfType<PortraitStage>();
				}
				// If portrait stage does not exist, do nothing
				if (replacedPortraitStage == null)
				{
					Continue();
					return;
				}
			}
			// Use default settings
			if (useDefaultSettings)
			{
				fadeDuration = portraitStage.fadeDuration;
			}
			switch(display)
			{
			case (stageDisplayType.Show):
				Show(portraitStage);
				break;
			case (stageDisplayType.Hide):
				Hide(portraitStage);
				break;
			case (stageDisplayType.Swap):
				Show(portraitStage);
				Hide(replacedPortraitStage);
				break;
			case (stageDisplayType.MoveToFront):
				MoveToFront(portraitStage);
				break;
			case (stageDisplayType.UndimAllPortraits):
				UndimAllPortraits(portraitStage);
				break;
			case (stageDisplayType.DimNonSpeakingPortraits):
				DimNonSpeakingPortraits(portraitStage);
				break;
			}

			if (!waitUntilFinished)
			{
				Continue();
			}
		}
		protected void Show(PortraitStage portraitStage) 
		{
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(gameObject,0,1,fadeDuration).setOnUpdate(
				(float fadeAmount)=>{
				foreach ( Character c in portraitStage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",fadeAmount);
				}
			}
			).setOnComplete(
				()=>{
				foreach ( Character c in portraitStage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",1);
				}
				OnComplete();
			}
			);
		}
		protected void Hide(PortraitStage portraitStage)
		{
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(gameObject,1,0,fadeDuration).setOnUpdate(
				(float fadeAmount)=>{
				foreach ( Character c in portraitStage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",fadeAmount);
				}
			}
			).setOnComplete(
				()=>{
				foreach ( Character c in portraitStage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",0);
				}
				OnComplete();
			}
			);
		}
		protected void MoveToFront(PortraitStage portraitStage)
		{
			foreach (PortraitStage ps in PortraitStage.activePortraitStages)
			{
				if (ps == portraitStage)
				{
					ps.portraitCanvas.sortingOrder = 1;
				}
				else
				{
					ps.portraitCanvas.sortingOrder = 0;
				}
			}
		}
		protected void UndimAllPortraits(PortraitStage portraitStage) 
		{
			portraitStage.dimPortraits = false;
			foreach (Character character in portraitStage.charactersOnStage)
			{
				Portrait.Undim(character,portraitStage);
			}
		}
		protected void DimNonSpeakingPortraits(PortraitStage portraitStage) 
		{
			portraitStage.dimPortraits = true;
		}
		protected void OnComplete() 
		{
			if (waitUntilFinished)
			{
				Continue();
			}
		}
		public override string GetSummary()
		{
			string displaySummary = "";
			if (display != stageDisplayType.NULL)
			{
				displaySummary = StringFormatter.splitCamelCase(display.ToString());
			}
			else
			{
				return "Error: No display selected";
			}
			string portraitStageSummary = "";
			if (portraitStage != null)
			{
				portraitStageSummary = " \"" + portraitStage.name + "\"";
			}
			return displaySummary + portraitStageSummary;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(230, 200, 250, 255);
		}

		public override void OnCommandAdded(Sequence parentSequence)
		{
			//Default to display type: show
			display = stageDisplayType.Show;
		}
	}
}