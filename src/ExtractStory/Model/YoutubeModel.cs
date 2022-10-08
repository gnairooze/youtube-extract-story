using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractStory.Model
{
    internal class YoutubeModel
    {
        public string Url { get; set; }
        public List<Image> Images {get; private set;}

        public YoutubeModel()
        {
            Images = new List<Image>();
        }
    }
}
