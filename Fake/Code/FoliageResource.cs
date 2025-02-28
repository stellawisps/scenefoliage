using System;

namespace Foliage;
[Serializable]
[GameResource( "Foliage/Foliage", "fol", "Foliage Definition", Icon = "grass" )]
public class FoliageResource : GameResource
{
	public static HashSet<FoliageResource> All { get; set; } = new();
	[Category("info")]
	public string Name { get; set; }
	
	[ResourceType( "vmdl" ),Category("info")]
	public Model Model { get; set; }
	
	
	public float Scale { get; set; } = 1.0f;
	public float ScaleRandom { get; set; } = 0f;
	[Category("placement")]
	public bool AlignToNormal { get; set; } = true;
	[Category("placement")]
	public bool RandomRotation { get; set; } = true;
	[Category("placement")]
	public float ZOffset { get; set; } = 0f;
	[Category("placement")]
	public float ZOffsetRandom { get; set; } = 0f;
	protected override void PostLoad()
	{
		All.Add( this );
	}
}
