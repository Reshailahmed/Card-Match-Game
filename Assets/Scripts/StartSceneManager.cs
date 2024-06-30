using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    public TMP_InputField rowsInput;
    public TMP_InputField columnsInput;

    public void StartNewGame()
    {
        OnButtonClick();
        if (int.TryParse(rowsInput.text, out int rows) && int.TryParse(columnsInput.text, out int columns))
        {
            GameConstants.Rows = rows;
            GameConstants.Columns = columns;
        }
        else
        {
            Debug.LogWarning("Invalid input for rows or columns.");
        }
            GameConstants.GameState = true;  // Set to true for new game
            SceneManager.LoadScene("GamePlay");
    }

    public void ResumeGame()
    {
        OnButtonClick();
        GameConstants.GameState = false;  // Set to false for resume game
        SceneManager.LoadScene("GamePlay");
    }

    public void OnButtonClick()
    {
        EventManager.Instance.NotifyObservers("ButtonClick");
    }
}
