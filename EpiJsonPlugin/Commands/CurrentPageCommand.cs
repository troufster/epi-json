using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer;

namespace EpiJsonPlugin.Commands
{
    [Command(CommandName="current")]
    public class CurrentPageCommand : ICommandTemplate
    {
        public IEnumerable<PageData> ExecuteCommand(PageBase currentPage)
        {
            var pageList = new List<PageData>();

            pageList.Add(currentPage.CurrentPage);

            return pageList;
        }

        public virtual string[] GetPropertyFilter() {
            return default(string[]);
        }

    }
}
