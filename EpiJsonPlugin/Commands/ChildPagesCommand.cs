using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer;

namespace EpiJsonPlugin.Commands
{
    [Command(CommandName = "children")]
    public class ChildPagesCommand : ICommandTemplate
    {
        public IEnumerable<PageData> ExecuteCommand(PageBase currentPage)
        {
            var fa = new FilterAccess(EPiServer.Security.AccessLevel.Read);
            var pdc = currentPage.GetChildren(currentPage.CurrentPageLink);

            fa.Filter(pdc);

            return pdc;
        }

        public virtual string[] GetPropertyFilter()
        {
            return default(string[]);
        }

    }
}
