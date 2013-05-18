﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using System.Net.Http;
//using System.Xml.Linq;

namespace Riddle
{
    struct RiddleItem
    {
        public string Title { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
    }

    class RiddleManager
    {
        const string RiddleRSS = "http://www.xhxsw88.cn/data/rss/80.xml"; 

        public async Task<XDocument> GetRiddleRssContents()
        {
            string responseBody = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                
                HttpResponseMessage response = await client.GetAsync(RiddleRSS);
                response.EnsureSuccessStatusCode();
                // Do not ReadAsString directly because the string is not encoded correctly
                // responseBody = await response.Content.ReadAsStringAsync();
                Encoding encode = Encoding.GetEncoding("gb2312");
                var responseStream = await response.Content.ReadAsStreamAsync();
                var encodedStream = new StreamReader(responseStream, encode);
                responseBody = encodedStream.ReadToEnd();
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Message:{0}", e.Message);
            }
            XDocument doc = string.IsNullOrEmpty(responseBody) ? null : XDocument.Parse(responseBody);
            var rtnTask = Task.Factory.StartNew(() => doc);
            return await rtnTask;
        }

        // Stub function for testing
        public static async Task<XDocument> LoadXml()
        {
            StorageFolder storageFolder = await Package.Current.InstalledLocation.GetFolderAsync("Test");
            StorageFile storageFile = await storageFolder.GetFileAsync("TestRss.xml");

            XmlLoadSettings loadSettings = new XmlLoadSettings();
            loadSettings.ProhibitDtd = false;
            loadSettings.ResolveExternals = false;
                
            XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(storageFile, loadSettings);
            XDocument doc = XDocument.Parse(xmlDoc.GetXml());
            var rtnTask = Task.Factory.StartNew(() => doc);
            return await rtnTask;
        }

        public static async Task<List<RiddleItem>> ParseRiddles(XDocument doc)
        {
            var result = doc.Descendants("channel").Elements("item")
                         .Select(item => new RiddleItem
                         {
                             Title = item.Element("title").Value,
                             Link = item.Element("link").Value,
                             PubDate = item.Element("pubDate").Value,
                             Category = item.Element("category").Value
                         });
            var rtnTask = Task.Factory.StartNew(() => result.ToList());
            return await rtnTask;
        }
    }
}
