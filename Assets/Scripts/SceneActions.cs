using UnityEngine.SceneManagement;

public class SceneActions
{
    private int BuildSceneIndex => SceneManager.GetActiveScene().buildIndex;

    public void ResetLevel()
    {
        LoadLevel(BuildSceneIndex);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = BuildSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        LoadLevel(nextSceneIndex);
    }

    private void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
