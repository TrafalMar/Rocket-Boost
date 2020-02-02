using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] float levelLoadDelay = 1.5f;
    [SerializeField] float nextLevel;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Trascending };
    State state = State.Alive;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
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
        if (Debug.isDebugBuild)
        {
            RespondeToDebugKeys();
        }
    }

    private void RespondeToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void ProcessInput()
    {
        RespondeToThrustInput();
        RespondeToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Dying || state == State.Trascending || collisionsDisabled) return; // ignore collisions

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            case "Enemy":
                PlayDeathSequence();
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                PlaySuccessSequence();
                break;
        }
    }

    private void PlayDeathSequence()
    {
        state = State.Dying;
        print("Dead");
        audioSource.Stop();
        audioSource.volume = 0.2f;
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadStartLevel", levelLoadDelay);
    }

    private void PlaySuccessSequence()
    {
        state = State.Trascending;
        audioSource.Stop();
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(success);
        mainEngineParticles.Stop();
        successParticles.Play();
        rigidBody.freezeRotation = false;
        Invoke("LoadNextLevel", nextLevel);
        print("Congratulations!");
    }

    private void LoadStartLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex + 1 >= SceneManager.sceneCountInBuildSettings) { 
            LoadStartLevel(); 
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }

    }

    private void RespondeToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rotationThisFrame);
        }
        
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame);
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
