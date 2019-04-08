using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    [FMODUnity.EventRef]
    public List<string> sounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if(sounds != null)
        {
            Rigidbody rigidBodytest = null;
            int rand = UnityEngine.Random.Range(0, sounds.Count);
            FMOD.Studio.EventInstance soundEvent;
            string soundPath = sounds[rand];
            soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), rigidBodytest);
            soundEvent.start();
        }
        
        
    }

    public void PlayFrom(Transform transfom)
    {
        Rigidbody rigidBodytest = null;
            int rand = UnityEngine.Random.Range(0, sounds.Count);
            FMOD.Studio.EventInstance soundEvent;
            string soundPath = sounds[rand];
            soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, transfom, rigidBodytest);
            soundEvent.start();
    }
}


