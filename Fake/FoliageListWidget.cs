using System;
using Foliage;

namespace Editor.FoliagePainter;

class FoliageListWidget : Widget
{
	public Action BrushSelected { get; set; }

	public FoliageListWidget() : base( null )
	{
		ListView list = new();
		// list.MaximumHeight = 96;
		//list.SetItems( FoliagePainter.FoliageList.AllFoliage );
		list.ItemSize = new Vector2( 40, 40 );
		list.ItemAlign = Sandbox.UI.Align.SpaceBetween;
		list.OnPaintOverride += () => PaintListBackground( list );
		list.ItemPaint = PaintBrushItem;
		list.ItemSelected = ( item ) => { if ( item is FoliageResource brush ) { SelectBrush( brush ); } };
		//list.SelectItem( FoliagePainter.FoliageList.Selected );

		Layout = Layout.Column();

		var label = new Label( "Brushes" );
		label.SetStyles( "font-weight: bold" );
		Layout.Add( label );
		Layout.AddSpacingCell( 8 );
		Layout.Add( list );
	}

	private void SelectBrush( FoliageResource selFoliage )
	{
		//FoliagePainter.FoliageList.Selected = selFoliage;
		BrushSelected?.Invoke();
	}

	private void PaintBrushItem( VirtualWidget widget )
	{
		var brush = (FoliageResource)widget.Object;

		Paint.Antialiasing = true;
		Paint.TextAntialiasing = true;

		if ( widget.Hovered || widget.Selected )
		{
			Paint.ClearPen();
			Paint.SetBrush( widget.Selected ? Theme.Primary : Color.White.WithAlpha( 0.1f ) );
			Paint.DrawRect( widget.Rect.Grow( 2 ), 3 );
		}
		
		Paint.SetBrush(Color.White.WithAlpha(1f ) );
		Paint.DrawText( widget.Rect.Grow(3  ),brush.Name.ToString() );
	}

	private bool PaintListBackground( Widget widget )
	{
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( widget.LocalRect );

		return false;
	}
}
