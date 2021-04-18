using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Destroy object after a certain amount of time
        Destroy(gameObject, GameControl.difficulty);
    }

    private void OnMouseDown()
    {
        if (!GameControl.pausePanelActive)
        {
            // add score points
            GameControl.targetsHit++;
            // Destroy object after click on it
            Destroy(gameObject);
        }
    }
}
