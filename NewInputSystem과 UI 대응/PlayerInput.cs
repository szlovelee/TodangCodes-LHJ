public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    private UIManager _uiManager;
    private GameManager _gameManager;

    private UI_VirtualPad _virtualPad;
    private GameSceneUIInputController _gameSceneUIInputController;

    private HashSet<InputAction> _controllingAction = new HashSet<InputAction>();

    private PointerEventData eventData = new PointerEventData(EventSystem.current);
    private List<RaycastResult> rayResults = new List<RaycastResult>(1);

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _uiManager = UIManager.Instance;
        _gameManager = GameManager.Instance;

        Debug.Assert(_uiManager != null, "Null Exception : UIManager");
        Debug.Assert(_gameManager != null, "Null Exception : GameManager");

        _virtualPad = _uiManager.GetUIComponent<UI_VirtualPad>();

        _gameSceneUIInputController = new GameSceneUIInputController(this);

        EnableInput(true);
        _gameManager.ActivateEscapeInput(false);
    }


    public void EnableInput(bool isActive)
    {
        if (!isActive)
        {
            _inputActions.Disable();
            _virtualPad.CloseUI(false);
        }
        else
        {
            _inputActions.Enable();
            _virtualPad.OpenUI(false);
            foreach (InputAction action in _controllingAction)
            {
                action.Disable();
            }
        }
    }

    /// <summary>
    /// Input : Player/Move, Player/Interaction, Player/PickUp, Game/CookBook, Game/Pause
    /// Enabling the disabled input is required in order to make the input active.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="enable"></param>
    public void ControlInput(string input, bool enable)
    {
        InputAction action = InputActions.FindAction(input);
        if (action != null)
        {
            if (enable)
            {
                action.Enable();
                if (_controllingAction.Contains(action))
                {
                    _controllingAction.Remove(action);
                }
            }
            else
            {
                action.Disable();
                _controllingAction.Add(action);
            }
        }
    }

    public bool IsMouseOverUIButton()
    {
        rayResults.Clear();
        eventData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(eventData, rayResults);

        return rayResults.Count > 0;
    }

    private void ResetInputs()
    {
        InputActions.Disable();
        _gameManager.ActivateEscapeInput(true);
    }

    private void OnDestroy()
    {
        ResetInputs();
    }

}
