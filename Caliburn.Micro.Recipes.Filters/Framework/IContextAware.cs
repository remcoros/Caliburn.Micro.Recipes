namespace Caliburn.Micro.Recipes.Filters.Framework
{
    using System;

    public interface IContextAware : IFilter, IDisposable
    {
        #region Public Methods and Operators

        void MakeAwareOf(ActionExecutionContext context);

        #endregion
    }
}