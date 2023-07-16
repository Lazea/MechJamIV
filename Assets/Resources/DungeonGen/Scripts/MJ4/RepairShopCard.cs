public class RepairShopCard : BaseCardReader
{
    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    protected override void SelectCard()
    {
        // Load Repair Shop
        ScenesManager.Instance.LoadScene(2);
    }
}
