using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScribble : MonoBehaviour
{
    [FMODUnity.EventRef]
    public List<string> sounds;

    Rigidbody rigidBodytest = null;
    FMOD.Studio.EventInstance soundEvent;
    string soundPath;
    // Start is called before the first frame update
    void Awake()
    {
        int rand = UnityEngine.Random.Range(0, sounds.Count);
        soundPath = sounds[rand];
        soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), rigidBodytest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if(sounds != null)
        {
            
            
            soundEvent.start();
        }
        
        
    }

    public void Stop()
    {
        FMOD.Studio.STOP_MODE stopType;
        stopType = FMOD.Studio.STOP_MODE.IMMEDIATE;

        soundEvent.stop(stopType);
    }


}