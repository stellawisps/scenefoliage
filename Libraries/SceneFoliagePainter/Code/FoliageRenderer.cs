using System;
using System.Text.Json.Serialization;
using Sandbox.Engine.Settings;

namespace Foliage;


public class FoliageRenderer : Component, Component.ExecuteInEditor
{
	public List<FoliageSceneObject> Renderers { get; set; } = new();
	[Property,Hide] Dictionary<int,List<Transform>> FoliageRenderers { get; set; } = new();
	[Property] private int ChunkSize { get; set; } = 512;
	[Property] OptionsWidget Options { get; set; }
	public void PaintFoliage(FoliageResource foliage, Transform transform)
	{
		if ( FoliageRenderers.TryGetValue(foliage.ResourceId, out var renderer) )
		{
			renderer.Add( transform );
			UpdateRenderers();
		}
		else
		{
			FoliageRenderers.Add( foliage.ResourceId, new List<Transform> { transform } );
			FoliageRenderers[foliage.ResourceId].Add( transform );
			UpdateRenderers();
		}
	}

	public void EraseFoliage( Vector3 position,float size, int id )
	{
		List<Transform> toRemove = new();

		if ( id != 0 )
		{
			foreach ( var testPos in FoliageRenderers[id] )
			{
				if ( testPos.Position.DistanceSquared( position ) <= size * size )
				{
					toRemove.Add( testPos );
				}
			}
			
			foreach ( var remove in toRemove )
			{
				FoliageRenderers[id].Remove( remove );
			}
			return;
		}
		
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
		UpdateRenderers();
		
		
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		UpdateRenderers();
	}

	protected override void OnDestroy()
	{
		foreach ( var val in Renderers )
		{
			val.Delete();
		}
		base.OnDestroy();
	}

	protected override void OnDisabled()
	{
		foreach ( var val in Renderers )
		{
			val.Delete();
		}
		base.OnDisabled();
		
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
			var transformMins = new Vector3( 0 );
			var transformMaxs = new Vector3( 0 );

			foreach ( var testTrans in folRenderer.Value )
			{
				transformMins = Vector3.Min( transformMins, testTrans.Position );
				transformMaxs = Vector3.Max( transformMaxs, testTrans.Position );
			}
			
			var transformSize = transformMaxs - transformMins;

			var transformArray = folRenderer.Value.ToArray();
			var folInstance = ResourceLibrary.Get<FoliageResource>( folRenderer.Key );

			
			for ( int x = 0; x < (transformSize.x / ChunkSize).CeilToInt(); x++ )
			{
				for ( int y = 0; y < (transformSize.y / ChunkSize).CeilToInt(); y++ )
				{
					List<Transform> newTransforms = new();
					var transformedMins = transformMins + new Vector3( (x * ChunkSize)+ChunkSize, (y * ChunkSize)+ChunkSize, 10000 );
					var testBBox = new BBox( transformMins+ new Vector3( x * ChunkSize, y * ChunkSize, -500 ),transformedMins );
					foreach ( var testTrans in folRenderer.Value )
					{
						if ( testBBox.Contains( testTrans.Position ) )
						{
							newTransforms.Add( testTrans );
						}
					}

					if ( newTransforms.Count <= 0 )
					{
						continue;
					}

					var renderModelLargestAxis = folInstance.Model.Bounds.Size;

					var boundsMin = Vector3.Zero;
					var boundsMax = Vector3.Zero;
					for ( int i = 0; i < newTransforms.Count; i++ )
					{
						boundsMin = Vector3.Min( boundsMin, newTransforms[i].Position);
						boundsMax = Vector3.Max( boundsMax, newTransforms[i].Position);
					}
						
					

					
					var folSceneObject = new FoliageSceneObject( GameObject.Scene.SceneWorld, folInstance.Model )
					{
						Bounds = new BBox( boundsMin-renderModelLargestAxis,boundsMax+renderModelLargestAxis ),
						Transforms = newTransforms.ToArray()
					};
					folSceneObject.Flags.CastShadows = true;
			
					Renderers.Add( folSceneObject );
				}
			}

			

			




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
	public Model RenderModel;
	public Transform[] Transforms = Array.Empty<Transform>();
	public FoliageSceneObject(SceneWorld sceneWorld, Model renderModel) : base(sceneWorld)
	{
		RenderModel = renderModel;
	}

	
	public void AddTransform(Transform transform)
	{
		//Transforms.Add( transform );
	}

	public override void RenderSceneObject()
	{
		base.RenderSceneObject();
		Graphics.DrawModelInstanced( RenderModel, Transforms.AsSpan() );
		
		
	}
}
