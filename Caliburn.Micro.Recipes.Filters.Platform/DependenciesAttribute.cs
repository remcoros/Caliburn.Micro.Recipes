namespace Caliburn.Micro.Recipes.Filters
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
	/// Updates the availability of the action (thus updating the UI)
	/// </summary>
	public class DependenciesAttribute : Attribute, IContextAware
	{

		ActionExecutionContext _context;
		INotifyPropertyChanged _inpc;

		public DependenciesAttribute(params string[] propertyNames)
		{
			this.PropertyNames = propertyNames ?? new string[] { };
		}

		public string[] PropertyNames { get; private set; }
		public int Priority { get; set; }



		public void MakeAwareOf(ActionExecutionContext context)
		{
			this._context = context;
			this._inpc = context.Target as INotifyPropertyChanged;
			if (this._inpc != null)
				this._inpc.PropertyChanged += this.inpc_PropertyChanged;
		}





		public void Dispose()
		{
			if (this._inpc != null)
				this._inpc.PropertyChanged -= this.inpc_PropertyChanged;
			this._inpc = null;
		}



		void inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.PropertyNames.Contains(e.PropertyName))
			{
				Execute.OnUIThread(() =>
				{
					this._context.Message.UpdateAvailability();
				});
			}
		}
		
	}

	//usage:
	//[Dependencies("MyProperty", "MyOtherProperty")]
	//public void DoAction() { ... }
	//public bool CanDoAction() { return MyProperty > 0 && MyOtherProperty < 1; }
}
