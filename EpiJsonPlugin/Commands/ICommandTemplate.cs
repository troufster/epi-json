using System.Collections.Generic;
using EPiServer.Core;
using EPiServer;

namespace EpiJsonPlugin.Commands
{
    public interface ICommandTemplate
    {
        IEnumerable<PageData> ExecuteCommand(PageBase currentPage);
        string[] GetPropertyFilter();
    }
}
