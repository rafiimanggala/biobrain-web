using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.Dialogs
{
    public class BaseDialog<T> : ContentView
	{
	    public bool Closable = false;

	    public event EventHandler<T> Result;

	    protected StackLayout DialogContent;

	    public BaseDialog()
	    {
		    HorizontalOptions = LayoutOptions.FillAndExpand;
		    VerticalOptions = LayoutOptions.FillAndExpand;
		    IsVisible = false;

		    DialogContent = new StackLayout
		    {
			    HorizontalOptions = LayoutOptions.FillAndExpand,
			    VerticalOptions = LayoutOptions.CenterAndExpand,
			    Orientation = StackOrientation.Vertical,
			    Padding = 15,
			    BackgroundColor = Colors.Transparent,
		    };
		    var frame = new Frame
		    {
			    HorizontalOptions = LayoutOptions.FillAndExpand,
			    VerticalOptions = LayoutOptions.CenterAndExpand,
			    BorderColor = Colors.Transparent,
			    CornerRadius = 5,
			    BackgroundColor = Colors.White,
			    Content = DialogContent,
			    Margin = 15,
			    Padding = 0
		    };
		    Content = frame;
		    GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { if (Closable) Close(); }) });
	    }

	    public virtual void Submit(T result)
	    {
		    OnResult(result);
		    Close();
	    }

	    public virtual void Close()
	    {
		    IsVisible = false;
	    }

	    public virtual void Show()
	    {
		    IsVisible = true;
	    }

	    protected virtual void OnResult(T e)
	    {
		    Result?.Invoke(this, e);
	    }
    }
}
