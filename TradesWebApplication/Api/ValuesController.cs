﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TradesWebApplication.DAL;
using TradesWebApplication.DAL.EFModels;
using TradesWebApplication.ViewModels;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TradesWebApplication.Api
{
    public class ValuesController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string value)
        {

            if (ModelState.IsValid)
            {
                var vm = new TradesCreationViewModel();

                string jsonData = value;

                vm = JsonConvert.DeserializeObject<TradesCreationViewModel>(value);
                try
                {
                    PersistToDb(vm);
                }
                catch (DataException ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new JsonContent(new
                        {
                            Success = true, //error
                            Message = "Fail", //return exception
                            result = "Database Exception occured: " + ex.InnerException.ToString()
                        })
                    };
                }

               
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new JsonContent(new
                    {
                        Success = true, //error
                        Message = "Success", //return exception
                        result = "Trade sucessfully created"
                    })
                };
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new JsonContent(new
                {
                    Success = false, //error
                    Message = "Fail", //return exception
                    result = "Trade creation failed"
                })
            };
        }

        private void PersistToDb(TradesCreationViewModel vm)
        {
            //Create Trade
            Trade trade = new Trade();
            trade.service_id = vm.service_id;
            trade.length_type_id = vm.length_type_id;
            trade.relativity_id = vm.relativity_id;
            //only for related trades
            if (trade.relativity_id == 2) //2: benchmark is relative
            {
                if (vm.benchmark_id > 0)
                {
                    trade.benchmark_id = vm.benchmark_id;
                }
                
            }            
            trade.created_on = trade.last_updated = DateTime.Now;
            trade.trade_label = vm.trade_label;
            trade.trade_editorial_label = vm.trade_editorial_label;
            trade.currency_id = vm.currency_id;
            //TODO: createdby
            //STATUS Always Visible on create
            trade.status = 1; //1 is Visible, 2: Invisible, 3: Deleted
            unitOfWork.TradeRepository.InsertTrade(trade);
            unitOfWork.Save();
            var newTradeId = trade.trade_id;
            //TODO: verify uri
            trade.trade_uri = @"http://data.emii.com/bca/trades/" + newTradeId + ">";


            //Add groups
            foreach (var grp in vm.tradegroups)
            {
                var lineGroup = new Trade_Line_Group();
                lineGroup.trade_line_group_type_id = grp.trade_line_group_type_id;
                lineGroup.trade_line_group_editorial_label = grp.trade_line_group_editorial_label;
                lineGroup.trade_line_group_label = grp.trade_line_group_label;
                unitOfWork.TradeLineGroupRepository.Insert(lineGroup);
                unitOfWork.Save();
                var grpId = lineGroup.trade_line_group_id;
                //TODO: verify uri
                lineGroup.trade_line_group_uri = @"http://data.emii.com/bca/trade-line-groups/" + grpId + ">";

                //Add tradelines
                foreach (var line in grp.tradeLines)
                {
                    var tradeLine = new Trade_Line();
                    tradeLine.trade_line_group_id = grpId; //this groupID
                    tradeLine.trade_id = newTradeId; //this trade
                    tradeLine.position_id = line.position_id;
                    tradeLine.tradable_thing_id = line.tradable_thing_id;
                    //?trade_line_editorial_label
                    //?trade_line_label
                    //TODO: createdby
                    tradeLine.created_on = tradeLine.last_updated = DateTime.Now;
                    unitOfWork.TradeLineRepository.Insert(tradeLine);
                    unitOfWork.Save();
                    var newTradeLineId = tradeLine.trade_line_id;
                    //TODO: verify uri
                    tradeLine.trade_line_uri = @"http://data.emii.com/bca/trade-lines/" + newTradeLineId + ">";
                }
                
            }

            // trade instructions
            var tradeInstruction = new Trade_Instruction
            {
                trade_id = newTradeId,
                instruction_entry = vm.instruction_entry,
                instruction_entry_date = DateTime.Parse(vm.instruction_entry_date),
                instruction_exit = vm.instruction_exit
            };
            if (!String.IsNullOrEmpty(vm.instruction_exit_date))
            {
                tradeInstruction.instruction_exit_date = DateTime.Parse(vm.instruction_exit_date);
            }
            tradeInstruction.instruction_type_id = vm.instruction_type_id;
            tradeInstruction.instruction_label = vm.instruction_label;
            tradeInstruction.hedge_id = vm.hedge_id;
            tradeInstruction.created_on = tradeInstruction.last_updated = DateTime.Now;
            unitOfWork.TradeInstructionRepository.Insert(tradeInstruction);

            // related trades, TODO:
            if (vm.related_trade_ids != null)
            {
                foreach (var i in vm.related_trade_ids)
                {
                    var relatedTrade = new Related_Trade();
                    relatedTrade.trade_id = newTradeId;
                    relatedTrade.related_trade_id = i;
                    relatedTrade.created_on = relatedTrade.last_updated = DateTime.Now;
                    //TODO: created by
                    unitOfWork.RelatedTradeRepository.Insert(relatedTrade);
                }
            }
               
            //TODO: where does this go, which tradePerfomance
            string apl_func = vm.apl_func;
            bool isTradePerfomanceCreated = false;

            if (!String.IsNullOrEmpty(vm.mark_to_mark_rate))
            {
                var markTR = new Track_Record
                {
                    trade_id = newTradeId,
                    track_record_type_id = 1,
                    track_record_value = decimal.Parse(vm.mark_to_mark_rate)
                };
                //TODO: NO field exists!! interestTR.created_on = 
                markTR.last_updated = DateTime.Now;
                unitOfWork.TrackRecordRepository.Insert(markTR);

            }
            
            if (!String.IsNullOrEmpty(vm.interest_rate_diff))
            {
                var interestTR = new Track_Record
                {
                    trade_id = newTradeId,
                    track_record_type_id = 2,
                    track_record_value = decimal.Parse(vm.interest_rate_diff)
                };
                //TODO: NO field exists!! interestTR.created_on = 
                interestTR.last_updated = DateTime.Now;
                unitOfWork.TrackRecordRepository.Insert(interestTR);
            }

            // absolute performance
            
            var abs_measure_id = vm.abs_measure_type_id;
            if (abs_measure_id != null)
            {
                var absPerformance = new Trade_Performance();
                absPerformance.trade_id = newTradeId;
                var abs_measure_type_id = (int)abs_measure_id;
                absPerformance.measure_type_id = abs_measure_type_id;
                if (abs_measure_type_id == 2)
                {
                    absPerformance.return_currency_id = vm.abs_currency_id;
                }
                absPerformance.return_value = vm.abs_return_value;
                isTradePerfomanceCreated = true;
                if(!String.IsNullOrEmpty(apl_func))
                {
                    absPerformance.return_apl_function = apl_func;
                }
                absPerformance.created_on = absPerformance.last_updated = DateTime.Now;
                unitOfWork.TradePerformanceRepository.Insert(absPerformance);
            }

            // relative performance
            var rel_measure_id = vm.rel_measure_type_id;
            if (rel_measure_id != null)
            {
                var relPerformance = new Trade_Performance();
                relPerformance.trade_id = newTradeId;
                var rel_measure_type_id = (int)rel_measure_id;
                relPerformance.measure_type_id = rel_measure_type_id;
                if (rel_measure_type_id == 2)
                {
                    relPerformance.return_currency_id = vm.rel_currency_id;
                }
                relPerformance.return_value = vm.rel_return_value;
                if (vm.return_benchmark_id != null && vm.return_benchmark_id > 0)
                {
                    relPerformance.return_benchmark_id = vm.return_benchmark_id;
                }               
                isTradePerfomanceCreated = true;
                if (!String.IsNullOrEmpty(apl_func))
                {
                    relPerformance.return_apl_function = apl_func;
                }
                relPerformance.created_on = relPerformance.last_updated = DateTime.Now;
                unitOfWork.TradePerformanceRepository.Insert(relPerformance);
            }

            //TODO: Verify if creating empty trade performacne for apl_func
            isTradePerfomanceCreated = true;
            if (!String.IsNullOrEmpty(apl_func) && 
                !isTradePerfomanceCreated)
            {
                var tradePerformance = new Trade_Performance();
                tradePerformance.trade_id = newTradeId;
                tradePerformance.return_apl_function = apl_func;
                unitOfWork.TradePerformanceRepository.Insert(tradePerformance);
            }

            if (!String.IsNullOrEmpty(vm.comments))
            {
                var comment = new Trade_Comment
                {
                    trade_id = newTradeId, 
                    comment_label = vm.comments, 
                };
                comment.created_on = comment.last_updated = DateTime.Now;
                unitOfWork.TradeCommentRepository.Insert(comment);
            }
            unitOfWork.Save();
           
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }


        // GET api/<controller>/<action>
        [System.Web.Http.HttpGet]
         public IEnumerable<string> Test()
        {
            var values = new[] { "John", "Pete", "Ben" };

            return values;
   
       }

       public class JsonContent : HttpContent {

    private readonly MemoryStream _Stream = new MemoryStream();
    public JsonContent(object value) {

        Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var jw = new JsonTextWriter( new StreamWriter(_Stream));
        jw.Formatting = Formatting.Indented;
        var serializer = new JsonSerializer();
        serializer.Serialize(jw, value);
        jw.Flush();
        _Stream.Position = 0;

    }
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) {
        return _Stream.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length) {
        length = _Stream.Length;
        return true;
    }
}


    }
}