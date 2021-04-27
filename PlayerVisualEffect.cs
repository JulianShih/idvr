using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffect : MonoBehaviour
{
    public GameObject highFiveEffect;
    public GameObject lightEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void activateHighFiveEffect(Vector3 position){
        highFiveEffect.SetActive(false);
        highFiveEffect.transform.position = position;
        highFiveEffect.SetActive(true);
    }
    public void activateLightEffect(Vector3 position)
    {
        lightEffect.SetActive(false);
        lightEffect.transform.position = position;
        lightEffect.SetActive(true);
    }
}
