public class CounterTop : KitchenInteraction
{
    protected override void Initialize()
    {
        GetUtensilData();

        base.Initialize();

        CanInteractWithPlayer = true;
        IsPlaceable = true;

        interactionPos = foodPos[0];
        interactionSound = "CounterTop2";
    }
}
