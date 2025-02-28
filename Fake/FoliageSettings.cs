using System;
using Foliage;

namespace Editor.FoliagePainter;

public class FoliageSettings
{
	[Property, Range( 8, 512 )] public int Size { get; set; } = 50;
	[Property, Range( 0, 100 )] public float PaintSpeed { get; set; } = 1;
	[Property, ResourceType(".fol")] public FoliageResource Foliage { get; set; }
}

public class FoliageSettingsWidgetWindow : WidgetWindow
{
	class FoliageSelectedWidget : Widget
	{
		public FoliageSelectedWidget( Widget parent ) : base( parent )
		{
			MinimumSize = new( 48, 48 );
			Cursor = CursorShape.Finger;
		}

		protected override void OnMouseClick( MouseEvent e )
		{
			var popup = new PopupWidget( null );
			popup.Position = Application.CursorPosition;
			popup.Visible = true;
			popup.Layout = Layout.Column();
			popup.Layout.Margin = 10;
			popup.MaximumSize = new Vector2( 300, 150 );

			var list = new FoliageListWidget();
			list.BrushSelected += () => { popup.Close(); Update(); };
			popup.Layout.Add( list );
		}
	}

	public FoliageSettingsWidgetWindow( Widget parent, SerializedObject so ) : base( parent, "Foliage Settings" )
	{
		Layout = Layout.Row();
		Layout.Margin = 8;
		MaximumWidth = 300.0f;

		var cs = new ControlSheet();
		cs.AddRow( so.GetProperty( nameof( FoliageSettings.Size ) ) );
		cs.AddRow( so.GetProperty( nameof( FoliageSettings.PaintSpeed) ) );
		cs.AddRow( so.GetProperty( nameof( FoliageSettings.Foliage ) ) );

		cs.SetMinimumColumnWidth( 0, 50 );
		cs.Margin = new Sandbox.UI.Margin( 8, 0, 4, 0 );

		var l = Layout.Row();
		l.Add( cs );

		Layout.Add( l );
	}
}
