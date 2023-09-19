using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMangement : MonoBehaviour
{
    private AudioSource backGroundMusicAudioSource;
    
    public AudioClip backGroundMusic;

    private void Awake()
    {
        backGroundMusicAudioSource = gameObject.AddComponent<AudioSource>();
        
        backGroundMusic = Resources.Load<AudioClip>("Audios/pianomoment");
        
        backGroundMusicAudioSource.clip = backGroundMusic;

        backGroundMusicAudioSource.Play();
    }
    private void Start()
    {
        // irei remover essa musica tocando aqui para criar a logica toda no script ChessBoard        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public string getBackgroundMusicName()
    {
        return backGroundMusicAudioSource.clip.name;
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

    public void changeBackgroundMusic(string songName)
    { 
        string path = "Audios/" + songName;

        stopBackgroundMusic();
        backGroundMusic = Resources.Load<AudioClip>(path);
        playBackgroundMusic(backGroundMusic);
    }

    
    public void playBackgroundMusic(AudioClip audiosource)
    {
        backGroundMusicAudioSource.clip = audiosource;
        backGroundMusicAudioSource.Play();
    }
}
