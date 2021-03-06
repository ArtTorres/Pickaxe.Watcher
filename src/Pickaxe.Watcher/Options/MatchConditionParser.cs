﻿using MagnetArgs;
using Pickaxe.Watcher.IO.Twitter.Stream;

namespace Pickaxe.Watcher.Options
{
    class MatchConditionParser : IParser
    {
        public object Parse(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "all":
                    return MatchCondition.AllConditions as object;
                default:
                    return MatchCondition.AnyCondition as object;
            }
        }
    }
}
