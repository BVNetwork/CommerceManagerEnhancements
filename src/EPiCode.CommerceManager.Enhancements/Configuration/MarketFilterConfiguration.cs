using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceManagerEnhancements.Configuration
{
    public class MarketFilterConfiguration
    {
        public static bool UseDropdownMarketFilter()
        {

            var useDropdown = false;
            var useDropdownSetting = ConfigurationManager.AppSettings["UseDropdownMarketFilter"];

            if (useDropdownSetting != null)
            {
                useDropdown = Convert.ToBoolean(useDropdownSetting);
            }

            return useDropdown;
        }
    }
}
