using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// William de Beer
/// </summary>
public class LevelManager : SingletonPersistent<LevelManager>
{
    public enum Transition
    {
        CROSSFADE,
        YOUDIED,
        YOUWIN,
        OUTOFAMMO,
    }
    
    public static bool cheatsEnabled = false;
    public static bool loadingNextArea = false;

    public GameObject CompleteLoadUI;

    public static GameObject transitionPrefab;
    public static GameObject youdiedPrefab;
    public static GameObject youwinPrefab;
    public static Animator transition;

    public bool isTransitioning = false;
    public float transitionTime = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        transitionPrefab = Resources.Load<GameObject>("Transitions/TransitionCanvas");
        youdiedPrefab = Resources.Load<GameObject>("Transitions/YouDiedCanvas");
        youwinPrefab = Resources.Load<GameObject>("Transitions/YouWinCanvas");
    }

    private void Start()
    { 
        loadingNextArea = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ReloadLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name));
    }
    public void LoadNextLevel()
    {
        loadingNextArea = true;
        if (SceneManager.sceneCountInBuildSettings <= SceneManager.GetActiveScene().buildIndex + 1) // Check if index exceeds scene count
        {
            StartCoroutine(LoadLevel(SceneManager.GetSceneByBuildIndex(0).name)); // Load menu
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name)); // Loade next scene
        }
    }
    public void LoadNewLevel(string _name, Transition _transition = Transition.CROSSFADE)
    {
        if (!isTransitioning)
            StartCoroutine(LoadLevel(_name, _transition));
    }
    public void ResetScene()
    {
        loadingNextArea = true;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name));
    }

    public void LoadLevelAsync(int levelIndex, float maxTime)
    {
        
    }


    IEnumerator LoadLevel(string _name, Transition _transition = Transition.CROSSFADE)
    {
        float startTimeMult = 1.0f;
        float endTimeMult = 1.0f;
        isTransitioning = true;

        switch (_transition)
        {
            case Transition.CROSSFADE:
                transition = Instantiate(transitionPrefab, transform).GetComponent<Animator>();
                break;
            case Transition.OUTOFAMMO:
                transition = Instantiate(transitionPrefab, transform).GetComponent<Animator>();
                startTimeMult = 10.0f;
                endTimeMult = 1.0f;
                break;
            case Transition.YOUDIED:
                transition = Instantiate(youdiedPrefab, transform).GetComponent<Animator>();
                startTimeMult = 5.0f;
                endTimeMult = 5.0f;
                break;
            case Transition.YOUWIN:
                transition = Instantiate(youwinPrefab, transform).GetComponent<Animator>();
                startTimeMult = 3.5f;
                endTimeMult = 3.5f;
                break;
        }

        transition.speed = 1.0f / startTimeMult;

        if (transition != null)
        {
            // Wait to let animation finish playing
            yield return new WaitForSeconds(transitionTime * startTimeMult);
        }


        transition.speed = 1.0f / endTimeMult;

        //Load Scene
        SceneManager.LoadScene(_name);

        yield return new WaitForSeconds(transitionTime * endTimeMult);

        if (transition != null)
        {
            Destroy(transition.gameObject);
            transition = null;
        }
        isTransitioning = false;
        yield return null;
    }

    //public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    //if(GameManager.HasInstance())
    //    //{
    //    //    var objects = GameManager.instance.m_saveSlot.GetSceneData(scene.buildIndex);

    //    //    if (objects == null || objects.Length == 0)
    //    //        return;

    //    //    foreach (var item in objects)
    //    //    {
    //    //        if (item != null)
    //    //        {
    //    //            int id = item.m_itemID;
    //    //            GameObject prefab = Resources.Load<GameObject>(GameManager.instance.m_items.list[id].placePrefabName);

    //    //            GameObject inWorld = Instantiate(prefab, new Vector3(item.x, item.y, item.z), Quaternion.Euler(item.rx, item.ry, item.rz));
    //    //            inWorld.GetComponent<SerializedObject>().UpdateTo(item);
    //    //        }
    //    //    }

    //    //    GameManager.instance.m_saveSlot.InstansiateNPCs(scene.buildIndex);
    //    //}
    //}

    //public void SaveSceneToSlot(SaveSlot slot)
    //{
    //    slot.SaveObjects(GameObject.FindGameObjectsWithTag("SerializedObject"));
    //    foreach (var item in GameObject.FindGameObjectsWithTag("NPC"))
    //    {
    //        slot.AddNPC(item.GetComponent<NPCScript>());
    //    }
    //}
    //IEnumerator OperationLoadLevelAsync(int levelIndex, float maxTime)
    //{
    //    AsyncOperation gameLoad = SceneManager.LoadSceneAsync(levelIndex);
    //    gameLoad.allowSceneActivation = false;
    //    float time = 0.0f;

    //    while (!gameLoad.isDone)
    //    {
    //        time += Time.deltaTime;
    //        if (gameLoad.progress >= 0.9f)
    //        {
    //            CompleteLoadUI.SetActive(true);

    //            if (InputManager.GetInstance().GetKeyDown(InputManager.ButtonType.BUTTON_SOUTH, 0))
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //            if (InputManager.GetInstance().GetKeyDown(InputManager.ButtonType.BUTTON_SOUTH, 1))
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //            if (time >= maxTime)
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //        }
    //        yield return new WaitForEndOfFrame();
    //    }

    //    CompleteLoadUI.SetActive(false);
    //    yield return null;
    //}
}
