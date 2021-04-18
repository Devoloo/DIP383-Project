using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, GameControl.difficulty);
    }

    private void OnMouseDown()
    {
        if (!GameControl.pausePanelActive)
        {
            GameControl.targetsHit++;
            Destroy(gameObject);
        }
    }
}
