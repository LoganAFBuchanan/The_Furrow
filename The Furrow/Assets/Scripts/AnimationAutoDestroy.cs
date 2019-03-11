using UnityEngine;
using System.Collections;

// Adapted from firestroke's answer fround here: https://answers.unity.com/questions/670860/delete-object-after-animation-2d.html

public class AnimationAutoDestroy : MonoBehaviour {
     public float delay = 0f;
 
     // Use this for initialization
     void Start () {
         Destroy (gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay); 
     }
 }