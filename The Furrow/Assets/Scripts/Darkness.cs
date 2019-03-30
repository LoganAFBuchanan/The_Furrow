using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Darkness : MonoBehaviour
{

    //Z position when darkness covers the entirety of the map
    public float maxZPos;
    //Z position when darkness covers none of the map
    public float minZPos;
    //Total distance the darkness can travel to cover the whole map
    private float totalZDistance;

    public float incrementPct;
    private float incrementVal;

    public bool isMoving;

    public float MovementSpeed;

    public void Initialize()
    {
        totalZDistance = Mathf.Abs(minZPos - maxZPos);
        SetIncrements();    
    }

    public void MoveDarkness(int increments)
    {
        
        float distanceToTravel = incrementVal * increments;
        Debug.Log("WE MOVING " + distanceToTravel);
        StartCoroutine(MovementAnimation(distanceToTravel));
    }

    //Set value for increment value based on a percentage of the total darkness distance
    public void SetIncrements()
    {
        incrementVal = totalZDistance * incrementPct;
    }

    private IEnumerator MovementAnimation(float zDest)
    {
        isMoving = true;

        Vector3 destination_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z + zDest);
        while (transform.position != destination_pos)
        {
                
            transform.position = Vector3.MoveTowards(transform.position, destination_pos, Time.deltaTime * MovementSpeed);
            yield return 0;
        }
        
        isMoving = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("entered " + other.gameObject.name);

            if(other.gameObject.tag == "MapNode")
            {
                other.gameObject.GetComponent<MapNode>().isCorrupted = true;
            }
    }

}