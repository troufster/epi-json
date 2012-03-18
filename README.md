#EPi-JSON
###A simple EPiServer plugin for returning pages as JSON

This project first started out as a quick hack based on Allan's previous work at 
http://labs.episerver.com/en/Blogs/Allan/Dates/2009/3/Output-PageData-as-JSON/.
The hack was used to fill a requirement gap in a customer's intranet, where we needed a 
quick and easy way to get EPiServer page instances as JSON.

The hack has since then evolved into this easy to use, yet fully customizable plugin.

***
### 1. Installation

1. Clone/zipball this repo
2. Build with EPiServer 6 R2 
3. Drop assembly into your project

### 2. Usage

EPi-JSON comes bundled with 3 basic commands, and capability to serialize any of the default EPiServer types into 
proper, good looking, sexy JSON.

To get a page as JSON, simply append ```&json=current``` to the querystring of the page's URL

The following commands are available out of the box:

1. current - get the current page.
2. children - get the current page's children.
3. childrenids - get only the id's of the current pages children.


### 3. Customization

EPi-JSON is extensible in every single way. Have a custom property you need to serialize? Tired of .NETs ugly JSON?
Use a custom property mapper!

Want to declare your own command that only serializes certain properties of pages? No problem good Sir, Use a custom command!

#### 3.1 Property mapper

    [TypeMap(PropertyType = typeof(PropertyDate))]
    public class PropertyDateMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            var propertyDate = propertyData as PropertyDate;
            if (propertyDate != null)
            {
                var d = propertyDate.Date;
                return d != default(DateTime) ? Utils.UnixTicks(d).ToString(CultureInfo.InvariantCulture) : (-1).ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }
    }

#### 3.2 Commands

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


