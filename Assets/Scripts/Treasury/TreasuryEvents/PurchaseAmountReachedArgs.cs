public class PurchaseAmountReachedArgs
{
    public int Total { get; private set; }

    public PurchaseAmountReachedArgs( int total )
    {
        Total = total;
    }
}
