using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Snip.Models
{
    public enum Language
    {
        None,
        Auto,
        Bash,
        Coffeescript,
        [Description("C++")]
        Cpp,
        [Description("C#")]
        Cs,
        Css,
        Diff,
        Dos,
        Go,
        Html,
        Ini,
        Java,
        Javascript,
        Json,
        Lua,
        Markdown,
        [Description("Objective-C")]
        Objectivec,
        Perl,
        Php,
        Python,
        Ruby,
        Scala,
        Sql,
        Vbscript,
        Xml
    }
}