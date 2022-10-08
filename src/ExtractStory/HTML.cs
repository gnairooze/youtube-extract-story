using Mustache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ExtractStory
{
    internal class HTML:IOutput
    {

        public void Write(Model.YoutubeModel input)
        {
            var createdate = new DateTime(DateTime.Now.Ticks);
            var createtext = createdate.ToString("yyyy-MM-dd-HH-mm-fff");

            var filename = $"output/{createtext}.html";

            HtmlFormatCompiler compiler = new HtmlFormatCompiler();
            var template = File.ReadAllText(@"templates\youtube-story.html");

            var generator = compiler.Compile(template);
            var html = generator.Render(new
            {
                YoutubeUrl = input.Url,
                ImagesCount = input.Images.Count,
                Images = input.Images
            });

            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            File.WriteAllText(filename, html);
        }

    }
}
