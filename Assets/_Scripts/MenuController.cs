﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;

public class MenuController : MonoBehaviour
{
    public static MenuController S;
    float firstScreenPos, secondScreenPos, thirdScreenPos;
    public Button fileSelection, play, gallery, settings;
    public float moveDuration = 1;
    public float moveDistance = 20;

    public AudioSource backgroundMusic;
    string _inputSongPath = "";
    AudioClip _inputSong;
    public string songPath { get { return _inputSongPath; } }
    public AudioClip song { get { return _inputSong; } }

    void Awake()
    {
        S = this;

        if(!fileSelection || !play || !gallery || !settings)
        {
            fileSelection = transform.Find("Text").GetComponent<Button>();
            play = transform.Find("Play").GetComponent<Button>();
            gallery = transform.Find("Gallery").GetComponent<Button>();
            settings = transform.Find("Settings").GetComponent<Button>();
        }
    }

    void Start()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Music", ".mp3", ".wav"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".mp3");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExploreFiles()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1); //Loads Main Game Scene
    }

    public void Gallery()
    {
        StartCoroutine(ViewGallery(moveDuration));
    }

    public void Settings()
    {
        StartCoroutine(ViewSettings(moveDuration));
    }

    public void ChangeMusic(string path)
    {
        fileSelection.GetComponent<Text>().text = path;
    }

    IEnumerator ViewGallery(float duration)
    {
        float startingPos = transform.position.x;
        float endPos = -moveDistance;
        Vector3 tempPos = transform.position;
        for(float t = 0; t < 1; t += (.01f / duration))
        {
            tempPos.x = Mathf.SmoothStep(startingPos, endPos, t); //Negative target value because we're really offseting the entire canvas by that value
            transform.position = tempPos;
            print(t);
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator ViewSettings(float duration)
    {
        float startingPos = transform.position.x;
        float endPos = moveDistance;
        Vector3 tempPos = transform.position;
        for (float t = 0; t < 1; t += (.01f / duration))
        {
            tempPos.x = Mathf.SmoothStep(startingPos, endPos, t); //Negative target value because we're really offseting the entire canvas by that value
            transform.position = tempPos;
            print(t);
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        UnityEngine.Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            _inputSongPath = FileBrowser.Result;
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);
            backgroundMusic.clip = NAudioPlayer.FromMp3Data(bytes);
            _inputSong = backgroundMusic.clip;
            backgroundMusic.Play();
        }

    }

}
