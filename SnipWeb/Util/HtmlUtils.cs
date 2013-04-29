using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Snip
{
    public static class HtmlUtils
    {
        public static MvcHtmlString EditorBlockFor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expr,
            IEnumerable<SelectListItem> dropDownItems = null,
            string labelText = null,
            string autoCompleteUrl = null,
            bool useMarkdownEditor = false,
            bool hideLabel = false) where TModel : class
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='editor-block'>");

            if (!hideLabel)
            {
                sb.Append("<div class='editor-label'>");

                if (typeof(TProperty) == typeof(bool))
                {
                    sb.Append(html.EditorFor(expr));
                }

                if (labelText == null)
                {
                    sb.Append(html.LabelFor(expr));
                }
                else
                {
                    sb.Append(html.LabelFor(expr, labelText));
                }

                sb.Append("</div>");
            }

            sb.Append("<div class='editor-field");

            if (autoCompleteUrl != null)
            {
                sb.Append(" autocomplete' data-autocompleteurl='" + autoCompleteUrl);
            }
            else if (useMarkdownEditor)
            {
                sb.Append(" mdd'");
            }

            sb.Append("'>");

            var propType = typeof(TProperty);

            if (propType == typeof(bool))
            {
                // No editor, it is in the label above
            }
            else if (dropDownItems != null)
            {
                sb.Append(html.DropDownListFor(expr, dropDownItems));
            }
            else if (useMarkdownEditor)
            {
                sb.Append("<div class='mdd_toolbar'></div>");
                sb.Append(html.TextAreaFor(expr, new { @class = "mdd_editor", rows = 15 }));
            }
            else if (propType.IsEnum)
            {
                sb.Append(html.EnumDropDownListFor(expr));
            }
            else
            {
                sb.Append(html.EditorFor(expr));
            }

            sb.Append(html.ValidationMessageFor(expr));
            sb.Append("</div>");
            sb.Append("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetInputName(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1);

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
        }

        private static string GetInputName(MethodCallExpression expression)
        {
            // p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...
            MethodCallExpression methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetInputName(methodCallExpression);
            }
            return expression.Object.ToString();
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            string inputName = GetInputName(expression);
            var value = htmlHelper.ViewData.Model == null
                ? default(TProperty)
                : expression.Compile()(htmlHelper.ViewData.Model);

            return htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), value.ToString()));
        }

        public static SelectList ToSelectList(Type enumType, string selectedItem)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(item.ToString());
                var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
                var listItem = new SelectListItem
                {
                    Value = item.ToString(),
                    Text = title,
                    Selected = selectedItem == item.ToString()
                };
                items.Add(listItem);
            }

            return new SelectList(items, "Value", "Text");
        }

        public static MvcHtmlString AliasLink(this HtmlHelper html, string alias)
        {
            alias = StringUtils.GetAliasFromQualifiedName(alias);
            return MvcHtmlString.Create(string.Format(
@"<span><img class=""presenceIcon"" id=""{0}{1}"" src=""{2}"" onload=""PresenceControl('{0}@microsoft.com');"" /><a href=""http://who/is/{0}"">{3}</a></span>",
                 alias,
                 RandomUtils.GlobalRandom.Next(100000),
                 new UrlHelper(html.ViewContext.RequestContext).Content("~/Content/Images/presence/presence_16-unknown.png"),
                 alias));
        }

        public static MvcHtmlString ToAgo(this DateTime date)
        {
            return MvcHtmlString.Create(string.Format(
                "<abbr title='{0}Z'>{1}</abbr>",
                date.ToString("s"),
                date.ToRelative()));
        }
    }
}