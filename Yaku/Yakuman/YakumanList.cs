﻿using RMU.Hands.CompleteHands;
using System.Collections.Generic;
using System.Linq;

namespace RMU.Yaku.Yakuman
{
    internal sealed class YakumanList
    {
        private readonly ICompleteHand _completeHand;
        private List<YakumanBase> _yakumanList;

        public YakumanList(ICompleteHand completeHand)
        {
            _completeHand = completeHand;
            InitializeList();
        }

        private void InitializeList()
        {
            _yakumanList = new List<YakumanBase>
            {
                new ThirteenOrphansYakuman(_completeHand),
                new ThirteenWaitThirteenOrphansYakuman(_completeHand),
                new FourConcealedTripletsYakuman(_completeHand),
                new FourConcealedTripletsSingleWaitYakuman(_completeHand),
                new BigThreeDragonsYakuman(_completeHand),
                new FourLittleWindsYakuman(_completeHand),
                new FourBigWindsYakuman(_completeHand),
                new AllHonorsYakuman(_completeHand),
                new AllTerminalsYakuman(_completeHand),
                new AllGreensYakuman(_completeHand),
                new NineGatesYakuman(_completeHand),
                new TrueNineGatesYakuman(_completeHand),
                new FourQuadsYakuman(_completeHand)
            };
        }

        public List<YakumanBase> CheckYakuman()
        {
            return _yakumanList.Where(yakuman => yakuman.Check()).ToList();
        }
    }
}
