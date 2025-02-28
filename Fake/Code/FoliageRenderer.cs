﻿using System;
using Sandbox.Engine.Settings;

namespace Foliage;


public class FoliageRenderer : Component, Component.ExecuteInEditor
{
	public List<FoliageSceneObject> Renderers { get; set; } = new();
	[Property] Dictionary<int,List<Transform>> FoliageRenderers { get; set; } = new();
	[Property] OptionsWidget Options { get; set; }
	public void PaintFoliage(FoliageResource foliage, Transform transform)
	{
		if ( FoliageRenderers.ContainsKey( foliage.ResourceId ) )
		{
			FoliageRenderers[foliage.ResourceId].Add( transform );
		}
		else
		{
			FoliageRenderers.Add( foliage.ResourceId, new List<Transform> { transform } );
			FoliageRenderers[foliage.ResourceId].Add( transform );
			UpdateRenderers();
		}
	}

	public void EraseFoliage( Vector3 position,float size )
	{
		List<Transform> toRemove = new();
		
		foreach ( var testVal in FoliageRenderers )
		{
			foreach ( var testPos in testVal.Value )
			{
				if ( testPos.Position.DistanceSquared( position ) <= size * size )
				{
					toRemove.Add( testPos );
				}
			}

			foreach ( var remove in toRemove )
			{
				testVal.Value.Remove( remove );
			}
		}
		
		
	}

	protected override void OnStart()
	{
		foreach ( var val in Scene.SceneWorld.SceneObjects )
		{
			if ( val.GetType() == typeof(FoliageSceneObject) )
			{
				val.Delete();
			}
		}
		
		UpdateRenderers();
		
		
	}

	protected override void OnUpdate()
	{
		
		
	}

	
	public void UpdateRenderers()
	{
		foreach ( var val in Renderers )
		{
			val.Delete();
			
		}
		Renderers.Clear();
		foreach ( var folRenderer in FoliageRenderers )
		{
			var folInstance = ResourceLibrary.Get<FoliageResource>( folRenderer.Key );
			
			
			var folSceneObject = new FoliageSceneObject( GameObject.Scene.SceneWorld, folInstance.Model )
			{
				Transforms = folRenderer.Value
			};

			
			
			Renderers.Add( folSceneObject );




		}
		
	}
	public void ClearAll()
	{
		foreach ( var val in FoliageRenderers )
		{
			val.Value.Clear();
		}
		UpdateRenderers();
	}
	
	public class OptionsWidget { }
}
public class FoliageSceneObject : SceneCustomObject
{
	private Model _renderModel;
	public List<Transform> Transforms = new();
	public FoliageSceneObject(SceneWorld sceneWorld, Model renderModel) : base(sceneWorld)
	{
		_renderModel = renderModel;
	}

	
	public void AddTransform(Transform transform)
	{
		Transforms.Add( transform );
	}

	public override void RenderSceneObject()
	{
		base.RenderSceneObject();
		Graphics.DrawModelInstanced( _renderModel, Transforms.ToArray().AsSpan() );
		
		
	}
}
