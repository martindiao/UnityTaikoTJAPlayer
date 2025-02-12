﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UI.ScrollSnaps;

public class SongLoader : MonoBehaviour
{

    public TaikoGameInstance TGI;
    public Canvas CanvasAttached;

    public List<GameObject> UICreated = new List<GameObject>();
    ScrollRect sr;

    [SerializeField]
    private Transform SpawnPoint = null;

    [SerializeField]
    private GameObject item = null;

    TaikoSongContainer ActivePreviewSong;
    AudioType CurrentAudioType;


    public string[] itemNames = null;
    public Sprite[] itemImages = null;
    public float SongSpacing = 50;

    AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        CanvasAttached = this.gameObject.GetComponent<Canvas>();
        AS = this.GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateUI()
    {
        for (int i = 0; i <= TGI.TJAFileAvailable.Count - 1; i++)
        {

            //newSpawn Position
            Vector3 pos = new Vector3(SpawnPoint.position.x, SpawnPoint.position.y, SpawnPoint.position.z);
            //instantiate item
            GameObject SpawnedItem = Instantiate(item, new Vector3(100,-100,0), SpawnPoint.rotation);
            //setParent
            SpawnedItem.transform.SetParent(SpawnPoint.transform, false);
            //SpawnedItem.transform.position = SpawnPoint.transform.position;
            //get ItemDetails Component
            GetComponent<DirectionalScrollSnap>().InsertChild(SpawnedItem.GetComponent<RectTransform>(), SpawnedItem.transform.position, SongSpacing, SongSpacing, true);
            SpawnedItem.GetComponent<LevelTemplate>().AssignStruc(TGI.TJAFileAvailable[i], i);
            UICreated.Add(SpawnedItem);
        }
        //gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0,0);

    }

    public void OnSongSwitch(TaikoSongContainer CurrentActiveSong)
    {
        if(ActivePreviewSong.TitleName !=  "" && CurrentActiveSong.TitleName != ActivePreviewSong.TitleName)
        {
            ActivePreviewSong = CurrentActiveSong;
            Debug.Log(CurrentActiveSong.TitleName);
            //AS.Stop();
            CurrentAudioType = GetAudioTypeBasedOnPath(ActivePreviewSong.SoundWavePath);
            StartCoroutine(LoadAlbumAudio(AS));

        }
    }

    private IEnumerator LoadAlbumAudio(AudioSource player)
    {
        //Debug.Log("File Path = " + "file://" + ActivePreviewSong.OggPath + "StrucFilePath = " + ActivePreviewSong.OggPath);
        //Debug.Log(CurrentAudioType);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + ActivePreviewSong.OggPath, CurrentAudioType))
        {
            yield return www.SendWebRequest();
            //yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (player.clip != null)
                {
                    // Remove from memory
                    Destroy(player.clip);
                    //yield return null;
                }
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                player.clip = clip;
                PlaySounds();
                //Debug.Log(clip.length);
            }
        }
    }

    public void PlaySounds()
    {
        AS.time = ActivePreviewSong.DEMOSTART;
        AS.Play();
    }
    AudioType GetAudioTypeBasedOnPath(string path)
    {
        //path.Trim();
        string filename = path.Split('.')[0];
        string extension = path.Split('.')[1];
       // Debug.Log(filename + extension);
        //Debug.Log(path);
        switch (extension)
        {
            case "ogg":
                return AudioType.OGGVORBIS;
            case "mp3":
                return AudioType.MPEG;
            case "wav":
                return AudioType.WAV;
        }
        path.Split('.');
        return AudioType.OGGVORBIS;
    }
}

