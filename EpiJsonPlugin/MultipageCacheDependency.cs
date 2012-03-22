using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EPiServer;
using EPiServer.Core;

namespace EpiJsonPlugin
{
    class MultipageCacheDependency : System.Web.Caching.CacheDependency
    {
        private readonly Timer _timer;
        private const int POLLTIME = 120000;
        private readonly List<Tuple<PageReference, int>> _pageDependencies;  

        public MultipageCacheDependency(IEnumerable<PageData> pages)
        {
            _pageDependencies = new List<Tuple<PageReference, int>>();

            foreach(var page in pages)
            {
                _pageDependencies.Add(Tuple.Create(page.PageLink, page.GetHashCode()));
            }

            _timer = new Timer(CheckDependencyCallback, this, 0, POLLTIME);
        }

        private void CheckDependencyCallback(object sender)
        {
            foreach (var pageDependency in _pageDependencies)
            {
                var currentPageHash = DataFactory.Instance.GetPage(pageDependency.Item1).GetHashCode();

                if (currentPageHash == pageDependency.Item2) continue;
               
                Invalidate();     
                break;
            }

            var listparent = DataFactory.Instance.GetPage(_pageDependencies.First().Item1).ParentLink;
            var currcount = DataFactory.Instance.GetChildren(listparent).Count;
            
            if(_pageDependencies.Count != currcount)
            {
                Invalidate();
            }

        }

        private void Invalidate()
        {
            NotifyDependencyChanged(this, EventArgs.Empty);
            _timer.Dispose();    
        }
    }
}
