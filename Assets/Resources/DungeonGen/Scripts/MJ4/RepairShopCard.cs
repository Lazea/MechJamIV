public class RepairShopCard : BaseCardReader, ICardReader
{
    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    protected override void SelectCard()
    {
        // Load Repair Shop
        ScenesManager.Instance.LoadScene(2);
    }

    public void ReadCard(Map_Card newCard)
    {

    }
}
