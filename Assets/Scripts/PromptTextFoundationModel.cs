using AiToolbox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AiToolboxRuntimeSample
{
	public class PromptTextFoundationModel : MonoBehaviour
	{
		public Parameters parameters;
		public string prompt = "Generate a character description";

		[SerializeField]
		MapCreator mapCreator;

		public TMP_InputField inputField;
		public TMP_Text chatDisplay;

		bool isNewChat = true;

		string basePrompt = @"
            1.) In C# for a Unity runtime application, create a Dictionary of format Dictionary<Point, Tile> tilesDict();
	
			2.) Only generate the dictionary. No preamble response or code. Only the dictionary.

		    3.) The Point class is defined as:

public struct Point : IEquatable<Point>
{
	#region Fields
	public int x;
	public int y;
	#endregion

	#region Constructors
	public Point (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	#endregion

	#region Operator Overloads
	public static Point operator +(Point a, Point b)
	{
		return new Point(a.x + b.x, a.y + b.y);
	}
	
	public static Point operator -(Point p1, Point p2) 
	{
		return new Point(p1.x - p2.x, p1.y - p2.y);
	}
	
	public static bool operator ==(Point a, Point b)
	{
		return a.x == b.x && a.y == b.y;
	}
	
	public static bool operator !=(Point a, Point b)
	{
		return !(a == b);
	}

	public static implicit operator Vector2(Point p)
	{
		return new Vector2(p.x, p.y);
	}
	#endregion

	#region Object Overloads
	public override bool Equals (object obj)
	{
		if (obj is Point)
		{
			Point p = (Point)obj;
			return x == p.x && y == p.y;
		}
		return false;
	}
	
	public bool Equals (Point p)
	{
		return x == p.x && y == p.y;
	}
	
	public override int GetHashCode ()
	{
		return x ^ y;
	}

	public override string ToString ()
	{
		return string.Format (""({0},{1})"", x, y);
	}
	#endregion
}

	        4.) The Tile class is defined as:

public class Tile : MonoBehaviour
{
	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Material material;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture team2Highlight;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture team3Highlight;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture tileHighlight;

	/// <summary>
	/// In WA mode can exit map and go to a different map by stepping on certain tiles.
	/// Material for indicating a tile is able to exit the map.
	/// </summary>
	[SerializeField]
	Material tileExitMapMaterial;

	#region Const
	/// <summary>
	/// Used for scaling the height of a tile
	/// </summary>
	public const float stepHeight = 0.25f;

	/// <summary>
	/// used for centering the indicator for the tile
	/// used for centering the height of units that walk around the map
	/// </summary>
	public const float centerHeight = 0.45f;
	#endregion

	#region Fields / Properties
	/// <summary>
	/// Contains the x and y coordinates of a tile
	/// </summary>
	public Point pos;

	/// <summary>
	/// Has the z coordinates (height) of the tile
	/// </summary>
	public int height;

	public Vector3 center { get { return new Vector3(pos.x, height * stepHeight + centerHeight, pos.y); } }
	public GameObject content;

	/// <summary>
	/// When calculating valid movements in cref, use the prev field
	/// </summary>
	[HideInInspector] public Tile prev;

	/// <summary>
	/// WHen calculating valid movements in cref, use the distance to track how many
	/// additional tiles the unit can move to
	/// </summary>
	[HideInInspector] public int distance;

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	public Dictionary<int, Tile> prevDict = new Dictionary<int, Tile>();

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	public Dictionary<int, int> distanceDict = new Dictionary<int, int>();

	private int unitId = NameAll.NULL_UNIT_ID;

	/// <summary>
	/// added to easier check tiles for PlayerUnits
	/// </summary>
	public int UnitId
	{
		get { return unitId; }
		set { unitId = value; }
	}

	private int pickUpId = 0; //0 for nothing, 1 for crystals

	/// <summary>
	/// Can sometimes be objects on tiles
	/// for now, 0 for nothing, 1 for crystals
	/// to do: add enums for this type
	/// </summary>
	public int PickUpId
	{
		get { return pickUpId; }
		set { pickUpId = value; }
	}

	private int tileType = NameAll.TILE_TYPE_DEFAULT;
	/// <summary>
	/// for WA mode Tile Type can be exit map as well
	/// </summary>
	public int TileType
	{
		get { return tileType; }
		set { tileType = value; }
	}
	#endregion

	#region Public

	/// <summary>
	/// increase the height of a tile
	/// </summary>
	public void Grow()
	{
		height++;
		Match();
	}

	/// <summary>
	/// reduce the height of a tile
	/// </summary>
	public void Shrink()
	{
		height--;
		Match();
	}

	/// create a tile from a tile object
	public void Load(Tile t)
	{
		this.pos = t.pos;
		this.height = t.height;
		Match();
		//this.content = t.content;
		//this.prev = t.prev;
		//this.distance = t.distance;
		//this.prevDict = t.prevDict;
		//this.distanceDict = t.distanceDict;
		//this.unitId = t.unitId;
		//this.pickUpId = t.pickUpId;
		//this.tileType = t.tileType;
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Point p, int h)
	{
		pos = p;
		height = h;
		Match();
	}

	/// <summary>
	///
	/// </summary>
	public void Load(Vector3 v)
	{
		Load(new Point((int)v.x, (int)v.z), (int)v.y);
		this.UnitId = NameAll.NULL_UNIT_ID;
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Vector3 v, int renderMode)
	{
		Load(new Point((int)v.x, (int)v.z), (int)v.y, renderMode);
		this.UnitId = NameAll.NULL_UNIT_ID;
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Point p, int h, int renderMode)
	{
		pos = p;
		height = h;
		if (renderMode != NameAll.PP_RENDER_NONE)
			Match();

	}

	/// <summary>
	/// change tile look for MapBuilder Scene
	/// </summary>
	public void RevertTile()
	{
		var rend = this.gameObject.GetComponent<Renderer>();
		if (this.TileType == NameAll.TILE_TYPE_EXIT_MAP)
		{
			rend.material = tileExitMapMaterial;
		}
		else
		{
			rend.material = material;
		}
	}

	/// <summary>
	/// Change texture of tile to highlight it for a specific team or look
	/// </summary>
	public void HighlightTile(int teamId)
	{
		if (teamId == 2)
		{
			var rend = this.gameObject.GetComponent<Renderer>();
			//rend.material.mainTexture = Resources.Load(""square frame 1"") as Texture;
			rend.material.mainTexture = team2Highlight;
		}
		else if (teamId == 3)
		{
			var rend = this.gameObject.GetComponent<Renderer>();
			//rend.material.mainTexture = Resources.Load(""square frame 2"") as Texture;
			rend.material.mainTexture = team3Highlight;
		}
		else
		{
			var rend = this.gameObject.GetComponent<Renderer>();
			//rend.material.mainTexture = Resources.Load(""square frame 3"") as Texture;
			rend.material.mainTexture = tileHighlight;
		}
	}
	#endregion

	#region Private
	/// <summary>
	/// When a tile is loaded or height changed, call this to line the tile up correctly
	/// </summary>
	void Match()
	{
		transform.localPosition = new Vector3(pos.x, height * stepHeight / 2f, pos.y);
		transform.localScale = new Vector3(1, height * stepHeight, 1);
	}

	//private IEnumerator AutoRevert()
	//{
	//    yield return new WaitForSeconds(3);
	//    RevertTile();
	//}
	#endregion

	/// <summary>
	/// Get info on tile for debugging or display purposes in UI Target panel
	/// </summary>
	public string GetTileSummary()
	{
		return "" ("" + this.pos.x + "","" + this.pos.y + "")"" + "" unitID: "" + this.unitId + "" height: "" + this.height;
	}
}
		    5.) Focus on the tile.pos and tile.height properties. The rest of the properties will be defaults.

		    6.) Populate the dictionary with <Point, Tile> pairs where the Point is the key and the Tile is the value.

		    7.) The Point is the x and y coordinates of the tile. The Tile is the tile object itself.

		    8.) The dictionary should be populated with all the tiles in the map.

		    9.) The map is a 2D grid of tiles. The tiles are in a square grid. The tiles are not hexagonal.

		    10.) The resulting map should look like: 
        ";

		void Start()
		{
			//// Check if the API Key is set in the Inspector, just in case.
			//if (parameters == null || string.IsNullOrEmpty(parameters.apiKey))
			//{
			//	const string errorMessage = "Please set the <b>API Key</b> in the <b>ChatGPT Dialogue</b> Game Object.";
			//	Debug.LogError(errorMessage);
			//	return;
			//}

			//// This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
			//// is not provided, the request will be completed in one step, and the `completeCallback` will be called only
			//// once, with the full text of the answer.
			//ChatGpt.Request(prompt, parameters, response => { Debug.Log("Full response: " + response); },
			//				(errorCode, errorMessage) => {
			//					var errorType = (ChatGptErrorCodes)errorCode;
			//					Debug.LogError("Error: " + errorType + " - " + errorMessage);
			//				});

			//// This request provides all three callbacks: `completeCallback`, `updateCallback`, and `failureCallback`.
			//// Since the `updateCallback` is provided, the request will be completed in multiple steps, and the
			//// `completeCallback` will be called only once, with the full text of the answer.
			//ChatGpt.Request(prompt, parameters, response => { Debug.Log("Full response: " + response); },
			//				(errorCode, errorMessage) => {
			//					var errorType = (ChatGptErrorCodes)errorCode;
			//					Debug.LogError("Error: " + errorType + " - " + errorMessage);
			//				}, chunk => { Debug.Log("Next part of response: " + chunk); });

			isNewChat = true;
		}

		void OnDestroy()
		{
			ChatGpt.CancelAllRequests();
		}

		void SendPointTileDictToMapCreator(List<SerializableVector3> sv3List)
		{


			//Old way doesn't work due to not being able to convert LLM output to dictionary
			//Dictionary<Point, Tile> testTilesDict = new Dictionary<Point, Tile>
			//{
			//	{ new Point(0, 0), new Tile { pos = new Point(0, 0), height = 1 } },
			//	{ new Point(0, 1), new Tile { pos = new Point(0, 1), height = 1 } },
			//	{ new Point(1, 0), new Tile { pos = new Point(1, 0), height = 1 } },
			//	{ new Point(1, 1), new Tile { pos = new Point(1, 1), height = 1 } }
			//};

			//mapCreator.SetLevelFromTiles(testTilesDict);

			var testSv3List = new List<SerializableVector3>
			{
				new SerializableVector3(0, 1, 1),
				new SerializableVector3(0, 1, 1),
				new SerializableVector3(1, 1, 1),
				new SerializableVector3(1, 1, 1)
			};

			mapCreator.SetLevelFromSerializableVector3List(testSv3List);
			//mapCreator.SetLevelFromSerializableVector3List(sv3List);

		}

		public void SubmitText()
		{
			string submitPrompt;

			if( isNewChat)
				submitPrompt = basePrompt + inputField.text;
			else
				submitPrompt = inputField.text;

			// This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
			// is not provided, the request will be completed in one step, and the `completeCallback` will be called only
			// once, with the full text of the answer.
			ChatGpt.Request(prompt, parameters, response => { 
					Debug.Log("Full response: " + response);
				// stopped here: parse string to dictionary
				SendPointTileDictToMapCreator(null);
				},
				(errorCode, errorMessage) =>
				{
					var errorType = (ChatGptErrorCodes)errorCode;
					Debug.LogError("Error: " + errorType + " - " + errorMessage);
				});

			
			SendPointTileDictToMapCreator(null);

			isNewChat = false;
		}

		public void StartNewConversation()
		{
			// Implement the logic to start a new conversation.
			//chatDisplay.text = "New conversation started.\n";
			isNewChat = true;
		}

		public void ClearText()
		{
			inputField.text = ""; // Simply clear the input field.
		}
	}
}
