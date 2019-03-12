using System;
using System.Collections.Generic;
using Infrastructure.SearchModel.Model;

namespace Infrastructure.SearchModel.TransformProviders
{
    public interface ITransformProvider
    {
        bool Match(ConditionItem item, Type type);
        IEnumerable<ConditionItem> Transform(ConditionItem item, Type type);
    }
}
