﻿using RMU.Tiles;
using static RMU.Globals.Algorithms.CountingSortForCollections;

namespace RMU.Shanten.HandSplitter;

public static class HandSplitter
{
    private static TileCollection
        _manCollection,
        _pinCollection,
        _souCollection,
        _windCollection,
        _dragonCollection;

    private static readonly object splitLock = new();

    public static List<TileCollection> SplitHandBySuit(List<Tile> hand)
    {
        lock (splitLock)
        {
            return LockedSplitHandBySuit(hand);
        }
    }

    private static List<TileCollection> LockedSplitHandBySuit(List<Tile> hand)
    {
        CreateNewTileCollections();
        foreach (Tile tile in hand)
        {
            switch (tile.GetSuit())
            {
                case MAN:
                    _manCollection.AddTile(tile);
                    break;
                case PIN:
                    _pinCollection.AddTile(tile);
                    break;
                case SOU:
                    _souCollection.AddTile(tile);
                    break;
                case WIND:
                    _windCollection.AddTile(tile);
                    break;
                case DRAGON:
                    _dragonCollection.AddTile(tile);
                    break;
            }
        }
        return new List<TileCollection>
        {
            SortCollection(_manCollection),
            SortCollection(_pinCollection),
            SortCollection(_souCollection),
            SortCollection(_windCollection),
            SortCollection(_dragonCollection)
        };
    }

    private static void CreateNewTileCollections()
    {
        _manCollection = new TileCollection(MAN);
        _pinCollection = new TileCollection(PIN);
        _souCollection = new TileCollection(SOU);
        _windCollection = new TileCollection(WIND);
        _dragonCollection = new TileCollection(DRAGON);
    }
}
