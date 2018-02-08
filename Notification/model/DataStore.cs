using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;

namespace Notification
{
    public class DataStore
    {
        public long lastDate = 0;
        public List<EventData> items = new List<EventData>();

        public static async Task<int> uploadEvent(EventData data, bool useAuthor = false, string author = "", string password = "")
        {
            int res = 0;
            try
            {
                if (data.isLocalImage())
                {
                    res=await sendEventWithImage(data, useAuthor, author, password);
                }
                else
                {
                    res=await sendEvent(data, useAuthor, author, password);
                }
            }
            catch (Exception e)
            {
                res = -1000;
            }
            return res;
        }

        private async static Task<int> sendEventWithImage(EventData data, bool useAuthor = false, string author = "", string password = "")
        {
            int res = 0;
            if (File.Exists(data.image))
            {

                try
                {
                    Uri uri = new Uri(Constants.API_URI + Constants.API_UPLOAD_EVENT_WITH_IMAGE);
                    HttpClient httpClient = new HttpClient();
                    MultipartFormDataContent form = new MultipartFormDataContent();

                    form.Add(new StringContent(data.name), "name");
                    form.Add(new StringContent(data.kind), "kind");
                    form.Add(new StringContent(data.date.ToString()), "date");
                    form.Add(new StringContent(data.duration.ToString()), "duration");
                    //form.Add(new StringContent(data.image), "image");
                    form.Add(new StringContent(data.uri), "uri");
                    form.Add(new StringContent(data.info), "info");
                    form.Add(new StringContent(data.shortInfo), "shortInfo");
                    form.Add(new StringContent("#" + data.kindColor.ToString("X")), "color");
                    if (useAuthor)
                    {
                        form.Add(new StringContent(author), "login");
                        form.Add(new StringContent(password), "password");
                    }


                    FileStream fs = File.OpenRead(data.image);


                    var streamContent = new StreamContent(fs);
                    streamContent.Headers.Add("Content-Type", "application/octet-stream");
                    //Content-Disposition: form-data; name="file"; filename="C:\B2BAssetRoot\files\596090\596090.1.mp4";
                    streamContent.Headers.Add("Content-Disposition", "form-data; name=\"picture\"; filename=\"" + Path.GetFileName(data.image) + "\"");
                    form.Add(streamContent, "picture", Path.GetFileName(data.image));
                    //form.Add(new FileStreamConten(imagebytearraystring, 0, imagebytearraystring.Length), "profile_pic", "hello1.jpg");
                    HttpResponseMessage response = await httpClient.PostAsync(uri, form);

                    response.EnsureSuccessStatusCode();
                    httpClient.Dispose();
                    string sd = response.Content.ReadAsStringAsync().Result;
                }
                catch
                {
                    res = -900;
                }
            }
            else
            {
                res = -800;
            }
            return res;
        }

        private async static Task<int> sendEvent(EventData data, bool useAuthor = false, string author = "", string password = "")
        {
            int res = 0;
            try
            {
                Uri uri = new Uri(Constants.API_URI + Constants.API_UPLOAD_EVENT);
                HttpClient httpClient = new HttpClient();
                //  MultipartFormDataContent form = new MultipartFormDataContent();
                var a =  (useAuthor) ?
                new[]  {
                    new KeyValuePair<string,string>("name",data.name ),
                    new KeyValuePair<string,string>("kind",data.kind ),
                    new KeyValuePair<string,string>("date",data.date.ToString()),
                    new KeyValuePair<string,string>("duration",data.duration.ToString()),
                    new KeyValuePair<string,string>("image",data.image ),
                    new KeyValuePair<string,string>("uri",data.uri ),
                    new KeyValuePair<string,string>("info",data.info ),
                    new KeyValuePair<string,string>("shortInfo",data.shortInfo ),
                    new KeyValuePair<string,string>("color","#"+data.kindColor.ToString("X") ),
                    new KeyValuePair<string,string>("login",author),
                    new KeyValuePair<string,string>("password",password )
                }
                :
                    new[]  {
                    new KeyValuePair<string,string>("name",data.name ),
                    new KeyValuePair<string,string>("kind",data.kind ),
                    new KeyValuePair<string,string>("date",data.date.ToString()),
                    new KeyValuePair<string,string>("duration",data.duration.ToString()),
                    new KeyValuePair<string,string>("image",data.image ),
                    new KeyValuePair<string,string>("uri",data.uri ),
                    new KeyValuePair<string,string>("info",data.info ),
                    new KeyValuePair<string,string>("shortInfo",data.shortInfo ),
                    new KeyValuePair<string,string>("color","#"+data.kindColor.ToString("X") ) };

                var form = new FormUrlEncodedContent(a);
                


                //   form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Length), "profile_pic", "hello1.jpg");
                HttpResponseMessage response = await httpClient.PostAsync(uri, form);

                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string sd = response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                res = -900;
            }
            return res;
        }

        public Task<DataStore> updateListAsync(bool needLoad = false)
        {
            var result = this;
            return Task.Run(() =>
            {
                var date = (needLoad) ? 0 : lastDate;
                return result.checkList(date) ?? this;
                //if (needLoad || !result.checkLastDate(lastDate))
                //{
                //    return result.getData(new Uri(Constants.API_URI + Constants.API_EVENT_LIST));
                //}
                //else
                //    return result;
            });
        }

        private DataStore checkList(long date)
        {
            Uri uri = new Uri(Constants.API_URI + Constants.API_CHECK_LOAD(date));
            //   Console.Out.WriteLine(uri);
            try
            {
                //HttpWebRequest webrequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
                //using (HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse())
                //{
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {

                //    }
                //}
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                var ds = webClient.DownloadString(uri);
                if (ds != null && ds.Length > 0)
                {
                    //    Console.Out.WriteLine(ds);
                    JObject result = JObject.Parse(ds);
                    return extractStore(result);
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                //    Console.Out.WriteLine("Error");
                //     Console.Out.WriteLine(e.StackTrace);
                return null;//Если запрос не удался вернём true
            }
        }

        private bool checkLastDate(long date)
        {
            Uri uri = new Uri(Constants.API_URI + Constants.API_CHECK_DATE(date));
            Console.Out.WriteLine(uri);
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
                using (HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //    Console.Out.WriteLine("Error");
                //   Console.Out.WriteLine(e.StackTrace);
                return true;//Если запрос не удался вернём true
            }
        }

        private DataStore getData(Uri uri)
        {
            Console.Out.WriteLine(uri);
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                JObject result = JObject.Parse(webClient.DownloadString(uri));
                DataStore store = extractStore(result);
                return store;
            }
            catch
            {
                //    Console.Out.WriteLine("Error");
                return this;
            }
        }



        private DataStore extractStore(JObject result)
        {
            var store = new DataStore();
            store.lastDate = Convert.ToInt64(result["time"]);
            JArray arr = (JArray)result["list"];
            foreach (var a in arr)
            {
                var item = new EventData();
                opt(a, "id", ref item.id);
                opt(a, "date", ref item.date);
                opt(a, "duration", ref item.duration);
                opt(a, "image", ref item.image);
                opt(a, "info", ref item.info);
                opt(a, "kind", ref item.kind);
                optColor(a, "color", ref item.kindColor);
                opt(a, "name", ref item.name);
                opt(a, "shortInfo", ref item.shortInfo);
                opt(a, "uri", ref item.uri);
                store.items.Add(item);
            }

            return store;
        }

        public void opt(JToken token, String name, ref long value)
        {
            try
            {
                value = Convert.ToInt64(token[name]);
            }
            catch { }
        }

        public void optColor(JToken token, String name, ref int value)
        {
            try
            {
                value = System.Drawing.ColorTranslator.FromHtml((string)token[name]).ToArgb();
            }
            catch { }
        }

        public List<EventData> newItems(long now, DataStore newStore)
        {
            //   var rm= this.items.RemoveAll(x => x.date < now);
            return newStore.items.FindAll(x => x.date > now && !this.items.Exists(y => y.id == x.id));
        }

        public void opt(JToken token, String name, ref int value)
        {
            try
            {
                value = Convert.ToInt32(token[name]);
            }
            catch { }
        }

        public void opt(JToken token, String name, ref string value)
        {
            try
            {
                value = (string)token[name];
            }
            catch { }
        }

    }
}
