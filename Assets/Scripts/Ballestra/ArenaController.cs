using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public List<SpotController> Spots;


    void Awake()
    {
        foreach (SpotController spot in GetComponentsInChildren<SpotController>())
        {
            Spots.Add(spot);
        }
       ;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public SpotController getNextSpot(SpotController currentSpot)
    {
        int currentIndex = Spots.IndexOf(currentSpot);
        if (currentIndex < 0) return null;
        int nextIndex = (currentIndex + 1) % Spots.Count;
        return Spots[nextIndex];
    }
    
    public SpotController getPrevSpot(SpotController currentSpot)
    {
        int currentIndex = Spots.IndexOf(currentSpot);
        if (currentIndex < 0) return null;
        int prevIndex = (currentIndex - 1 + Spots.Count) % Spots.Count;
        return Spots[prevIndex];
    }
}
