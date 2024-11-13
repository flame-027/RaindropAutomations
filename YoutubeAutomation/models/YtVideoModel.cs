using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainDropAutomations.Youtube.models
{
    public class YtVideoModel
    {
        public string rawVideoUrl { get; set; }

        public string pureVideoUrl => rawVideoUrl.Split('&')[0];
    }
}
