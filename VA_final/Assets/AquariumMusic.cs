using UnityEngine;
using System.Collections;

public class AquariumMusic : MonoBehaviour
{
	
	public AudioClip background;	// a clip of background music
	public AudioClip positive;		// a clip of music with positive feedback
	public AudioClip negative;		// a clip of music with negative feedback
	
	private new AudioSource audio;
	
	// Use this for initialization
	void Start ()
	{
		// set this class to incorporate the main camera's AudioSource
		audio = GetComponent<AudioSource>();
	}
	
	void Update ()
	{
		if (!audio.isPlaying) {
			PlayBackground ();
		}
	}
	
	public void PlayBackground()
	{
		MusicChanger (background, true, 1.0f);
	}
	
	public void PlayPositiveFeedback()
	{
		PlayFeedback (positive);
	}

	public void PlayNegativeFeedback()
	{
		PlayFeedback (negative);
	}

	private void PlayFeedback(AudioClip clip)
	{
		// if the clip is already playing, do nothing
		if (audio.clip == clip && audio.isPlaying) {
			return;
		}

		MusicChanger (clip, false, 0.75f);
	}

	// Changes the setting of the AudioSource to the given settings
	private void MusicChanger(AudioClip clip, bool loop, float volume)
	{
		audio.clip = clip;
		audio.loop = loop;
		audio.volume = volume;
		audio.Play ();
	}
}