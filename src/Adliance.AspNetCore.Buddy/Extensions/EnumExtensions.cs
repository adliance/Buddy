using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Adliance.AspNetCore.Buddy.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// A generic extension method that aids in reflecting and retrieving any attribute that is applied to an `Enum`.
    /// </summary>
    public static string GetDisplayName(this Enum enumValue)
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