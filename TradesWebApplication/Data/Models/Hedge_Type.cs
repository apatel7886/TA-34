//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradesWebApplication.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hedge_Type
    {
        public Hedge_Type()
        {
            this.Trade_Instruction = new HashSet<Trade_Instruction>();
        }
    
        public int hedge_id { get; set; }
        public string hedge_label { get; set; }
    
        public virtual ICollection<Trade_Instruction> Trade_Instruction { get; set; }
    }
}
