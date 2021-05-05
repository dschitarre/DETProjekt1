using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class StartGui : UIBehaviour, ICancelHandler
{
    // Start is called before the first frame update
    public int schwierigkeit=1;


    [Tooltip("The name of the scene to load to shoose the settings of the game.")]
    public string SchwierigkeitsentscheidungScenenname;
    [Tooltip("The name of the scene to load Game started.")]
    public string gameSceneName;
    public void OnStartClicked()
    {
        SceneManager.LoadScene(SchwierigkeitsentscheidungScenenname, LoadSceneMode.Single);
    }
     public void OnQuitClicked()
    {
    #if UNITY_EDITOR
        Debug.Log("Quit");
    #endif
    Application.Quit(); // note: this does nothing in the Editor
    }
    public void OnCancel(BaseEventData eventData)
    {
        OnQuitClicked();
    }
    public void startMovingWalls()
    {
        schwierigkeit=0;
        startGame();
    }

    public void startEasyGame()
    {
        schwierigkeit=1;
        startGame();
    }

    public void startMediumGame()
    {
        schwierigkeit=2;
        startGame();
    }

    public void startHardGame()
    {
        schwierigkeit=3;
        startGame();
    }

    public void startImpossibleGame()
    {
        schwierigkeit=4;
        startGame();
    }
    private void startGame()
    {
        SceneManager.LoadScene("GUI", LoadSceneMode.Single);
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Additive);
        Resources.Load<GameSettings>("Settings").changeSettingsAtStart(schwierigkeit);
        
    }
}
