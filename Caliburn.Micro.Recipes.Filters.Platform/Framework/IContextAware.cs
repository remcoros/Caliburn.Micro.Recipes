namespace Caliburn.Micro.Recipes.Filters.Framework
{
    using System;

    public interface IContextAware : IFilter, IDisposable
    {
        void MakeAwareOf(ActionExecutionContext context);
    }
}