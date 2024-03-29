﻿using System.Text.RegularExpressions;

namespace Microservices.Banking.Host.Routing;

public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        if (value is null)
        {
            return null;
        }

        // Slugify value
        return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}