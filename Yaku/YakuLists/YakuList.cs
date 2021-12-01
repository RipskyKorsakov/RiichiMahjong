﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RMU.Yaku.YakuLists
{
    public static class YakuList
    {
        private static List<AbstractYaku> _yakuList = new List<AbstractYaku>
        {
            ALL_SIMPLES,
            ALL_TERMINALS_AND_HONORS,
            FULL_FLUSH,
            GREEN_DRAGON,
            RED_DRAGON,
            WHITE_DRAGON
        };

        public static List<AbstractYaku> GetYakuList()
        {
            return _yakuList;
        }

        public static readonly AbstractYaku ALL_SIMPLES = new AllSimples();
        public static readonly AbstractYaku ALL_TERMINALS_AND_HONORS = new AllTerminalsAndHonors();
        public static readonly AbstractYaku FULL_FLUSH = new FullFlush();
        public static readonly AbstractYaku GREEN_DRAGON = new GreenDragon();
        public static readonly AbstractYaku RED_DRAGON = new RedDragon();
        public static readonly AbstractYaku WHITE_DRAGON = new WhiteDragon();
    }
}
