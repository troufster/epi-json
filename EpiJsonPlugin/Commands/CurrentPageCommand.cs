using System.Collections.Generic;
using EPiServer.Core;
using EPiServer;

namespace EpiJsonPlugin.Commands
{
    [Command(CommandName="current")]
    public class CurrentPageCommand : ICommandTemplate
    {
        public IEnumerable<PageData> ExecuteCommand(PageBase currentPage)
        {
            var pageList = new List<PageData> {currentPage.CurrentPage};

            return pageList;
        }

        public virtual string[] GetPropertyFilter() {
            return default(string[]);
        }

    }
}
