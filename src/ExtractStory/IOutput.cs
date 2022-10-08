using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractStory
{
    internal interface IOutput
    {
        void Write(Model.YoutubeModel input);
    }
}
