﻿using Godot;
using RMU.Calls.CreateMeldBehaviours;
using RMU.DiscardPile;
using RMU.Globals.Algorithms;
using RMU.Hands.RiichiCheckHands;
using RMU.Hands.TenpaiHands;
using RMU.Shanten;
using RMU.Tiles;
using RMU.Walls;
using RMU.Walls.DeadWall;
using System;

namespace RMU.Hands;

public abstract class Hand
{
    protected Wall _wall;
    protected IDeadWall _deadWall;
    protected List<Tile> _closedTiles;
    private Tile _drawTile;
    private readonly StandardDiscardPile _discardPile;
    private readonly HandSorter _handSorter;
    private readonly List<OpenMeld> _openMelds;
    private bool _isOpen;
    private readonly List<ITenpaiHand> _tenpaiHands;
    private readonly List<Tile> _waits;
    private int _shanten;
    private bool _inRiichi;

    public event EventHandler OnCanRiichi;

    #region Constructor
    protected Hand(WallObject wallObject)
    {
        _wall = wallObject.GetWall();
        _discardPile = new StandardDiscardPile();
        _handSorter = new HandSorter();
        _closedTiles = new List<Tile>();
        _openMelds = new List<OpenMeld>();
        _tenpaiHands = new List<ITenpaiHand>();
        _waits = new List<Tile>();
        _inRiichi = false;
    }

    internal void SetDeadWall(IDeadWall deadWall)
    {
        _deadWall = deadWall;
    }
    #endregion

    #region DrawAndDiscard
    #region Discard
    public virtual void DiscardTile(int index)
    {
        if (index >= _closedTiles.Count)
        {
            return;
        }

        _discardPile.DiscardTile(_closedTiles[index]);
        _closedTiles.RemoveAt(index);
        AddDrawTileToHand();
    }

    public void DiscardDrawTile()
    {
        if (_drawTile == null)
        {
            return;
        }

        _discardPile.DiscardTile(_drawTile);
        _drawTile = null;
    }

    public void RemoveDrawTile()
    {
        _drawTile = null;
    }
    #endregion
    #region Draw
    public void DrawTileFromWall()
    {
        if (_wall.GetSize() <= 0)
        {
            throw new IndexOutOfRangeException("Tried to draw a tile from an empty wall");
        }
        SortHand();
        if (_closedTiles.Count >= 13 && _drawTile != null)
        {
            return;
        }
        if (_drawTile != null)
        {
            AddDrawTileToHand();
        }
        _drawTile = _wall.DrawTileFromWall();
    }

    public void DrawTileFromDeadWall()
    {
        SortHand();
        if (_closedTiles.Count >= 13 && _drawTile != null)
        {
            return;
        }
        if (_drawTile != null)
        {
            AddDrawTileToHand();
        }
        _drawTile = _deadWall.DrawTile();
    }

    public virtual void AddDrawTileToHand()
    {
        if (_drawTile == null)
        {
            return;
        }

        if (_closedTiles.Count >= 13)
        {
            return;
        }

        _closedTiles.Add(_drawTile);
        _drawTile = null;
        SortHand();
    }

    public void SetDrawTile(Tile tile)
    {
        _drawTile = tile;
    }

    public virtual Tile GetDrawTile()
    {
        return _drawTile;
    }
    #endregion
    #endregion

    #region Melds
    public void CreateOpenMeld(Tile calledTile, MeldType meldType)
    {
        OpenMeld openMeld = new(meldType, calledTile);
        _openMelds.Add(openMeld);
    }

    public virtual List<OpenMeld> GetOpenMelds()
    {
        return _openMelds;
    }
    #endregion

    #region GetTiles
    public virtual List<Tile> GetAllTiles(Tile extraTile)
    {
        List<Tile> outputList = new();
        CompileAllTiles(outputList);
        AddExtraTileToOutputList(extraTile, outputList);
        return _handSorter.SortHand(outputList);
    }

    public virtual List<Tile> GetClosedTiles()
    {
        return _closedTiles;
    }

    public List<Tile> GetTilesInHand()
    {
        List<Tile> t = _closedTiles.GetRange(0, _closedTiles.Count);
        if (_drawTile != null)
        {
            t.Add(_drawTile);
        }

        return t;
    }

    public virtual List<Tile> GetAllTiles()
    {
        List<Tile> outputList = new();
        CompileAllTiles(outputList);
        if (_drawTile != null)
        {
            AddExtraTileToOutputList(_drawTile, outputList);
        }
        return _handSorter.SortHand(outputList);
    }

    public bool ContainsTile(Tile tile)
    {
        foreach(Tile t in GetClosedTiles())
        {
            if (AreTilesEquivalent(tile, t))
                return true;
        }
        return false;
    }

    private void CompileAllTiles(List<Tile> tileList)
    {
        AddClosedTilesToOutputList(tileList);
        AddEachOpenMeldToOutputList(tileList);
    }

    private static void AddExtraTileToOutputList(Tile extraTile, List<Tile> outputList)
    {
        outputList.Add(extraTile);
    }

    private void AddEachOpenMeldToOutputList(List<Tile> outputList)
    {
        foreach (OpenMeld openMeld in _openMelds)
        {
            AddEachTileInOpenMeldToOutputList(outputList, openMeld);
        }
    }

    private static void AddEachTileInOpenMeldToOutputList(List<Tile> outputList, OpenMeld openMeld)
    {
        foreach (Tile tile in openMeld.GetTiles())
        {
            outputList.Add(tile);
        }
    }

    private void AddClosedTilesToOutputList(List<Tile> outputList)
    {
        foreach (Tile tile in _closedTiles)
        {
            outputList.Add(tile);
        }
    }
    #endregion

    #region Shanten

    public int GetShanten()
    {
        CheckShanten();
        return _shanten;
    }

    public void CheckShanten()
    {
        List<ITenpaiHand> tempTenpaiHands = new();
        foreach (ITenpaiHand tenpaiHand in _tenpaiHands)
        {
            tempTenpaiHands.Add(tenpaiHand);
        }
        ClearTenpaiHands();
        _shanten = ShantenCalculator.CalculateShanten(this);
        if (_shanten < 0)
        {
            foreach (ITenpaiHand tenpaiHand in tempTenpaiHands)
            {
                _tenpaiHands.Add(tenpaiHand);
            }
        }
        else if (_shanten > 0)
        {
            ClearWaits();
        }
    }

    #region Tenpai
    public List<ITenpaiHand> GetTenpaiHands()
    {
        return _tenpaiHands;
    }
    public List<Tile> GetWaits()
    {
        return _waits;
    }

    private void ClearTenpaiHands()
    {
        _tenpaiHands.Clear();
    }

    public void AddTenpaiHand(ITenpaiHand tenpaiHand)
    {
        _tenpaiHands.Add(tenpaiHand);
    }

    public void ClearWaits()
    {
        _waits.Clear();
    }

    public void AddWait(Tile tile)
    {
        _waits.Add(tile);
    }

    public List<Tile> GetRiichiDiscardTiles()
    {
        return RiichiChecker.CheckRiichi(this);
    }
    #endregion
    #endregion

    public StandardDiscardPile GetDiscardPile()
    {
        return _discardPile;
    }

    public void OpenHand()
    {
        _isOpen = true;
    }

    private void SortHand()
    {
        _closedTiles = _handSorter.SortHand(_closedTiles);
    }

    public void RemoveCopyOfTile(Tile calledTile)
    {
        for (int i = _closedTiles.Count - 1; i >= 0; i--)
        {
            if (IsDuplicateTile(_closedTiles[i], calledTile, i))
            {
                return;
            }
        }
    }

    public void RemoveExactCopyOfTile(Tile calledTile)
    {
        for (int i = _closedTiles.Count - 1; i >= 0; i--)
        {
            if (IsExactDuplicateTile(_closedTiles[i], calledTile, i))
                return;
        }
    }

    private bool IsDuplicateTile(Tile closedTile, Tile calledTile, int index)
    {
        if (AreTilesEquivalent(closedTile, calledTile))
        {
            _closedTiles.RemoveAt(index);
            return true;
        }
        return false;
    }

    private bool IsExactDuplicateTile(Tile closedTile, Tile calledTile, int index)
    {
        if (AreTilesExactlyEquivalent(closedTile, calledTile))
        {
            _closedTiles.RemoveAt(index);
            return true;
        }
        return false;
    }

    public virtual bool IsOpen()
    {
        return _isOpen;
    }
}