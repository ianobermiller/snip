using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Snip
{
    public static class DateTimeExtensions
    {
        public static string ToRelative(this DateTime time)
        {
            return (DateTime.UtcNow - time).ToRelative();
        }
    }
}
