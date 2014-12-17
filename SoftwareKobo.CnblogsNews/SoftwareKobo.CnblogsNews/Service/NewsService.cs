using System.Net;
using System.Text.RegularExpressions;
using Windows.Web.Http.Filters;
using AngleSharp;
using AngleSharp.DOM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NewsService
    {
        public static string NewsBaseUrl;
    }
}