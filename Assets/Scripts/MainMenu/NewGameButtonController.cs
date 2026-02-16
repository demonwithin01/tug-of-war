using UnityEngine;

public class NewGameButtonController : MonoBehaviour
{
    /// <summary>
    /// Starts a new game by loading the main game scene.
    /// This method is meant to be called by the OnClick event of the "New Game" button in the main menu UI.
    /// </summary>
    public void StartNewGame()
    {
        // Load the main game scene.
        UnityEngine.SceneManagement.SceneManager.LoadScene( "SampleScene" );
    }
}
