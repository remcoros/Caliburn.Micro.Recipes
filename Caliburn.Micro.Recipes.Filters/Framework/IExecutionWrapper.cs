namespace Caliburn.Micro.Recipes.Filters.Framework
{
    public interface IExecutionWrapper : IFilter
    {
        IResult Wrap(IResult inner);
    }
}