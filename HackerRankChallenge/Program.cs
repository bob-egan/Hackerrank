using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace HackerRankChallenge
{
    class Program
    {
        private static string getUrl(string txnType, int page)
        {
            return $"https://jsonmock.hackerrank.com/api/transactions/search?txnType={txnType}&page={page}";
        }
        private static ResponseObject GetResponse(WebRequest req)
        {
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return new JavaScriptSerializer().Deserialize<ResponseObject>(reader.ReadToEnd());
            }
        }


        public static List<List<int>> getTxnAmountByLocation(int locationId, string txnType)
        {
            var totalPages = 1;
            var resp = new ResponseObject();

            var url = getUrl(txnType, 1);
            var req = WebRequest.Create(url);
            resp = GetResponse(req);
            totalPages = resp.totalPages;
            var data = resp.data.Where(x => x.location.id == locationId).Select(x => new ResultObject { userId = x.userId, amt = Convert.ToDecimal(x.amount.Replace("$", string.Empty).Replace(",", string.Empty)) }).ToList();

            for (var i = 2; 1 <= totalPages; i++)
            {
                url = getUrl(txnType, i);
                req = WebRequest.Create(url);
                resp = GetResponse(req);
                data.AddRange(resp.data.Where(x => x.location.id == locationId).Select(x => new ResultObject { userId = x.userId, amt = Convert.ToDecimal(x.amount.Replace("$", string.Empty).Replace(",", string.Empty)) }));
            }

            var result = new List<ResultObject>();
            data.OrderBy(x => x.userId).ToList().ForEach(x =>
            {
                x.amt = data.Where(r => r.userId == x.userId).Sum(s => s.amt);
                if (!result.Any(t => t.userId == x.userId))
                {
                    result.Add(x);
                }
            });

            return result.Select(x => new List<int> { x.userId, Convert.ToInt32(Math.Round(x.amt)) }).ToList();
        }

        static void Main(string[] args)
        {
            for (var i = 0; i < 10; i++)
            {
                var res = getTxnAmountByLocation(i, "debit");
                if (res.Any())
                {
                    Console.WriteLine($"Location: {i}");
                    res.ForEach(x => Console.WriteLine(x[0] + "," + x[1]));
                }
            }
        }
    }
}
