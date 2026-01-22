public class TreasuryChangedArgs
{
    public int Value { get; private set; }
    public int Total { get; private set; }

    public TreasuryChangedArgs( int value, int total )
    {
        Value = value;
        Total = total;
    }
}