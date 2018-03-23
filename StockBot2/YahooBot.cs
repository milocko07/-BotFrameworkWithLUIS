using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace StockBot2
{
    public class YahooBot
    {
        public static async Task<double?> GetStockRateAsync(string StockSymbol)
        {
            
            try
            {
                //string ServiceURL = $"http://finance.yahoo.com/d/quotes.csv?s={StockSymbol}&f=sl1d1nd";
                string ServiceURL = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={StockSymbol}&interval=1min&apikey=L608Z7A1QWOM2DTK";


                string ResultInCSV;
                using (WebClient client = new WebClient())
                {
                    ResultInCSV = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
                }





                var FirstLine = ResultInCSV.Split('\n')[0];
                var Price = FirstLine.Split(',')[1];

                //var FirstLine = Regex.Split(ResultInCSV, "\"1. open\": ")[1];
                //var Price = FirstLine.Split(',')[0].Trim('"');


                if (Price != null && Price.Length >= 0)
                {
                    
                    double result;
                    if (double.TryParse(Price, out result))
                    {
                        return result;
                    }

                }
                return null;
            }
            catch (WebException ex)
            {
                //handle your exception here
                throw ex;
            }
        }

        private static async Task<string> NewMethod(string ServiceURL, string ResultInCSV)
        {
            using (WebClient client = new WebClient())
            {
                ResultInCSV = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
            }

            return ResultInCSV;
        }
    }
}