using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    [Serializable]
    public class Clips
    {
        public string Name;
        public List<AudioClip> tracks = new List<AudioClip>();
    }
    [SerializeField] private List<Clips> _clips;
    [SerializeField] private float _timeAppearance;
    private AudioSource _audio;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        
        Destroy(this.gameObject);
    }
    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += GetNowScene;
        StartCoroutine(PlaySong(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));
    }

    private void GetNowScene(Scene current, Scene next)
    {
        StopCoroutine(PlaySong(current.buildIndex));
        StartCoroutine(PlaySong(next.buildIndex));
    }

    IEnumerator PlaySong(int index) 
    {
        int clipIndex = Random.Range(0, _clips[index].tracks.Count);
        _audio.clip = _clips[index].tracks[clipIndex];
        _audio.volume = 0;
        _audio.Play();
        while(_audio.volume < 1)
        {
            _audio.volume += _timeAppearance * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(_audio.clip.length - _timeAppearance * 60);
        while(Math.Abs(_audio.volume) > .1f)
        {
            _audio.volume -= _timeAppearance * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(PlaySong(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));
    }
    
    public IEnumerator SwitchToScene(int sceneId)
    {
        while (Math.Abs(_audio.volume) > .1f)
        {
            _audio.volume -= .6f * Time.deltaTime;
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
    }
}
