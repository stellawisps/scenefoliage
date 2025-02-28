using Foliage;

namespace Editor.FoliagePainter;

	using Editor;
using Sandbox;
	using System;
using System.IO;
using System.Linq;


[Sandbox.CustomEditor( typeof( FoliageRenderer.OptionsWidget ) )]
public class FoliageRendererButtons : ControlWidget
{
	FoliageRenderer FoliageManager { get; set; }
	
	public FoliageRendererButtons( Sandbox.SerializedProperty property ) : base( property )
    {
        FoliageManager = property.Parent.Targets.First() as FoliageRenderer;

        Layout = Layout.Column();
        Layout.Spacing = 2;

        Rebuild();
    }

    [EditorEvent.Hotload]
    void Rebuild()
    {
        Layout.Clear( true );

        {
            var button = new Button( this );
            button.Text = "Reset Foliage";
            button.Clicked += ResetResource;
            Layout.Add( button );
        }

    }

    void ResetResource()
    {
	    FoliageManager.ClearAll();
        Rebuild();
    }
    
}

