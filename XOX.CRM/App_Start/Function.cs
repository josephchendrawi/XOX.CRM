using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Reflection;
using XOX.CRM.Lib;

namespace CRM
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, Type enumType, object htmlAttributes = null, GroupPosition? position = GroupPosition.Right, bool? newLine = false)
        {
            var typeOfProperty = enumType;
            if (!typeOfProperty.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));
            

            var sb = new StringBuilder();
            var htmlFormat = "{1} {2}";
            if (position == GroupPosition.Left)
            {
                htmlFormat = "{2} {1}";
            }
            if (newLine == true)
            {
                htmlFormat = "<div>" + htmlFormat + "</div>";
            }

            var selected_value = ModelMetadata.FromLambdaExpression(
                modelExpression, htmlHelper.ViewData
            ).Model;

            List<SelectListItem> enumValues = new List<SelectListItem>();
            foreach (var v in Enum.GetValues(typeOfProperty))
            {
                var radio = htmlHelper.RadioButtonFor(modelExpression, (int)v, htmlAttributes).ToHtmlString();
                
                sb.AppendFormat(
                    htmlFormat,
                    v,
                    HttpUtility.HtmlEncode(v.GetType().GetMember(v.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description),
                    radio
                );
            }
                        
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, Type enumType, object htmlAttributes = null, string optionLabel = null)
        {
            var typeOfProperty = enumType;
            if (!typeOfProperty.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));

            List<SelectListItem> enumValues = new List<SelectListItem>();
            foreach (var v in Enum.GetValues(typeOfProperty))
            {
                var text = v.GetType().GetMember(v.ToString()).First().GetCustomAttribute<DescriptionAttribute>();
                enumValues.Add(new SelectListItem()
                {
                    Text = text == null ? v.ToString() : text.Description,
                    Value = ((int)v).ToString()
                });
            }

            return htmlHelper.DropDownListFor(modelExpression, enumValues, optionLabel, htmlAttributes);
        }


        public static MvcHtmlString DropDownListForKeyPairList<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, string LookupKey, object htmlAttributes = null, string optionLabel = null)
        {
            ICommonService CommonService = new CommonService();
            var lookup = new LookupVO()
            {
                LookupKey = LookupKey
            };
            CommonService.GetLookupValues(lookup);

            if (lookup.KeyValues.Count() < 1)
                throw new ArgumentException("Lookup Key is not correct.");

            List<SelectListItem> ListItem = new List<SelectListItem>();
            foreach (var v in lookup.KeyValues)
            {
                ListItem.Add(new SelectListItem()
                {
                    Text = v.Key,
                    Value = v.Value
                });
            }

            return htmlHelper.DropDownListFor(modelExpression, ListItem, optionLabel, htmlAttributes);
        }

    }
}