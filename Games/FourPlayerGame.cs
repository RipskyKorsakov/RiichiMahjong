﻿using Godot;
using RMU.Hands;
using RMU.Players;

namespace RMU.Games;

public abstract class FourPlayerGame : AbstractGame
{
    
    
    protected virtual void Init()
    {
        _players = new FourPlayerAbstractPlayer[4];
        _players[0] = new FourPlayerStandardPlayer(EAST, new StandardFourPlayerHand(_wallObject), this);
        _players[0].SetPlayerID(0);
        _players[1] = new FourPlayerStandardPlayer(SOUTH, new StandardFourPlayerHand(_wallObject), this);
        _players[1].SetPlayerID(1);
        _players[2] = new FourPlayerStandardPlayer(WEST, new StandardFourPlayerHand(_wallObject), this);
        _players[2].SetPlayerID(2);
        _players[3] = new FourPlayerStandardPlayer(NORTH, new StandardFourPlayerHand(_wallObject), this);
        _players[3].SetPlayerID(3);
        _wallObject.GenerateDeadWall();
        _wall = _wallObject.GetWall();
        SetDeadWall(_wallObject.GetDeadWall());
        ArrangePlayers();
        _firstGoAroundCounter = 4;
        Start();
    }

    protected void ArrangePlayers()
    {
        _players[0].SetPlayerOnLeft(_players[3]);
        _players[0].SetPlayerAcross(_players[2]);
        _players[0].SetPlayerOnRight(_players[1]);

        _players[1].SetPlayerOnLeft(_players[0]);
        _players[1].SetPlayerAcross(_players[3]);
        _players[1].SetPlayerOnRight(_players[2]);

        _players[2].SetPlayerOnLeft(_players[1]);
        _players[2].SetPlayerAcross(_players[0]);
        _players[2].SetPlayerOnRight(_players[3]);

        _players[3].SetPlayerOnLeft(_players[2]);
        _players[3].SetPlayerAcross(_players[1]);
        _players[3].SetPlayerOnRight(_players[0]);
    }

    protected override Wind GetNewWind(Wind originalWind)
    {
        return originalWind switch
        {
            EAST => NORTH,
            SOUTH => EAST,
            WEST => SOUTH,
            NORTH => WEST,
            _ => throw new System.Exception("Invalid wind")
        };
    }
}
