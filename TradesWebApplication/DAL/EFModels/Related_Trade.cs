//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradesWebApplication.DAL.EFModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class Related_Trade
    {
        public int trade_id { get; set; }
        public int related_trade_id { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
        public Nullable<int> created_by { get; set; }
        public Nullable<System.DateTime> last_updated { get; set; }
    
        public virtual Trade Trade { get; set; }
        public virtual Trade Trade1 { get; set; }
    }
}
