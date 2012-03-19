using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EPiServer;
using EPiServer.Core;

namespace EpiJsonPlugin
{
    class MultipageCacheDependency : System.Web.Caching.CacheDependency
    {
        private readonly Timer _timer;
        private const int POLLTIME = 30000;
        private readonly List<Tuple<PageReference, int>> _pageDependencies;  

        public MultipageCacheDependency(IEnumerable<PageData> pages)
        {
            _pageDependencies = new List<Tuple<PageReference, int>>();

            foreach(var page in pages)
            {
                _pageDependencies.Add(Tuple.Create(page.PageLink, page.GetHashCode()));
            }

            _timer = new Timer(new TimerCallback(CheckDependencyCallback), this, 0, POLLTIME);
        }

        private void CheckDependencyCallback(object sender)
        {
            foreach (var pageDependency in _pageDependencies)
            {
                var currentPageHash = DataFactory.Instance.GetPage(pageDependency.Item1).GetHashCode();

                if (currentPageHash == pageDependency.Item2) continue;
               
                NotifyDependencyChanged(this, EventArgs.Empty);
                _timer.Dispose();                   
                break;
            }
            
        }
    }
}
