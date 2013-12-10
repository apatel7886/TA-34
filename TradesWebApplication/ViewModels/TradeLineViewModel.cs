﻿using System.Collections.Generic;
using System.Web.Razor.Generator;
using TradesWebApplication.DAL.EFModels;

namespace TradesWebApplication.ViewModels
{
    public class TradeLineViewModel
    {
        public Trade_Line TradeLine { get; set; }
        public Position Position { get; set; }
        public virtual ICollection<Position> Positions
        { get; set; }
        public Tradable_Thing TradeTradableThing { get; set; }
        public virtual ICollection<Tradable_Thing> TradeTradableThings { get; set; }
    }
}