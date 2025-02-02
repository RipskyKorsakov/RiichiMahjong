using RMU.Players;
using RMU.Tiles;
using Godot;

namespace RMU.Calls.CallCommands;

public sealed class CallOpenKan1Command : CallCommand
{
    public CallOpenKan1Command(Player playerMakingCall, Tile calledTile) : base(playerMakingCall, calledTile)
    {

    }

    public override void Execute()
    {
        _handMakingCall.OpenHand();
        _handMakingCall.CreateOpenMeld(_calledTile, OPEN_KAN_1);
        for (int i = 0; i < 3; i++)
        {
            _handMakingCall.RemoveCopyOfTile(_calledTile);
        }
        _playerMakingCall.DrawTileFromDeadWall();
        _playerMakingCall.GetGame().GetDeadWall().RevealDoraTile();
    }

    public override int GetPriority()
    {
        return 2;
    }
}