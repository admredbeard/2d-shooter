using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBehaviourScript : MonoBehaviour
{
    SpriteRenderer zoneImage;
    // Start is called before the first frame update
    void Start()
    {
        zoneImage = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(5,5,1);
        StartCoroutine("Fade");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Fade(){
        float lerpAmount = 0;
        bool fadeOut = true;
		while (true) {
            Color c = zoneImage.color;
            if(c.a <= 0.1f && fadeOut){
                fadeOut = false;
                lerpAmount = 0;
            }else if(c.a >= 0.7f && !fadeOut){
                fadeOut = true;
                lerpAmount = 0;
            }
            if(fadeOut){
                c.a = Mathf.Lerp(0.7f,0.1f,lerpAmount);
                zoneImage.color = c;
                lerpAmount += 0.01f;
            }else{
                c.a = Mathf.Lerp(0.1f, 0.7f,lerpAmount);
                zoneImage.color = c;
                lerpAmount += 0.01f;
            }
            yield return null;
		}
    }
}
