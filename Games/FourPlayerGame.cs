﻿using RMU.Players;
using RMU.Hands;
using static RMU.Globals.Enums;

namespace RMU.Games
{
    public abstract class FourPlayerGame : AbstractGame
    {
        public FourPlayerGame()
        {
            _players = new FourPlayerAbstractPlayer[4];
            _players[0] = new FourPlayerStandardPlayer(EAST, new StandardFourPlayerHand(_wallObject));
            _players[1] = new FourPlayerStandardPlayer(SOUTH, new StandardFourPlayerHand(_wallObject));
            _players[2] = new FourPlayerStandardPlayer(WEST, new StandardFourPlayerHand(_wallObject));
            _players[3] = new FourPlayerStandardPlayer(NORTH, new StandardFourPlayerHand(_wallObject));
            _wall = _wallObject.GetWall();
            _deadWall = _wallObject.GetDeadWall();
            ArrangePlayers();
        }

        private void ArrangePlayers()
        {
            _players[0].SetPlayerOnRight(_players[1]);
            _players[0].SetPlayerAcross(_players[2]);
            _players[0].SetPlayerOnLeft(_players[3]);

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
    }
}
