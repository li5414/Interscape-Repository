using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibrate : ScriptAnimation
{
    public float duration;

    [Space]
    public float amplitude;
    public float decayRate;
    public float frequency;

    private float time = 0f;
    private Vector3 originalPos;
    private bool isPlaying = false;

    void Start() {
        originalPos = this.transform.position;
    }

    public override void Animate() {
        if (isPlaying) {
            StopCoroutine(vibrate());
            this.transform.position = originalPos;
        } 
        StartCoroutine(vibrate());
    }

    private IEnumerator vibrate() {
        isPlaying = true;
        time = 0f;
        Vector2 randomDirection = Random.insideUnitCircle;
        randomDirection.Normalize();

        while (time < duration) {
            Vector2 movementVec = dampedSinWave(time) * randomDirection;
            this.transform.position = new Vector3 (originalPos.x + movementVec.x, 
                originalPos.y + movementVec.y, originalPos.z);
            
            time += Time.deltaTime;
            yield return null;
        }

        this.transform.position = originalPos;
        isPlaying = false;
    }
    private float dampedSinWave(float x) {
        return amplitude * Mathf.Pow(2.718f, -decayRate * x) * Mathf.Sin(frequency * x);
    }
}
