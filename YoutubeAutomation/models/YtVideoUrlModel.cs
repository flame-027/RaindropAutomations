using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainDropAutomations.Youtube.models
{
    public class YtVideoUrlModel
    {
        public string rawCapturedVideoUrl { get; set; }

        public string pureVideoUrl => rawCapturedVideoUrl.Split('&')[0];
    }
}
