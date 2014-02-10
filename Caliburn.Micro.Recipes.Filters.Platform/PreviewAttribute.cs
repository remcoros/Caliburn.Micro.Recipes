namespace Caliburn.Micro.Recipes.Filters
{
    using System;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
	/// Allows to specify a guard method or property with an arbitrary name
	/// </summary>
	public class PreviewAttribute : Attribute, IContextAware
	{

		public PreviewAttribute(string methodName)
		{
			this.MethodName = methodName;
		}

		public string MethodName { get; private set; }
		public int Priority { get; set; }


		public void MakeAwareOf(ActionExecutionContext context)
		{
			var targetType = context.Target.GetType();
			var guard = targetType.GetMethod(this.MethodName);
			if (guard == null)
				guard = targetType.GetMethod("get_" + this.MethodName);

			if (guard == null) return;

			var oldCanExecute = context.CanExecute ?? new Func<bool>(() => true);
			context.CanExecute = () =>
			{
				if (!oldCanExecute()) return false;

				return (bool)guard.Invoke(
					context.Target,
					MessageBinder.DetermineParameters(context, guard.GetParameters())
				);
			};
		}

		public void Dispose() { }

	}
	//usage:
	//[Preview("IsMyActionAvailable")]
	//public void MyAction(int value) { ... }
	//public bool IsMyActionAvailable(int value) { ... }
}
