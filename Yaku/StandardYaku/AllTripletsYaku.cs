using RMU.Hands.CompleteHands;
using RMU.Hands.CompleteHands.CompleteHandComponents;
using static RMU.Globals.Enums;

namespace RMU.Yaku.StandardYaku;

public class AllTripletsYaku : YakuBase
{
    private new readonly StandardCompleteHand _completeHand;
    
    public AllTripletsYaku(ICompleteHand completeHand) : base(completeHand)
    {
        _name = "All Triplets";
        _value = 2;
        _getValueBehaviour = new StandardGetValueBehaviour();
        _completeHand = completeHand as StandardCompleteHand;
    }

    public override bool Check()
    {
        if (_completeHand is null) return false;
        if (_completeHand.GetCompleteHandType() is not STANDARD) return false;
        return _completeHand.GetTriplets().Count == 4;
    }
}