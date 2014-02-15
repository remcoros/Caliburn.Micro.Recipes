namespace Caliburn.Micro.Recipes.Filters.Framework
{
    public interface IExecutionWrapper : IFilter
    {
        #region Public Methods and Operators

        IResult Wrap(IResult inner);

        #endregion
    }
}