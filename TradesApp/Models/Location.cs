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
    
    public partial class Location
    {
        public Location()
        {
            this.Tradable_Thing = new HashSet<Tradable_Thing>();
        }
    
        public int location_id { get; set; }
        public string location_uri { get; set; }
        public string location_code { get; set; }
        public string location_label { get; set; }
    
        public virtual ICollection<Tradable_Thing> Tradable_Thing { get; set; }
    }
}
