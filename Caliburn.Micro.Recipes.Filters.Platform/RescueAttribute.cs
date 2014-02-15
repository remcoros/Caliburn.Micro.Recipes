namespace Caliburn.Micro.Recipes.Filters
{
    using System;

    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
    /// Allows to specify a "rescue" method to handle exception occurred during execution
    /// </summary>
    public class RescueAttribute : ExecutionWrapperBase
    {
        #region Constructors and Destructors

        public RescueAttribute()
            : this("Rescue")
        {
        }

        public RescueAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        #endregion

        #region Public Properties

        public string MethodName { get; private set; }

        #endregion

        #region Methods

        protected override bool HandleException(CoroutineExecutionContext context, Exception ex)
        {
            var method = context.Target.GetType().GetMethod(this.MethodName, new[] { typeof(Exception) });
            if (method == null)
            {
                return false;
            }

            try
            {
                var result = method.Invoke(context.Target, new object[] { ex });
                if (result is bool)
                {
                    return (bool)result;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        //usage:
        //[Rescue]
        //public void ThrowingAction()
        //{
        //    throw new NotImplementedException();
        //}
        //public bool Rescue(Exception ex)
        //{
        //    MessageBox.Show(ex.ToString());
        //    return true;
        //}
    }
}