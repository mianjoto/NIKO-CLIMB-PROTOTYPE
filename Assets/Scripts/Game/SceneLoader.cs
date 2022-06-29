using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // Code is inspired from CodeMonkey's YouTube channel
    public enum Scene
    {
        MainMenu,
        Loading,
        ArcadeMode
    }

    private static Action OnLoaderCallback;

    public static void Load(Scene scene)
    {   
        // Load the target scene after the callback
        OnLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };

        // Load the loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback() {
        if (OnLoaderCallback != null)
        {
            OnLoaderCallback();
            OnLoaderCallback = null;
        }
    }
}
