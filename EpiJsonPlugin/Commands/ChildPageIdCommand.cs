﻿namespace EpiJsonPlugin.Commands {

    [Command(CommandName = "childrenids")]
    public class ChildPageIdCommand : ChildPagesCommand
    {
        public override string[] GetPropertyFilter()
        {
            return new[] { "PageLink" };
        }
    }
}
