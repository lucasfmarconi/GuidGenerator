using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;

namespace GuidGenerator
{
    public static class GuidGenerator
    {
        [FunctionName("GuidGenerator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string keyString = req.Query["key"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            keyString = keyString ?? data?.key;

            string responseMessage = string.IsNullOrEmpty(keyString)
                ? "This HTTP triggered function executed successfully. Pass a key in the query string or in the request body for a personalized response."
                : GenerateGuid(keyString);

            return new OkObjectResult(responseMessage);
        }

        private static string GenerateGuid(string keyString)
        {
            using(MD5 cypher = MD5.Create())
            {
                byte[] hash = cypher.ComputeHash(Encoding.Default.GetBytes(keyString));
                Guid guidResult = new Guid(hash);
                string guid = guidResult.ToString();
                dynamic uuidObj = new ExpandoObject();
                uuidObj.uuid = guid;
                return JsonConvert.SerializeObject(uuidObj);
            }
        }
    }
}
