using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[ExecuteInEditMode]
	public class Audio : MonoBehaviour 
	{	
		[HideInInspector]
		public AudioSource audioSource;
		static public List<Audio> activeAudio = new List<Audio>();
		
		protected virtual void OnEnable()
		{
			audioSource = gameObject.GetComponent<AudioSource>();
			if (!activeAudio.Contains(this))
			{
				activeAudio.Add(this);
			}
		}
		
		protected virtual void OnDisable()
		{
			activeAudio.Remove(this);
		}
	}
	
}