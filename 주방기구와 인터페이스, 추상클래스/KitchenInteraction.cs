public class KitchenInteraction : MonoBehaviour, IInteractable
{
    [Header("Cook")]
    [SerializeField] protected List<CookStepSO> availableRecipe;
    [SerializeField] protected List<GameObject> ingredients;
    protected KitchenUtensilInfoData data;

    [Header("Interaction")]
    protected Transform interactionPos;
    public bool CanInteractWithPlayer { get; set; }

    [Header("Progress")]
    [SerializeField] protected GameObject progressBar;
    [SerializeField] protected Image progressFill;
    protected float maxProgress;
    protected float currentProgress;

    [Header("PickUp")]
    [SerializeField] protected Transform[] foodPos;
    public bool IsPlaceable { get; set; }
    
    protected Player player;

    #region Managers
    protected DataManager dataManager;
    protected SoundManager soundManager;
    protected GameManager gameManager;
    protected ResourceManager resourceManager;
    #endregion

    protected void Start()
    {
        soundManager = SoundManager.Instance;
        SetManagers();
        Initialize();
    }

    private void SetManagers()
    {
        dataManager = DataManager.Instance;
        soundManager = SoundManager.Instance;
        gameManager = GameManager.Instance;
        resourceManager = ResourceManager.Instance;

        #region Manager Null Exception
        Debug.Assert(dataManager != null, "Null : DataManager");
        Debug.Assert(soundManager != null, "Null : SoundManager");
        Debug.Assert(gameManager != null, "Null : GameManager");
        Debug.Assert(resourceManager != null, "Null : ResourceManager");
        #endregion
    }


    protected virtual void Initialize()
    {
        trashObj = dataManager.GetDefaultData<IntermediateResultsInfoSO>("Trash").Prefab;
        
        if (ingredients == null) ingredients = new List<GameObject>();
        
        currentProgress = 0;

        if (data != null && data.Level > 0)
        {
            maxProgress = (data == null) ? 1 : data.DefaultData.SpeedUpgradeInfo[data.Level - 1];
        }
        else
        {
            maxProgress = 1;
        }

        UpdateProgressBar();
        successSound = "FoodSuccess2";
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }


    public virtual void Interaction()
{
    if (ingredients.Count <= 0) return;
    if (player.Ingredient != null) return;
    if (currentProgress >= maxProgress) return;

    ++ currentProgress;
    UpdateProgressBar();

    if (currentProgress == maxProgress)
    {
        CheckValidity();
        UpdateProgressBar();
    }
}

public virtual void PickUp()
{
    if (player.Ingredient != null) return;
    if (ingredients.Count <= 0) return;

    currentProgress = 0;

    ingredients[ingredients.Count - 1].transform.parent = player.foodPos;
    ingredients[ingredients.Count - 1].transform.localPosition = Vector3.zero;
    ingredients[ingredients.Count - 1].transform.localRotation = Quaternion.Euler(Vector3.zero);
    player.Ingredient = ingredients[ingredients.Count - 1];
    ingredients.RemoveAt(ingredients.Count - 1);
}

public virtual void PutDown()
{
    if (player.Ingredient == null) return;
    if (ingredients.Count == foodPos.Length) return;

    if (currentProgress != 0)
    {
        currentProgress = 0;
        UpdateProgressBar();
    }
    
    ingredients.Add(player.Ingredient);
    ingredients[ingredients.Count - 1].transform.position = foodPos[ingredients.Count - 1].position;
    ingredients[ingredients.Count - 1].transform.parent = foodPos[ingredients.Count - 1];
    ingredients[ingredients.Count - 1].transform.localRotation = Quaternion.Euler(Vector3.zero);
    player.Ingredient = null;
}

    protected virtual void CheckValidity()
    {
        List<IngredientInfoSO> ingredientStack = new List<IngredientInfoSO>();
        foreach(GameObject ingredient in ingredients)
        {
            if (ingredient == null) continue;
            ingredientStack.Add(GetIngredient(ingredient));
        }

        foreach (CookStep cookStep in availableRecipe)
        {
            if (AreListsEquivalent<IngredientInfoSO>(ingredientStack, cookStep.Ingredients))
            {
                MakeResult(cookStep.result);
                return;
            } 
        }
        MakeResult();
        currentProgress = 0;
    }


    protected virtual void MakeResult(Enums.Result  result = null)
    {
        if (ingredients.Count <= 0) return;
        
        foreach (GameObject obj in ingredients)
        {
            if (obj == null) continue;
            resourceManager.Destroy(obj); 
        }
        ingredients.Clear();

        if (result == null) result = Enums.Trash;
        GameObject newIngredient = resourceManager.Instantiate($"Foods/{result.ToString()}");
        
        if (CanInteractWithPlayer && !IsPlaceable)  // direct interaction without placing the result at any foodPos
        {
            player.Ingredient = newIngredient;
        }
        else
        {
            ingredients.Add(newIngredient);
        }
        
        newIngredient.transform.parent = interactionPos;
        newIngredient.transform.localPosition = Vector3.zero;
        newIngredient.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    

    protected void UpdateProgressBar()
    {
        if (progressBar == null) return;
        if (!IsPlaceable) return;

        if (currentProgress == 0 || currentProgress == maxProgress)
        {
            progressBar.SetActive(false);
        }
        if (currentProgress == 1 && currentProgress != maxProgress)
        {
            progressBar.SetActive(true);
        }

        progressFill.fillAmount = (float)currentProgress / maxProgress;
    }
}
