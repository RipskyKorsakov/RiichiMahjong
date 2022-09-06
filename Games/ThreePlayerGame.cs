﻿using RMU.Players;
using RMU.Hands;
using static RMU.Globals.Enums;

namespace RMU.Games
{
    public abstract class ThreePlayerGame : AbstractGame
    {

        public ThreePlayerGame()
        {
            _players = new ThreePlayerAbstractPlayer[3];
            _players[0] = new ThreePlayerStandardPlayer(EAST, new StandardThreePlayerHand(_wallObject));
            _players[1] = new ThreePlayerStandardPlayer(SOUTH, new StandardThreePlayerHand(_wallObject));
            _players[2] = new ThreePlayerStandardPlayer(WEST, new StandardThreePlayerHand(_wallObject));
            _wall = _wallObject.GetWall();
            _deadWall = _wallObject.GetDeadWall();
            ArrangePlayers();
        }

        private void ArrangePlayers()
        {
            _players[0].SetPlayerOnRight(_players[1]);
            _players[0].SetPlayerAcross(_players[2]);

            _players[1].SetPlayerOnLeft(_players[0]);
            _players[1].SetPlayerOnRight(_players[2]);

            _players[2].SetPlayerOnLeft(_players[1]);
            _players[2].SetPlayerAcross(_players[0]);
        }
    }
}
