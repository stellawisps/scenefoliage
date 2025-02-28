using System;

namespace Editor.FoliagePainter;
using Foliage;
[EditorTool] // this class is an editor tool
[Title( "Foliage" )] // title of your tool
[Icon( "grass" )] // icon name from https://fonts.google.com/icons?selected=Material+Icons
public class FoliagePainter : EditorTool
{
	private float PaintProgress { get; set; } = 0f;
	public FoliageSettings FoliageSettings { get; private set; } = new();
	public override void OnEnabled()
	{
		AllowGameObjectSelection = false;
		if ( GetSelectedComponent<FoliageRenderer>() == null )
		{
			Selection.Clear();
			var first = Scene.GetAllComponents<FoliageRenderer>().FirstOrDefault();
			if ( first is not null ) Selection.Add( first.GameObject );
		}
		
		var foliageSettings = new FoliageSettingsWidgetWindow( SceneOverlay, EditorUtility.GetSerializedObject( FoliageSettings ) );
		AddOverlay( foliageSettings, TextFlag.RightBottom, 10 );
	}

	private void AddUi()
	{
		// create a widget window. This is a window that  
		// can be dragged around in the scene view
		var window = new WidgetWindow( SceneOverlay );
		window.Layout = Layout.Column();
		window.Layout.Margin = 16;
 
		// Create a button for us to press
		var button = new Button( "Shoot Rocket" );
		button.Pressed = () => Log.Info( "Rocket Has Been Shot!" );

		// Add the button to the window's layout
		window.Layout.Add( button );

		// Calling this function means that when your tool is deleted,
		// ui will get properly deleted too. If you don't call this and
		// you don't delete your UI in OnDisabled, it'll hang around forever.
		AddOverlay( window, TextFlag.RightTop, 10 );
	}
	public override void OnDisabled()
	{
		_previewObject?.Delete();
		_previewObject = null;
	}

	private bool Painted { get; set; } = false;
	
	private FoliageBrushPreview _previewObject;

	public override void OnUpdate()
	{
		var tr = Scene.Trace.Ray( Gizmo.CurrentRay, 5000 )
			.UseRenderMeshes( true )
			.UsePhysicsWorld( false )
			.WithoutTags( "invisible" )
			.Run();


		if ( tr.Hit )
		{
			using ( Gizmo.Scope( "cursor" ) )
			{
				Gizmo.Transform = new Transform( tr.HitPosition, Rotation.LookAt( tr.Normal ) );
				Gizmo.Draw.LineCircle( 0, FoliageSettings.Size );

			}

			
			
			if ( Gizmo.IsLeftMouseDown && Application.KeyboardModifiers.HasFlag( Sandbox.KeyboardModifiers.Shift ) )
			{
				var paintTarget = GetSelectedComponent<FoliageRenderer>();
				if ( paintTarget == null ) { return; }
				
				paintTarget.EraseFoliage( tr.HitPosition, FoliageSettings.Size );
				Painted = true;
			}

			if ( !Gizmo.IsLeftMouseDown && Painted )
			{
				Painted = false;
				SceneEditorSession.Active.FullUndoSnapshot("Paint Grass");
				
				var paintTarget = GetSelectedComponent<FoliageRenderer>();
				if ( paintTarget == null ) { return; }
				//paintTarget.UpdateRenderers();
			}
			
			if ( Gizmo.IsLeftMouseDown && !Application.KeyboardModifiers.HasFlag( Sandbox.KeyboardModifiers.Shift ))
			{
				if ( FoliageSettings.Foliage == null ) { return;}
				PaintProgress += Time.Delta * FoliageSettings.PaintSpeed * 3f;
				if ( PaintProgress >= 1 )
				{
					PaintProgress = 0;

					var paintTarget = GetSelectedComponent<FoliageRenderer>();
					if ( paintTarget == null ) { return; }
					
					var randomPos =
						new Vector3( Random.Shared.Float( FoliageSettings.Size * -1, FoliageSettings.Size ),
							Random.Shared.Float( FoliageSettings.Size * -1, FoliageSettings.Size ), FoliageSettings.Size ).RotateAround( Vector3.Zero,
							Rotation.LookAt( tr.Normal ) * Rotation.FromPitch( 90f ) );
					
					var paintTr = Scene.Trace.Ray( new Ray( tr.HitPosition + randomPos,Rotation.LookAt( tr.Normal ) * Rotation.FromPitch( 90f ).Down ), FoliageSettings.Size+2 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithoutTags( "invisible" )
						.Run();

					if ( paintTr.Hit )
					{
						var randomRot = Random.Shared.Float(0,360);
						
						var transform = new Transform( paintTr.HitPosition+(tr.Normal* (FoliageSettings.Foliage.ZOffset+Random.Shared.Float(FoliageSettings.Foliage.ZOffsetRandom*-1,FoliageSettings.Foliage.ZOffsetRandom))) );
						transform = transform.WithRotation( Rotation.LookAt( paintTr.Normal )*Rotation.FromPitch( 90f )*Rotation.FromYaw( FoliageSettings.Foliage.RandomRotation? randomRot : 0) );
						transform = transform.WithScale(
							FoliageSettings.Foliage.Scale + Random.Shared.Float( FoliageSettings.Foliage.ScaleRandom * -1 ,
								FoliageSettings.Foliage.ScaleRandom ));
					
						paintTarget.PaintFoliage( FoliageSettings.Foliage ,transform );
						Painted = true;
					}

				}
			}
			
		}
	}
}
