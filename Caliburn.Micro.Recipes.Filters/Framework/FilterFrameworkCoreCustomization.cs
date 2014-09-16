namespace Caliburn.Micro.Recipes.Filters.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class FilterFrameworkCoreCustomization
    {
        #region Public Methods and Operators

        public static void Hook()
        {
            var oldPrepareContext = ActionMessage.PrepareContext;
            ActionMessage.PrepareContext = context =>
                {
                    oldPrepareContext(context);
                    PrepareContext(context);
                };

            ActionMessage.InvokeAction = context =>
                {
                    var values = MessageBinder.DetermineParameters(context, context.Method.GetParameters());
                    InvokeAction(context, values);
                };
        }

        #endregion

        #region Methods

        internal static void InvokeAction(ActionExecutionContext context, object[] values)
        {
            var returnValue = context.Method.Invoke(context.Target, values);

            var task = returnValue as System.Threading.Tasks.Task;
            if (task != null)
            {
                returnValue = task.AsResult();
            }

            var result = returnValue as IResult;
            if (result != null)
            {
                returnValue = new[] { result };
            }

            var enumerable = returnValue as IEnumerable<IResult>;
            if (enumerable != null)
            {
                returnValue = enumerable.GetEnumerator();
            }

            var enumerator = returnValue as IEnumerator<IResult>;
            if (enumerator != null)
            {
                result = Coroutine.CreateParentEnumerator(enumerator);
                var wrappers = FilterManager.GetFiltersFor(context).OfType<IExecutionWrapper>();
                var pipeline = result.WrapWith(wrappers);
                //if pipeline has error, action execution should throw! 
                pipeline.Completed += (o, e) => Execute.OnUIThread(
                    () =>
                    {
                        if (e.Error != null)
                        {
                            throw new Exception(
                                string.Format("An error occurred while executing {0}", context.Message),
                                e.Error
                                );
                        }
                    });
                pipeline.Execute(
                    new CoroutineExecutionContext
                        {
                            Source = context.Source,
                            View = context.View,
                            Target = context.Target
                        });
            }
        }

        internal static void PrepareContext(ActionExecutionContext context)
        {
            var contextAwareFilters = FilterManager.GetFiltersFor(context).OfType<IContextAware>().ToArray();
            contextAwareFilters.Apply(x => x.MakeAwareOf(context));

            context.Message.Detaching += (o, e) => contextAwareFilters.Apply(x => x.Dispose());
        }

        #endregion
    }
}