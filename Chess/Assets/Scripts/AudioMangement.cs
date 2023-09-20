using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMangement : MonoBehaviour
{
    private AudioSource backGroundMusicAudioSource;
    
    public AudioClip backGroundMusic;
    private bool changingBetweenSongs = false;

    private void Awake()
    {
    }
    private void Start()
    {   
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public string getBackgroundMusicName()
    {
        return backGroundMusicAudioSource.clip.name;
    }

    public bool getChangingBetweenSongs()
    {
        return changingBetweenSongs;
    }
    public void setChangingBetweenSongs(bool value)
    {
        this.changingBetweenSongs = value;
    }

    public void decreaseBackgroundMusicVolume()
    {
        float volume = backGroundMusicAudioSource.volume;
        backGroundMusicAudioSource.volume = volume - 1;
    }
    
    public void increaseBackgroundMusicVolume()
    {
        float volume = backGroundMusicAudioSource.volume;
        backGroundMusicAudioSource.volume = volume + 1;
    }
    
    public void setBackgroundMusicVolume(float volume)
    {
        backGroundMusicAudioSource.volume = volume;
    }

    public void stopBackgroundMusic()
    {
        backGroundMusicAudioSource.Stop();
    }

    public void pauseBackgroundMusic()
    {
        backGroundMusicAudioSource.Pause();
    }

    public void resumeBackgroundMusic()
    {
        backGroundMusicAudioSource.UnPause();
    }

    public IEnumerator changeBackgroundMusic(string songName)
    {
        string path = "Audios/" + songName;
        float startVolume = backGroundMusicAudioSource.volume;
        float timer = 0f;
        float fadeTime = 5f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            backGroundMusicAudioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
            yield return null;
        }

        backGroundMusicAudioSource.volume = 0f;

        stopBackgroundMusic();
        //backGroundMusic = Resources.Load<AudioClip>(path);
        playBackgroundMusic(path);

        timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            backGroundMusicAudioSource.volume = Mathf.Lerp(0f, startVolume, timer / fadeTime);
            yield return null;
        }

        backGroundMusicAudioSource.volume = startVolume;
        
        setChangingBetweenSongs(false);
    }

    
    public void playBackgroundMusic(string path)
    {
        if (backGroundMusicAudioSource == null)
        {
            backGroundMusicAudioSource = gameObject.AddComponent<AudioSource>();
            backGroundMusicAudioSource.loop = true;
        }
        
        backGroundMusic = Resources.Load<AudioClip>(path);
        
        backGroundMusicAudioSource.clip = backGroundMusic;
        backGroundMusicAudioSource.Play();
    }
}
