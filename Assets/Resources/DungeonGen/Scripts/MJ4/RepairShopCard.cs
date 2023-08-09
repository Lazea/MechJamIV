public class RepairShopCard : CardReader
{
   public void LoadRepairShop()
    {
        BaseCardReader.SetActiveCard(map, card, env);

        // Load Repair Shop
        ScenesManager.Instance.LoadScene(2);
    }

    public override void ReadCard(Map_Card newCard)
    {
        card = newCard;
    }
}
