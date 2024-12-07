using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainDropAutomations.Youtube.Models
{
    public class UrlModel
    {
        public string rawCapturedUrl { get; set; }

        public string PureUrl
        {
            get { return rawCapturedUrl.Split('?')[0]; }
        }

        public string UrlAndFirstPram
        {
            get { return rawCapturedUrl.Split('&')[0]; }
        }
    }
}
