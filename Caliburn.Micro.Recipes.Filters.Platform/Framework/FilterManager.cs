namespace Caliburn.Micro.Recipes.Filters.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    public static class FilterManager
    {
        private static readonly Dictionary<string, IEnumerable<IFilter>> filtersCache = new Dictionary<string, IEnumerable<IFilter>>();

        public static IResult WrapWith(this IResult inner, IEnumerable<IExecutionWrapper> wrappers)
        {
            IResult previous = inner;
            foreach (var wrapper in wrappers)
            {
                previous = wrapper.Wrap(previous);
            }
            return previous;
        }


        public static Func<ActionExecutionContext, IEnumerable<IFilter>> GetFiltersFor = (context) =>
        {
            if (context.Target == null)
            {
                return new IFilter[0];
            }

            var type = context.Target.GetType();
            var cacheKey = type.FullName;
            if (!filtersCache.ContainsKey(cacheKey))
            {
                filtersCache[cacheKey] = type.GetAttributes<IFilter>(true);
            }

            var filters = filtersCache[cacheKey];

            if (context.Method != null)
            {
                cacheKey += context.Method.Name;
                if (!filtersCache.ContainsKey(cacheKey))
                {
                    filtersCache[cacheKey] = context.Method.GetAttributes<IFilter>(true);
                }

                filters = filters.Union(filtersCache[cacheKey]);
            }

            filters = filters.OrderBy(x => x.Priority);

            return filters;
        };

        public static void ExecuteAction(Expression<System.Action> action)
        {
            ExecuteActionImpl(action);
        }
        public static void ExecuteAction(Expression<Func<IEnumerable<IResult>>> coroutine)
        {
            ExecuteActionImpl(coroutine);
        }
        public static void ExecuteAction(Expression<Func<IEnumerator<IResult>>> coroutine)
        {
            ExecuteActionImpl(coroutine);
        }

        static void ExecuteActionImpl(LambdaExpression lambda)
        {
            var call = lambda.Body as MethodCallExpression;
            if (call == null) throw new ArgumentException("Execute action only supports lambda in the form FilterManager.ExecuteAction(() => vm.MyAction()), being MyAction void, IEnumerable<IResult> or IEnumerator<IResult>");

            var targetExp = call.Object as ConstantExpression;
            if (targetExp == null) throw new ArgumentException("Execute action only supports lambda in the form FilterManager.ExecuteAction(() => vm.MyAction()), being 'vm' an object instance");

            var context = new ActionExecutionContext
            {
                Method = call.Method,
                Target = targetExp.Value
            };

            FilterFrameworkCoreCustomization.InvokeAction(context, new object[] { });
        }


    }
}
