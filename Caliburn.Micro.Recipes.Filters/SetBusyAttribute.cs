namespace Caliburn.Micro.Recipes.Filters
{
    using System;

    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
    /// Sets "IsBusy" property to true (on models implementing ICanBeBusy) during the execution
    /// </summary>
    public class SetBusyAttribute : ExecutionWrapperBase
    {
        #region Methods

        protected override void AfterExecute(CoroutineExecutionContext context)
        {
            this.SetBusy(context.Target as ICanBeBusy, false);
        }

        protected override void BeforeExecute(CoroutineExecutionContext context)
        {
            this.SetBusy(context.Target as ICanBeBusy, true);
        }

        protected override bool HandleException(CoroutineExecutionContext context, Exception ex)
        {
            this.SetBusy(context.Target as ICanBeBusy, false);
            return false;
        }

        private void SetBusy(ICanBeBusy model, bool isBusy)
        {
            if (model != null)
            {
                model.IsBusy = isBusy;
            }
        }

        #endregion
    }

    //usage:
    //[SetBusy]
    //[Async] //prevents UI freezing, thus allowing busy state representation
    //public void VeryLongAction() { ... }
}