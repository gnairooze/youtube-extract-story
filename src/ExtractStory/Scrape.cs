using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using static System.Net.WebRequestMethods;
using OpenQA.Selenium.Support.UI;

namespace ExtractStory
{
    internal class Scrape
    {
        public async Task<List<string>> Run(string youtubeEmbedUrl)
        {
            List<string> result = new ();

            var storyUrlRaw = ExtractStoryFromYoutubeEmbed(youtubeEmbedUrl);
            if (storyUrlRaw == null) return result;

            var storyUrlTemplate = GetStoryTemplate(storyUrlRaw);
            if (storyUrlTemplate == null) return result;

            bool imageReturned = true;
            int counter = 0;

            while (imageReturned)
            {
                var image = storyUrlTemplate.Replace("{{number}}", counter.ToString());

                var response = await CallUrl(image);

                if (response.IsSuccessStatusCode)
                {
                    result.Add(image);
                }
                else
                {
                    imageReturned = false;
                }

                counter++;
            }

            return result;
        }

        public string ExtractStoryFromYoutubeEmbed(string youtubeEmbedUrl)
        {
            //youtubeEmbedUrl = "https://www.youtube.com/embed/A24YSrwpFvU";

            var options = new ChromeOptions()
            {
                BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            };

            options.AddArguments(new List<string>() { 
                //"headless", 
                "disable-gpu" 
            });

            var browser = new ChromeDriver(options);
            browser.Navigate().GoToUrl(youtubeEmbedUrl);

            var btnLargePlay = browser.FindElement(By.ClassName("ytp-large-play-button"));
            btnLargePlay.Click();

            var btnPlay = browser.FindElement(By.ClassName("ytp-play-button"));
            btnPlay.Click();

            var progressBar = browser.FindElement(By.ClassName("ytp-progress-bar")); 
            progressBar.Click();

            Console.WriteLine("make sure tooltip is loaded. press any key to continue ...");
            Console.Read();

            Console.WriteLine("... resumed !!!");

            var element = browser.FindElement(By.ClassName("ytp-tooltip-bg"));

            var outerHtml = element.GetAttribute("outerHTML");

            string url = GetStoryUrl(outerHtml);
            browser.Close();
            browser.Dispose();

            return url;
        }

        private static async Task<HttpResponseMessage> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetAsync(fullUrl);
            return await response;
        }

        public string GetStoryUrl(string outerHtml)
        {
            /*
             sample outerHtml
            <div class="ytp-tooltip-bg" style="width: 158px; height: 90px; background: url(&quot;https://i.ytimg.com/sb/A24YSrwpFvU/storyboard3_L2/M2.jpg?sqp=-oaymwENSDfyq4qpAwVwAcABBqLzl_8DBgjd24SaBg==&amp;sigh=rs%24AOn4CLBtd_7aSKHPkC31bP3op5ZPhhps0Q&quot;) -480px -90px / 800px 450px;"><div class="ytp-tooltip-duration"/></div>
             */

            var startIndex = outerHtml.IndexOf("https");
            var endIndex = outerHtml.IndexOf("&quot;)", startIndex);
            var result = outerHtml.Substring(startIndex, endIndex - startIndex);
            result = result.Replace("&amp;", "&");
            return result;
        }

        public string GetStoryTemplate(string storyUrl)
        {
            /*
             sample story url
            https://i.ytimg.com/sb/A24YSrwpFvU/storyboard3_L2/M2.jpg?sqp=-oaymwENSDfyq4qpAwVwAcABBqLzl_8DBgjd24SaBg==&sigh=rs%24AOn4CLBtd_7aSKHPkC31bP3op5ZPhhps0Q
             */

            var lastIndex = storyUrl.LastIndexOf("storyboard3_L");
            var firstPart = storyUrl.Substring(0, lastIndex + 16);
            var lastPart = storyUrl.Substring(lastIndex + 17);

            var result = $"{firstPart}{{{{number}}}}{lastPart}";
            return result;

        }
    }
}
