//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradesApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tradable_Thing
    {
        public Tradable_Thing()
        {
            this.Trade_Line = new HashSet<Trade_Line>();
        }
    
        public int tradable_thing_id { get; set; }
        public string tradable_thing_uri { get; set; }
        public Nullable<int> tradable_thing_class_id { get; set; }
        public Nullable<int> location_id { get; set; }
        public string tradable_thing_code { get; set; }
        public string tradable_thing_label { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual Tradable_Thing_Class Tradable_Thing_Class { get; set; }
        public virtual ICollection<Trade_Line> Trade_Line { get; set; }
    }
}
