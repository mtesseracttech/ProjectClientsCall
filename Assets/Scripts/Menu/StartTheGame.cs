using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTheGame : MonoBehaviour {

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(1);
        }
    }
}
