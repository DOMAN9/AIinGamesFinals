using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class MainMenu : MonoBehaviour
    {
       public void playGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void stopGame()
        {
            Application.Quit();
        }
    }
}
