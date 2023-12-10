public class GameSceneUIInputController
{
    private PlayerInput _input;

    private UIManager _uiManager;
    private SceneManagerEx _sceneManager;

    private UI_PausePanel _pausePanel;
    private UI_GameSettings _gameSettings;
    private UI_CookBook _cookBook;
    private UI_ResultPanel _resultPanel;
    private UI_IngredientBox _ingredientBox;

    public GameSceneUIInputController(PlayerInput input)
    {
        _input = input;
        Init();
        AddCallbacks();
    }

    private void Init()
    {
        _uiManager = UIManager.Instance;
        _sceneManager = SceneManagerEx.Instance;

        Debug.Assert(_uiManager != null, "Null Exception : UIManager");
        Debug.Assert(_sceneManager != null, "Null Exception : SceneManager");

        _pausePanel = _uiManager.GetUIComponent<UI_PausePanel>();
        _gameSettings = _uiManager.GetComponent<UI_GameSettings>();
        _cookBook = _uiManager.GetUIComponent<UI_CookBook>();
        _resultPanel = _uiManager.GetUIComponent<UI_ResultPanel>();

        if (_sceneManager.CurrentSceneType == Scenes.PracticeModeScene)
            _ingredientBox = _uiManager.GetUIComponent<UI_IngredientBoxPractice>();
        else
            _ingredientBox = _uiManager.GetUIComponent<UI_IngredientBox>();
    }

    private void AddCallbacks()
    {
        _pausePanel.OnUIOpen += (() => PauseCallback(true));
        _pausePanel.OnUIClose += (() => PauseCallback(false));

        _cookBook.OnUIOpen += (() => CookBookCallback(true));
        _cookBook.OnUIClose += (() => CookBookCallback(false));

        _resultPanel.OnUIOpen += (() => _input.EnableInput(false));

        _ingredientBox.OnUIOpen += (() => _input.EnableInput(false));
        _ingredientBox.OnUIClose += (() => _input.EnableInput(true));

        _input.GameActions.Pause.performed += TogglePausePanel;
        _input.GameActions.CookBook.performed += ToggleCookBook;

        #region Ensure Deactivating UI
        _pausePanel.gameObject.SetActive(false);
        _cookBook.gameObject.SetActive(false);
        _resultPanel.gameObject.SetActive(false);
        _ingredientBox.gameObject.SetActive(false);
        #endregion
    }

    private void TogglePausePanel(InputAction.CallbackContext context)
    {
        UI_Base currentUI = _uiManager.GetCurrentUI();

        if (currentUI is UI_PausePanel || currentUI is UI_GameSettings)
        {
            currentUI.CloseUI();
        }
        else
        {
            _uiManager.GetUIComponent<UI_PausePanel>().OpenUI();
        }
    }

    private void ToggleCookBook(InputAction.CallbackContext context)
    {
        UI_Base currentUI = _uiManager.GetCurrentUI();

        if (currentUI is UI_CookBook)
        {
            currentUI.CloseUI();
        }
        else
        {
            _uiManager.GetUIComponent<UI_CookBook>().OpenUI();
        }
    }

    private void PauseCallback(bool isOpen)
    {
        if (isOpen)
        {
            _input.EnableInput(false);
            _input.ControlInput("Game/Pause", true);
        }
        else
        {
            _input.EnableInput(true);
        }
    }

    private void CookBookCallback(bool isOpen)
    {
        if (isOpen)
        {
            _input.EnableInput(false);
            _input.ControlInput("Game/CookBook", true);
        }
        else
        {
            _input.EnableInput(true);
        }
    }
}
