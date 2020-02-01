using UnityEngine;
using UnityEngine.SceneManagement;
public class Rocket : MonoBehaviour
{
    [SerializeField]float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Trascending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1144;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // todo stop sound on death
    }
    void FixedUpdate()
    {
        if (state == State.Alive)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        RespondeToThrustInput();
        RespondeToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state == State.Dying || state == State.Trascending) return; // ignore collisions

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            case "Enemy":
                state = State.Dying;
                print("Dead");
                audioSource.Stop();
                audioSource.volume = 0.2f;
                audioSource.PlayOneShot(death);
                Invoke("LoadStartLevel", 1.5f);
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                state = State.Trascending;
                audioSource.Stop();
                audioSource.volume = 0.6f;
                audioSource.PlayOneShot(success);
                Invoke("LoadNextLevel", 1.5f);
                print("Congratulations!");
                break;
        }
    }

    private void LoadStartLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level "+2);
    }

    private void RespondeToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void RespondeToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }
}
