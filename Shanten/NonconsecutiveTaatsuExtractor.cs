﻿using RMU.Hands.CompleteHands.CompleteHandComponents;
using RMU.Shanten.HandSplitter;
using RMU.Tiles;
using System;
using System.Collections.Generic;
using static RMU.Hands.CompleteHands.CompleteHandComponents.CompleteHandComponentFactory;

namespace RMU.Shanten;

public static class NonconsecutiveTaatsuExtractor
{
    private static List<Tile> _tiles;
    private static List<ICompleteHandComponent> _outputList;
    private static TileCollection _collection;

    private static readonly object taatsuLock = new();

    public static List<ICompleteHandComponent> ExtractNonconsecutiveTaatsu(TileCollection collection)
    {
        lock (taatsuLock)
        {
            InitializeLists(collection);
            if (CollectionIsInvalid())
            {
                return _outputList;
            }

            CheckCollectionForNonconsecutiveTaatsu();
            return _outputList;
        }
    }

    private static void CheckCollectionForNonconsecutiveTaatsu()
    {
        for (int i = _collection.GetSize() - 1; i >= 1; i--)
        {
            CheckForNonconsecutiveTaatsuFromGivenTile(ref i);
        }
    }

    private static void CheckForNonconsecutiveTaatsuFromGivenTile(ref int i)
    {
        Tile tileTwoBelow;
        try
        {
            tileTwoBelow = GetTileTwoBelow(_tiles[i]);
        }
        catch (NullReferenceException e)
        {
            return;
        }
        for (int j = i - 1; j >= 0; j--)
        {
            if (FoundNonconsecutiveTaatsuFromGivenTile(ref i, tileTwoBelow, j))
            {
                break;
            }
        }
    }

    private static bool FoundNonconsecutiveTaatsuFromGivenTile(ref int i, Tile tileTwoBelow, int j)
    {
        return TilesFormNonconsecutiveTaatsu(ref i, tileTwoBelow, j);
    }

    private static bool TilesFormNonconsecutiveTaatsu(ref int i, Tile tileTwoBelow, int j)
    {
        if (AreTilesEquivalent(tileTwoBelow, _tiles[j]))
        {
            List<Tile> tileList = new() { _tiles[j], _tiles[i] };
            ExtractTilesToNewNonconsecutiveTaatsuComponent(ref i, j, tileList);
            return true;
        }
        return false;
    }

    private static void ExtractTilesToNewNonconsecutiveTaatsuComponent(ref int i, int j, List<Tile> tileList)
    {
        ICompleteHandComponent nonconsecutiveTaatsu = CreateCompleteHandComponent(tileList, INCOMPLETE_SEQUENCE_CLOSED_WAIT);
        _outputList.Add(nonconsecutiveTaatsu);
        ExtractTiles(_collection, _tiles, i, j);
        i--;
    }

    private static bool CollectionIsInvalid()
    {
        return _tiles.Count == 0 || CollectionIsWindCollection() || CollectionIsDragonCollection();
    }

    private static bool CollectionIsDragonCollection()
    {
        return _collection.GetSuit() == DRAGON;
    }

    private static bool CollectionIsWindCollection()
    {
        return _collection.GetSuit() == WIND;
    }

    private static void InitializeLists(TileCollection collection)
    {
        _tiles = collection.GetTiles();
        _outputList = new List<ICompleteHandComponent>();
        _collection = collection;
    }

    private static void ExtractTiles(TileCollection collection, List<Tile> tiles, int i, int j)
    {
        collection.RemoveTile(tiles[i]);
        collection.RemoveTile(tiles[j]);
    }
}
