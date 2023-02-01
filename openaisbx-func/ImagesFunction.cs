using System;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;

namespace openaisbx
{
    public static class ImagesFunction
    {
        [FunctionName("Images")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (req.Method== "GET")
            {
                return await HandleHttpGet(log, req);
            }
            else if (req.Method== "POST")
            {
                return await HandleHttpPost(log, req);
            }
            else
            {
                return new BadRequestObjectResult("not supported.");
            }
        }

        public static async Task<IActionResult> HandleHttpGet(ILogger log, HttpRequest req)
        {
            string id = req.Query["id"];
            var query_string = (id == null) ? "SELECT TOP 5 * FROM images" : "SELECT * FROM images WHERE image_id = @image_id";
            Dictionary<string, string> query_params = (id == null) ? null : new Dictionary<string, string>{{ "image_id", id }};
            List<Image> images = await TDatabaseHelper<List<Image>>.ExecuteQuery(log, query_string, query_params, (reader) => {
                return Image.MapReaderToList(reader);
            });
            return new OkObjectResult(images);
        }
        public static async Task<IActionResult> HandleHttpPost(ILogger log, HttpRequest req)
        {
            dynamic requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Image>(requestBody as string);
            string blob_link = data?.blob_link;
            string prompt = data?.prompt;
            if (blob_link == null || prompt == null)
            {
                string blob_link_null = (blob_link != null) ? "" : " null param 'blob_link'";
                string prompt_null = (prompt != null) ? "" : " null param 'prompt'.";
                return new BadRequestObjectResult("Invalid request." + blob_link_null + prompt_null);
            }
            else
            {
                string new_image_query = "INSERT INTO images (image_id, blob_link, prompt, date_created) VALUES (NEWID(), @blob_link, @prompt, GETDATE())";
                Dictionary<string, string> query_params = new Dictionary<string, string>
                    {
                        { "blob_link", blob_link },
                        { "prompt", prompt }
                    };
                int rows = await DatabaseHelper.ExecuteNonQuery(log, new_image_query, query_params);
                return new OkObjectResult(rows);
            }
        }

    }
}
