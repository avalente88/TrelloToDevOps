using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;

namespace TrelloToDevOps
{
    class TrelloTransformer
    {
        static void Main()
        {
            ReadEmails();
        }

        public static void ReadEmails(){

            //Get TRELLO API Key and Token
           string myUrl = "https://api.trello.com/1/lists/{LIST ID}/cards?key={KEY}&token={TOKEN}";

           HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(myUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                List<Root> roots = JsonConvert.DeserializeObject<List<Root>>(dataObjects);

                foreach(Root root in roots){
                     CreateIssueTFS(root.name, root.desc);
                    string myUrlDelete = "https://api.trello.com/1/cards/" + root.id + "?key={KEY}}&token={TOKEN}";

                    HttpClient client2 = new HttpClient();

                    // Add an Accept header for JSON format.
                    client2.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    // List data response.
                    HttpResponseMessage response2 = client2.DeleteAsync(myUrlDelete).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    if (response2.IsSuccessStatusCode)
                    {
                        
                    }
                    client2.Dispose();  
                }

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
        }

        public static void CreateIssueTFS(string title, string description)
        {
            //Get DEVOPs pat
            string pat = "{DEV OPS PAT}";
       

            var workItemTaskData = new List<dynamic>()
            {
                new
                    {
                        op = "add",
                        path = "/fields/System.Title",
                        value = title
                    }
            };

            workItemTaskData.Add(new
                    {
                        op = "add",
                        path = "/fields/System.Description",
                        value = description
                    });
            

            workItemTaskData.Add(new
                    {
                        op = "add",
                        path = "/fields/System.Tags",
                        value = "NOVO"
                    });
            
                        workItemTaskData.Add(new
                    {
                        op = "add",
                        path = "/fields/System.AssignedTo",
                        value = "{ASSIGNED TO EMAIL}"
                    });

            var workItemValue = new StringContent(JsonConvert.SerializeObject(workItemTaskData), Encoding.UTF8, "application/json-patch+json");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", pat))));


            using (HttpResponseMessage response = client.PostAsync("{AZURE DEV OPS URL}/_apis/wit/workitems/$bug?api-version=6.0", workItemValue).Result)
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
            }
        }
    }

    public class Trello    {
        public int board { get; set; } 
        public int card { get; set; } 
    }

    public class AttachmentsByType    {
        public Trello trello { get; set; } 
    }

    public class Badges    {
        public AttachmentsByType attachmentsByType { get; set; } 
        public bool location { get; set; } 
        public int votes { get; set; } 
        public bool viewingMemberVoted { get; set; } 
        public bool subscribed { get; set; } 
        public string fogbugz { get; set; } 
        public int checkItems { get; set; } 
        public int checkItemsChecked { get; set; } 
        public object checkItemsEarliestDue { get; set; } 
        public int comments { get; set; } 
        public int attachments { get; set; } 
        public bool description { get; set; } 
        public object due { get; set; } 
        public bool dueComplete { get; set; } 
        public object start { get; set; } 
    }

    public class Cover    {
        public object idAttachment { get; set; } 
        public object color { get; set; } 
        public object idUploadedBackground { get; set; } 
        public string size { get; set; } 
        public string brightness { get; set; } 
    }
    public class Root    {
        public string id { get; set; } 
        public object checkItemStates { get; set; } 
        public bool closed { get; set; } 
        public DateTime dateLastActivity { get; set; } 
        public string desc { get; set; } 
        public object descData { get; set; } 
        public object dueReminder { get; set; } 
        public string idBoard { get; set; } 
        public string idList { get; set; } 
        public List<object> idMembersVoted { get; set; } 
        public int idShort { get; set; } 
        public object idAttachmentCover { get; set; } 
        public List<object> idLabels { get; set; } 
        public bool manualCoverAttachment { get; set; } 
        public string name { get; set; } 
        public int pos { get; set; } 
        public string shortLink { get; set; } 
        public bool isTemplate { get; set; } 
        public Badges badges { get; set; } 
        public bool dueComplete { get; set; } 
        public object due { get; set; } 
        public List<object> idChecklists { get; set; } 
        public List<object> idMembers { get; set; } 
        public List<object> labels { get; set; } 
        public string shortUrl { get; set; } 
        public object start { get; set; } 
        public bool subscribed { get; set; } 
        public string url { get; set; } 
        public Cover cover { get; set; } 
    }
}
