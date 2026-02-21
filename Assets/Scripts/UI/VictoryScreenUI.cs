using System;
using UnityEngine;

public class VictoryScreenUI : MonoBehaviour
{
    [SerializeField]
    private VictoryLocation victoryLocation;

    /// <summary>
    /// Unity start method. Ensures the UI is hidden at the start of the game.
    /// </summary>
    private void Start()
    {
        // Ensure the UI is hidden at the start of the game.
        Hide();

        // Register the event handler for when the victory location is reached.
        this.victoryLocation.OnVictoryReached += VictoryLocation_OnVictoryReached;
    }

    /// <summary>
    /// Handler for when the victory location is reached. Shows the victory screen UI.
    /// </summary>
    private void VictoryLocation_OnVictoryReached(object sender, EventArgs e)
    {
        Show();
    }

    /// <summary>
    /// Shows the victory screen UI.
    /// </summary>
    private void Show()
    {
        
        // Activate the UI.
        gameObject.SetActive( true );
    }

    /// <summary>
    /// Hides the victory screen UI.
    /// </summary>
    private void Hide()
    {
        // Deactivate the UI.
        gameObject.SetActive( false );
    }

    /// <summary>
    /// Exits to the main menu.
    /// </summary>
    public void ExitToMainMenu()
    {
        // Load the main menu scene.
        UnityEngine.SceneManagement.SceneManager.LoadScene( "MainMenu" );
    }
}
