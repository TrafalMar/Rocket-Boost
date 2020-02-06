using UnityEngine;

[DisallowMultipleComponent]
public class oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(0,5,0);

    [Range(0,1)]
    [SerializeField]
    float movementFactor;
    [Range(0, 10)]
    [SerializeField] 
    float period = 2f;
    [Range(0, 1)]
    [SerializeField]
    float offset = 0;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) return;
        float cycles = Time.time/period;
        float rawSinWave = Mathf.Sin(cycles * Mathf.PI*2+offset*Mathf.PI);
        movementFactor = rawSinWave / 2f+0.5f;

        Vector3  position= movementVector * movementFactor;
        transform.position = startingPos + position;
    }
}
