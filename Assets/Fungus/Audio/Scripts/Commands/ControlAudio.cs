using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Audio",
	             "Plays, loops, or stops audio.")]
	public class ControlAudio : Command
	{
		public enum controlType
		{
			PlayOnce,
			PlayLoop,
			PauseLoop,
			StopLoop,
			ChangeVolume
		}
		[Tooltip("What to do to audio")]
		public controlType control;

		[Tooltip("Audio clip to play")]
		public AudioSource audioSource;

		[Range(0,1)]
		[Tooltip("Start audio at this volume")]
		public float startVolume = 1;

		[Range(0,1)]
		[Tooltip("End audio at this volume")]
		public float endVolume = 1;

		[Range(0,30)]
		[Tooltip("Time to fade between current volume level and target volume level.")]
		public float fadeDuration; 

		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			audioSource.volume = endVolume;
			switch(control)
			{
				case controlType.PlayOnce:
					if (fadeDuration > 0)
					{
					    audioSource.volume = 0;
						PlaySoundWithCallback(audioSource.audio.clip, endVolume, AudioFinished);
						LeanTween.value(audioSource.gameObject,0,endVolume,fadeDuration).setOnUpdate(
							(float updateVolume)=>{
								audioSource.volume = updateVolume;
							}
						).setOnComplete(
							()=>{
							if (waitUntilFinished)
							{
								Continue();
							}
						}
						);
					}
					else
					{
						audioSource.volume = 1;
						PlaySoundWithCallback(audioSource.audio.clip, endVolume, AudioFinished);
					}
					break;
				case controlType.PlayLoop:
	
					if (fadeDuration > 0)
					{
					    audioSource.volume = 0;
						audioSource.loop = true;
						audioSource.audio.Play();
						LeanTween.value(audioSource.gameObject,0,endVolume,fadeDuration).setOnUpdate(
							(float updateVolume)=>{
								audioSource.volume = updateVolume;
							}
						).setOnComplete(
							()=>{
							if (waitUntilFinished)
							{
								Continue();
							}
						}
						);
					}
				    else
					{
						audioSource.volume = 1;
						audioSource.loop = true;
						audioSource.audio.Play();
					}
					break;
			case controlType.PauseLoop:
				if (fadeDuration > 0)
				{
					LeanTween.value(audioSource.gameObject,audioSource.volume,0,fadeDuration).setOnUpdate(
						(float updateVolume)=>{
						audioSource.volume = updateVolume;
					}
					).setOnComplete(
						()=>{
						
						audioSource.audio.Pause();
						if (waitUntilFinished)
						{
							Continue();
						}
					}
					);
				}
				else
				{
					audioSource.audio.Pause();
				}
				break;
			case controlType.StopLoop:
					if (fadeDuration > 0)
					{
						LeanTween.value(audioSource.gameObject,audioSource.volume,0,fadeDuration).setOnUpdate(
							(float updateVolume)=>{
								audioSource.volume = updateVolume;
							}
						).setOnComplete(
							()=>{

								audioSource.audio.Stop();
								if (waitUntilFinished)
								{
									Continue();
								}
							}
						);
					}
					else
					{
						audioSource.audio.Stop();
					}
					break;
				case controlType.ChangeVolume:
					LeanTween.value(audioSource.gameObject,audioSource.volume,endVolume,fadeDuration).setOnUpdate(
						(float updateVolume)=>{
						audioSource.volume = updateVolume;
					}
					);
					break;
			}
			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		public delegate void AudioCallback();
		public void PlaySoundWithCallback(AudioClip clip, float endVolume, AudioCallback callback)
		{
			audioSource.audio.PlayOneShot(audioSource.clip, endVolume);
			StartCoroutine(DelayedCallback(audioSource.clip.length, callback));
		}
		private IEnumerator DelayedCallback(float time, AudioCallback callback)
		{
			yield return new WaitForSeconds(time);
			callback();
		}
		void AudioFinished()
		{
			Continue();
		}
		
		public override string GetSummary()
		{
			if (audioSource == null)
			{
				return "Error: No sound clip selected";
			}
			string fadeType = "";
			if (fadeDuration > 0)
			{
				fadeType = " Fade out";
				if (control != controlType.StopLoop)
				{
					fadeType = " Fade in volume to " + endVolume;
				}
				if (control == controlType.ChangeVolume)
				{
					fadeType = " to " + endVolume;
				}
				fadeType += " over " + fadeDuration + " seconds.";
			}
			return control.ToString() + " \"" + audioSource.name + "\"" + fadeType;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}
	
}