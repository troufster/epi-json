#EPi-JSON
###A simple EPiServer plugin for returning pages as JSON

This project first started out as a quick hack based on Allan's previous work at 
http://labs.episerver.com/en/Blogs/Allan/Dates/2009/3/Output-PageData-as-JSON/.
The hack was used to fill a requirement gap in a customer's intranet, where we needed a 
quick and easy way to get EPiServer page instances as JSON.

The hack has since then evolved into this easy to use, yet fully customizable plugin.

DISCLAIMER:
Please note that although used in production environments, I do not consider this project as production ready. 
The code does however work, and if you manage to find a scenario where it doesn't, please be kind enough to 
raise an issue or let me know. 

***
### 1. Features

* Proper, good looking JSON. No more __________d wrapping and ugly date objects.
* Low learning curve, just reference assembly and go
* Blazing fast. Really. JSON results are cached with dependencies using EPiServer's CacheManager.
* Fully extensible

### 2. Installation

1. Get assembly [here (0.7 64bit EPiServer 6 R2)](https://github.com/downloads/troufster/epi-json/EpiJsonPlugin.dll) or Clone/zipball this repo & Build with EPiServer 6 R2 
2. Drop assembly into your project

### 3. Usage

EPi-JSON comes bundled with 3 basic commands, and capability to serialize any of the default 
EPiServer types into proper, good looking, sexy JSON.

To get a page as JSON, simply append ```&json=current``` to the querystring of the page's URL

The following commands are available out of the box:

1. current - get the current page.
2. children - get the current page's children.
3. childrenids - get only the id's of the current pages children.


### 4. Customization

EPi-JSON is extensible in every single way. 

Have a custom property you need to serialize? Tired of .NETs ugly JSON? 
Use a custom property mapper!

Want to declare your own command that only serializes certain properties 
of pages? 
No problem good Sir, Use a custom command!

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
                return d != default(DateTime) ? UnixTicks(d).ToString(CultureInfo.InvariantCulture) : (-1).ToString(CultureInfo.InvariantCulture);
            }

            return String.Empty;
        }

        public static double UnixTicks(DateTime dt)
        {
            var d1 = new DateTime(1970, 1, 1);
            var d2 = dt.ToUniversalTime();
            var ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
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


