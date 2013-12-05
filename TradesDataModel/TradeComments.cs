using System;

namespace TradesDataModel
{
    
    public class TradeComments
    {
        public int CommentId { get; set; }
        public int? TradeId { get; set; }
        public string CommentLabel { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    
        public virtual Trades Trade { get; set; }
    }
}
