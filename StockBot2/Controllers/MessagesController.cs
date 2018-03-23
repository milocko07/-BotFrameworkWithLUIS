using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace StockBot2
{

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Messagess")
            {
                string StockRateString;
                HidenFunction();
                StockLUIS StLUIS = await GetEntityFromLUIS(message.Text);
                if (StLUIS.intents.Count() > 0)
                {
                    switch (StLUIS.intents[0].intent)
                    {
                        case "StockPrice":
                            StockRateString = await GetStock(StLUIS.entities[0].entity);
                            break;
                        case "StockPrice2":
                            StockRateString = await GetStock(StLUIS.entities[0].entity);
                            break;
                        default:
                            StockRateString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    StockRateString = "Sorry, I am not getting you...";
                }

                // return our reply to the user
                return message.CreateReplyMessage(StockRateString);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private void HidenFunction()
        {

        }

        private static async Task<StockLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            StockLUIS Data = new StockLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=7f626790-38d6-4143-9d46-fe85c56a9016&subscription-key=09f80de609fa4698ab4fe5249321d165&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<StockLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }


        private async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await YahooBot.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReplyMessage("Hello");
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}