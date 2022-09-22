﻿using RMU.Calls.CallCommands;
using RMU.Calls.PotentialCalls;
using RMU.Games.Scoring;
using RMU.Players;
using RMU.Tiles;
using RMU.Walls;
using RMU.Walls.DeadWall;
using RMU.Yaku;
using RMU.Yaku.Yakuman;

namespace RMU.Games;

public abstract class AbstractGame
{
    protected Player[] _players;
    protected WallObject _wallObject;
    protected Wall _wall;
    protected IDeadWall _deadWall;
    private Tile _lastTile;
    private PriorityQueueForPotentialCalls _potentialQueue;
    private PriorityQueueForCallCommands _commandQueue;
    private WinningCallType _winningCall;
    private Player _activePlayer;
    private Wind _roundWind;
    private HandScoreBase _scoreObject;
    private Dictionary<(int, Suit), int> _visibleTiles;

    protected void Start()
    {
        _roundWind = EAST;
        _scoreObject = null;
        _activePlayer = GetEastPlayer();
        _winningCall = NO_WIN;
        _activePlayer.GetHand().DrawTileFromWall();
        _lastTile = null;
        _potentialQueue = new PriorityQueueForPotentialCalls();
        _commandQueue = new PriorityQueueForCallCommands(this);
        _visibleTiles = new Dictionary<(int, Suit), int>();
        foreach (Player player in _players)
        {
            player.SetPriorityQueueForPotentialCalls(_potentialQueue);
            player.SetPriorityQueueForCallCommands(_commandQueue);
            player.SetAvailablePotentialCalls();
        }
    }

    public void RotateWinds()
    {
        foreach (Player player in _players)
        {
            player.SetSeatWind(GetNewWind(player.GetSeatWind()));
        }
    }

    protected abstract Wind GetNewWind(Wind originalWind);

    public void CheckCalls()
    {
        foreach (Player player in _players)
        {
            if (player.IsActivePlayer() == false)
            {
                player.GeneratePotentialDiscardCalls(_lastTile);
                player.UpdateAvailableCalls();
            }
        }
        if (_potentialQueue.IsEmpty())
        {
            NextPlayer();
        }
    }

    public virtual void NextPlayer()
    {
        if (_activePlayer.GetPlayerOnRight() != null)
        {
            SetActivePlayer(_activePlayer.GetPlayerOnRight());
        }
        else
        {
            SetActivePlayer(_activePlayer.GetPlayerAcross());
        }
    }

    private void SetActivePlayer(Player player)
    {
        _activePlayer = player;
        if (_activePlayer is not null)
            _activePlayer.DrawTile();
    }

    public Player GetActivePlayer()
    {
        return _activePlayer;
    }

    public Player[] GetPlayers()
    {
        return _players;
    }

    public Player GetEastPlayer()
    {
        return GetPlayerByWind(EAST);
    }

    public Player GetSouthPlayer()
    {
        return GetPlayerByWind(SOUTH);
    }

    public Player GetWestPlayer()
    {
        return GetPlayerByWind(WEST);
    }

    public Player GetNorthPlayer()
    {
        return GetPlayerByWind(NORTH);
    }

    public Wall GetWall()
    {
        return _wall;
    }

    public IDeadWall GetDeadWall()
    {
        return _deadWall;
    }

    public WallObject GetWallObject()
    {
        return _wallObject;
    }

    public Tile GetLastTile()
    {
        return _lastTile;
    }

    public void SetLastTile(Tile tile)
    {
        _lastTile = tile;
    }

    public void ClearLastTile()
    {
        _lastTile = null;
    }

    private Player GetPlayerByWind(Wind wind)
    {
        foreach (Player p in _players)
        {
            if (p.GetSeatWind() == wind)
            {
                return p;
            }
        }
        return null;
    }

    public void CallTsumo(Player player, List<YakuBase> satisfiedYaku)
    {
        _winningCall = TSUMO;
        CallWin(player, satisfiedYaku);
    }

    public void CallRon(Player player, List<YakuBase> satisfiedYaku)
    {
        _winningCall = RON;
        CallWin(player, satisfiedYaku);
    }

    private void CallWin(Player player, List<YakuBase> satisfiedYaku)
    {
        if (satisfiedYaku.Count == 0)
        {
            throw new System.Exception("Hand completed with no satisfied yaku");
        }

        if (_winningCall == NO_WIN)
        {
            throw new System.Exception("No winning call made");
        }

        SetActivePlayer(null);
        _scoreObject = satisfiedYaku[0] is YakumanBase ? new YakumanHandScore(player, _winningCall) : new StandardHandScore(player, _winningCall);
    }

    public HandScoreBase GetHandScore()
    {
        return _scoreObject;
    }

    public WinningCallType GetWinningCall()
    {
        return _winningCall;
    }

    public Wind GetRoundWind()
    {
        return _roundWind;
    }

    public void AddNewVisibleTile(Tile tile)
    {
        int value = tile.GetValue();
        Suit suit = tile.GetSuit();
        if (_visibleTiles.ContainsKey((value, suit)))
        {
            _visibleTiles[(value, suit)]++;
            return;
        }
        _visibleTiles.Add((value, suit), 1);
    }

    public int NumberOfCopiesVisible(Tile tile)
    {
        int value = tile.GetValue();
        Suit suit = tile.GetSuit();
        if (_visibleTiles.ContainsKey((value, suit)))
            return _visibleTiles[(value, suit)];
        return 0;
    }
}
