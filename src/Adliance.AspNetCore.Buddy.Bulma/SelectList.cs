using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Adliance.AspNetCore.Buddy.Email.Bulma;

public static class SelectList
{
    public static IList<SelectListItem> For(Enum enumValue)
    {
        return (from Enum e in Enum.GetValues(enumValue.GetType()) select new SelectListItem(e.GetDisplayName(), e.ToString())).ToList();
    }

    public static IList<SelectListItem> For(string trueText = "Ja", string falseText = "Nein")
    {
        return new List<SelectListItem>
        {
            new(trueText, "true"),
            new(falseText, "false")
        };
    }

    public static IList<SelectListItem> For(IDictionary<Guid, string> dict)
    {
        return dict.Select(x => new SelectListItem(x.Value, x.Key.ToString())).ToList();
    }

    private static string GetDisplayName(this Enum enumValue)
    {
        var attribute = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>();

        if (attribute == null)
        {
            return enumValue.ToString();
        }

        return attribute.GetName() ?? enumValue.ToString();
    }
}