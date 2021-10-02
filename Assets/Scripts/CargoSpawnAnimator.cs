using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSpawnAnimator : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 targetPos;

    public float maxAirTime = 10f;
    
    public int bounceNumber = 5;
    
    private float _currentAirTime = 0f;


    // Update is called once per frame
    void Update()
    {
        float time = _currentAirTime / maxAirTime;
        if (time <= 1)
        {
            Interpolation interpolation = new Interpolation.BounceOut(bounceNumber);
            float bounceY = interpolation.apply(time);

            Vector2 distance = startPos - targetPos;
            
            Vector2 pos = Vector2.Lerp(startPos, targetPos, time);

            transform.position = new Vector3(pos.x, startPos.y+(distance.y * bounceY), transform.position.z);

            _currentAirTime += Time.deltaTime;
        }
    }
}
