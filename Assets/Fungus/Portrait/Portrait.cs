using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public struct portraitState
	{
		public bool onScreen;
		public bool dimmed;
		public displayType display;
		public Sprite portrait;
		public RectTransform position;
		public facingDirection facing;
		public GameObject portraitObj;
		public Image portraitImage;
	}

	public enum displayType
	{
		NULL,
		Show,
		Hide,
		Swap,
		MoveToFront
	}
	public enum facingDirection
	{
		NULL,
		Left,
		Right
	}

	public enum positionOffset
	{
		NULL,
		OffsetLeft,
		OffsetRight
	}

	[CommandInfo("Portrait", 
	             "Portrait", 
	             "Controls a character portrait. " +
	             "Select [Game Object > Fungus > Dialog > Character] to create a new selectable speaking character.")]
	public class Portrait : Command 
	{
		[Tooltip("Display type")]
		public PortraitStage portraitStage;

		[Tooltip("Display type")]
		public displayType display;
		
		[Tooltip("Character to display")]
		public Character character;

		[Tooltip("Character to swap with")]
		public Character replacedCharacter;
		
		[Tooltip("Portrait to display")]
		public Sprite portrait;

		[Tooltip("Move the portrait from/to this offset position")]
		public positionOffset offset;

		[Tooltip("Move the portrait from this position")]
		public RectTransform fromPosition;
		protected RectTransform fromPositionition;

		[Tooltip("Move the portrait to this positoin")]
		public RectTransform toPosition;
		protected RectTransform toPositionition;
		
		[Tooltip("Direction character is facing")]
		public facingDirection facing;

		[Tooltip("Use Default Settings")]
		public bool useDefaultSettings = true;
	
		[Tooltip("Fade Duration")]
		public float fadeDuration;

		[Tooltip("Movement Speed")]
		public float moveSpeed;

		[Tooltip("Shift Offset")]
		public Vector2 shiftOffset;

		[Tooltip("Move")]
		public bool move;

		[Tooltip("Start from offset")]
		public bool shiftIntoPlace;
		
		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			// If no display specified, do nothing
			if (display == displayType.NULL)
			{
				Continue();
				return;
			}
			// If no character specified, do nothing
			if (character == null)
			{
				Continue();
				return;
			}
			// If Swap and no replaced character specified, do nothing
			if (display == displayType.Swap && replacedCharacter == null)
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

			if (character.state.portraitImage == null)
			{
				CreatePortraitObject(character,portraitStage);
			}
			// if no previous portrait, use default portrait
			if (character.state.portrait == null) 
			{
				character.state.portrait = character.profileSprite;
			}
			// Selected "use previous portrait"
			if (portrait == null) 
			{
				portrait = character.state.portrait;
			}
			// if no previous position, use default position
			if (character.state.position == null)
			{
				character.state.position = portraitStage.defaultPosition.rectTransform;
			}
			// Selected "use previous position"
			if (toPosition == null)
			{
				toPosition = character.state.position;
			}
			if (replacedCharacter != null)
			{
				// if no previous position, use default position
				if (replacedCharacter.state.position == null)
				{
					replacedCharacter.state.position = portraitStage.defaultPosition.rectTransform;
				}
			}
			// If swapping, use replaced character's position
			if (display == displayType.Swap)
			{
				toPosition = replacedCharacter.state.position;
			}
			// Selected "use previous position"
			if (fromPosition == null)
			{
				fromPosition = character.state.position;
			}
			// if portrait not moving, use from position is same as to position
			if (!move)
			{
				fromPosition = toPosition;
			}
			if (display == displayType.Hide)
			{
				fromPosition = character.state.position;
			}
			// if no previous facing direction, use default facing direction
			if (character.state.facing == facingDirection.NULL) 
			{
				character.state.facing = character.portraitsFace;
			}
			// Selected "use previous facing direction"
			if (facing == facingDirection.NULL)
			{
				facing = character.state.facing;
			}
			// Use default settings
			if (useDefaultSettings)
			{
				fadeDuration = portraitStage.fadeDuration;
				moveSpeed = portraitStage.moveSpeed;
				shiftOffset = portraitStage.shiftOffset;
			}
			switch(display)
			{
				case (displayType.Show):
					Show(character,fromPosition,toPosition);
					character.state.onScreen = true;
					portraitStage.charactersOnStage.Add(character);
					break;
				case (displayType.Hide):
					Hide(character,fromPosition,toPosition);
					character.state.onScreen = false;
					portraitStage.charactersOnStage.Remove(character);
					break;
				case (displayType.Swap):
					Show(character,fromPosition,toPosition);
					Hide(replacedCharacter, replacedCharacter.state.position, replacedCharacter.state.position);
					character.state.onScreen = true;
					replacedCharacter.state.onScreen = false;
					portraitStage.charactersOnStage.Add(character);
					portraitStage.charactersOnStage.Remove(replacedCharacter);
					break;
				case (displayType.MoveToFront):
					MoveToFront(character);
					break;
			}

			if (display == displayType.Swap)
			{
				character.state.display = displayType.Show;
				replacedCharacter.state.display = displayType.Hide;
			}
			else
			{
				character.state.display = display;
			}
			character.state.portrait = portrait;
			character.state.facing = facing;
			character.state.position = toPosition;
			if (!waitUntilFinished)
			{
				Continue();
			}
		}
		public static void CreatePortraitObject(Character character, PortraitStage portraitStage)
		{
			GameObject portraitObj = new GameObject(character.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			portraitObj.transform.SetParent(portraitStage.portraitCanvas.transform, true);
			Image portraitImage = portraitObj.GetComponent<Image>();
			portraitImage.preserveAspect = true;
			portraitImage.sprite = character.profileSprite;
			Material portraitMaterial = Instantiate(Resources.Load("Portrait")) as Material;
			portraitImage.material = portraitMaterial;
			character.state.portraitObj = portraitObj;
			character.state.portraitImage = portraitImage;
			character.state.portraitImage.material.SetFloat("_Alpha",0);
		}
		protected void SetupPortrait(Character character, RectTransform fromPosition)
		{
			SetRectTransform(character.state.portraitImage.rectTransform, fromPosition);
			character.state.portraitImage.material.SetFloat("_Fade",0);
			character.state.portraitImage.material.SetTexture("_MainTex", character.profileSprite.texture);
			Texture2D blankTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			blankTexture.SetPixel(0, 0, new Color(0f,0f,0f,0f));
			blankTexture.Apply();
			character.state.portraitImage.material.SetTexture("_TexStart", blankTexture as Texture);
			character.state.portraitImage.material.SetTexture("_TexEnd", blankTexture as Texture);
			if (character.state.facing != character.portraitsFace)
			{
				character.state.portraitImage.material.SetFloat("_FlipStart",1);
			}
			else
			{
				character.state.portraitImage.material.SetFloat("_FlipStart",0);
			}
			if (facing != character.portraitsFace)
			{
				character.state.portraitImage.material.SetFloat("_FlipEnd",1);
			}
			else
			{
				character.state.portraitImage.material.SetFloat("_FlipEnd",0);
			}
			character.state.portraitImage.material.SetFloat("_Alpha",1);
		}
		public static void SetRectTransform(RectTransform oldRectTransform, RectTransform newRectTransform)
		{
			oldRectTransform.eulerAngles      = newRectTransform.eulerAngles;
			oldRectTransform.position         = newRectTransform.position;
			oldRectTransform.rotation         = newRectTransform.rotation;
			oldRectTransform.anchoredPosition = newRectTransform.anchoredPosition;
			oldRectTransform.sizeDelta        = newRectTransform.sizeDelta;
			oldRectTransform.anchorMax        = newRectTransform.anchorMax;
			oldRectTransform.anchorMin        = newRectTransform.anchorMin;
			oldRectTransform.pivot            = newRectTransform.pivot;
			oldRectTransform.localScale       = newRectTransform.localScale;
		}
		protected void Show(Character character, RectTransform fromPosition, RectTransform toPosition) 
		{
			if (shiftIntoPlace)
			{
				fromPosition = Instantiate(toPosition) as RectTransform;
				if (offset == positionOffset.OffsetLeft)
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x-Mathf.Abs(shiftOffset.x), fromPosition.anchoredPosition.y-Mathf.Abs(shiftOffset.y));
				}
				else if (offset == positionOffset.OffsetRight)
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x+Mathf.Abs(shiftOffset.x), fromPosition.anchoredPosition.y+Mathf.Abs(shiftOffset.y));
				}
				else
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x, fromPosition.anchoredPosition.y);
				}
			}
			SetupPortrait(character, fromPosition);
			if (character.state.display != displayType.NULL && character.state.display != displayType.Hide)
			{
				character.state.portraitImage.material.SetTexture("_TexStart", character.state.portrait.texture);
			}
			character.state.portraitImage.material.SetTexture("_TexEnd", portrait.texture);
			UpdateTweens(character, fromPosition, toPosition);
		}
		protected void Hide(Character character, RectTransform fromPosition, RectTransform toPosition)
		{
			if (character.state.display == displayType.NULL)
			{
				return;
			}
			SetupPortrait(character, fromPosition);
			character.state.portraitImage.material.SetTexture("_TexStart", character.state.portrait.texture);
			UpdateTweens(character, fromPosition, toPosition);
		}
		protected void MoveToFront(Character character)
		{
			character.state.portraitImage.transform.SetSiblingIndex(character.state.portraitImage.transform.parent.childCount);
		}
		protected void UpdateTweens(Character character, RectTransform fromPosition, RectTransform toPosition) 
		{
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(character.state.portraitObj,0,1,fadeDuration).setEase(portraitStage.fadeEaseType).setOnComplete(OnComplete).setOnUpdate(
				(float fadeAmount)=>{
					character.state.portraitImage.material.SetFloat("_Fade", fadeAmount);
				}
			);
			float moveDuration = (Vector3.Distance(fromPosition.anchoredPosition,toPosition.anchoredPosition)/moveSpeed);
			if (moveSpeed == 0) moveDuration = float.Epsilon;
			LeanTween.value(character.state.portraitObj,fromPosition.anchoredPosition,toPosition.anchoredPosition,moveDuration).setEase(portraitStage.moveEaseType).setOnComplete(OnComplete).setOnUpdate(
				(Vector3 updatePosition)=>{
					character.state.portraitImage.rectTransform.anchoredPosition = updatePosition;
				}
			);
		}
		public static void Dim(Character character, PortraitStage portraitStage)
		{
			if (character.state.dimmed == false)
			{
				character.state.dimmed = true;
				float fadeDuration = portraitStage.fadeDuration;
				if (fadeDuration == 0) fadeDuration = float.Epsilon;
				LeanTween.value(character.state.portraitObj,1f,0.5f,fadeDuration).setEase(portraitStage.fadeEaseType).setOnUpdate(
					(float tintAmount)=>{
						Color tint = new Color(tintAmount,tintAmount,tintAmount,1);
						character.state.portraitImage.material.SetColor("_Color", tint);
					}
				);
			}
		}
		public static void Undim(Character character, PortraitStage portraitStage)
		{
			if (character.state.dimmed == true)
			{
				character.state.dimmed = false;
				float fadeDuration = portraitStage.fadeDuration;
				if (fadeDuration == 0) fadeDuration = float.Epsilon;
				LeanTween.value(character.state.portraitObj,0.5f,1f,fadeDuration).setEase(portraitStage.fadeEaseType).setOnUpdate(
					(float tintAmount)=>{
						Color tint = new Color(tintAmount,tintAmount,tintAmount,1);
						character.state.portraitImage.material.SetColor("_Color", tint);
					}
				);
			}
		}
		protected void OnComplete() 
		{
			if (waitUntilFinished)
			{
				if (!LeanTween.isTweening (character.state.portraitObj))
				{
					Continue();
				}
			}
		}
		public override string GetSummary()
		{
			if (display == displayType.NULL && character == null)
			{
				return "Error: No character or display selected";
			}
			else if (display == displayType.NULL)
			{
				return "Error: No display selected";
			}
			else if (character == null)
			{
				return "Error: No character selected";
			}
			string displaySummary = "";
			string characterSummary = "";
			string fromPositionSummary = "";
			string toPositionSummary = "";
			string portraitStageSummary = "";
			string portraitSummary = "";
			string facingSummary = "";

			displaySummary = StringFormatter.splitCamelCase(display.ToString());

			if (display == displayType.Swap)
			{
				if (replacedCharacter != null)
				{
					displaySummary += " \"" + replacedCharacter.name + "\" with";
				}
			}
			characterSummary = character.name;
			if (portraitStage != null)
			{
				portraitStageSummary = " on \"" + portraitStage.name + "\"";
			}
			
			if (portrait != null)
			{
				portraitSummary = " " + portrait.name;
			}
			if (shiftIntoPlace)
			{
				if (offset != 0)
				{
					fromPositionSummary = offset.ToString();
					fromPositionSummary = " from " + "\"" + fromPositionSummary + "\"";
				}
			}
			else if (fromPosition != null)
			{
				fromPositionSummary = " from " + "\"" + fromPosition.name + "\"";
			}
			if (toPosition != null)
			{
				string toPositionPrefixSummary = "";
				if (move)
					toPositionPrefixSummary = " to ";
				else
					toPositionPrefixSummary = " at ";
				toPositionSummary = toPositionPrefixSummary + "\"" + toPosition.name + "\"";
			}
			if (facing != facingDirection.NULL)
			{
				if ( facing == facingDirection.Left )
				{
					facingSummary = "<--";
				}
				if ( facing == facingDirection.Right )
				{
					facingSummary = "-->";
				}
				facingSummary = " facing \"" + facingSummary + "\"";
			}
			return displaySummary + " \"" + characterSummary + portraitSummary + "\"" + portraitStageSummary + facingSummary + fromPositionSummary + toPositionSummary;
		}

		public override Color GetButtonColor()
		{
			return new Color32(230, 200, 250, 255);
		}

		public override void OnCommandAdded(Sequence parentSequence)
		{
			//Default to display type: show
			display = displayType.Show;
		}
	}
}