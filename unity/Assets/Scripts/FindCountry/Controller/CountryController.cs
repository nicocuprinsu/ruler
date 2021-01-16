namespace FindACountry
{
	using General.Menu;
	using General.Model;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Algorithms.Polygon;
	using Util.Geometry;
	using Util.Geometry.Trapezoidal;
	using General.Controller;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class CountryController : MonoBehaviour
	{
		public LineRenderer m_line;

		[SerializeField]
		private GameObject m_borderMeshPrefab;

		[SerializeField]
		private ButtonContainer m_advanceButton;

		[SerializeField]
		private List<CountryLevel> m_levels;

		[SerializeField]
		private string m_victoryScene;

		[SerializeField]
		private GameObject m_pointPrefab;

		[SerializeField]
		private Text m_levelText;

		[SerializeField]
		private Text m_tryagainText;

		[SerializeField]
		private Image m_tryagainContainer;

		[SerializeField]
		private Text m_boromirExceptionText;

		[SerializeField]
		private Image m_boromirExceptionContainer;

		private string levelCountry;
		private CountryPoint selectedCountry;
		private string searchResult;
		private bool displayDecomposition = false;

		private List<LineSegment> m_segments;

		private List<GameObject> instantObjects;
		private List<GameObject> instantTrapezoidObjects;

		protected int m_levelCounter = -1;

		private List<CountryLineSegment> map;

		private TrapezoidalDecomposition decomp;

		// Use this for initialization
		void Start()
		{
			// get unity objects
			m_segments = new List<LineSegment>();
			instantObjects = new List<GameObject>();
			instantTrapezoidObjects = new List<GameObject>();
			m_advanceButton.Disable();
			m_tryagainContainer.gameObject.SetActive(false);
			m_tryagainText.gameObject.SetActive(false);
			m_boromirExceptionContainer.gameObject.SetActive(false);
			m_boromirExceptionText.gameObject.SetActive(false);

			map = new List<CountryLineSegment>();
			map.Add(new CountryLineSegment(new Vector2(0.27f, 0.3178717598908595f), new Vector2(0.32f, 0.41336971350613916f), "null"));
            map.Add(new CountryLineSegment(new Vector2(0.32f, 0.41336971350613916f), new Vector2(0.3f, 0.48158253751705316f), "Gondor"));
            map.Add(new CountryLineSegment(new Vector2(0.27f, 0.3178717598908595f), new Vector2(0.45f, 0.3997271487039563f), "Gondor"));
            map.Add(new CountryLineSegment(new Vector2(0.3f, 0.48158253751705316f), new Vector2(0.34f, 0.5361527967257844f), "Eriador"));
            map.Add(new CountryLineSegment(new Vector2(0.34f, 0.5361527967257844f), new Vector2(0.47f, 0.5634379263301501f), "Eriador"));
            map.Add(new CountryLineSegment(new Vector2(0.47f, 0.5634379263301501f), new Vector2(0.68f, 0.4679399727148704f), "Rohan"));
            map.Add(new CountryLineSegment(new Vector2(0.45f, 0.3997271487039563f), new Vector2(0.51f, 0.20873124147339694f), "Gondor"));
            map.Add(new CountryLineSegment(new Vector2(0.51f, 0.20873124147339694f), new Vector2(0.61f, 0.16780354706684852f), "Gondor"));
            map.Add(new CountryLineSegment(new Vector2(0.61f, 0.16780354706684852f), new Vector2(0.77f, 0.2633015006821282f), "Gondor"));
            map.Add(new CountryLineSegment(new Vector2(0.68f, 0.4679399727148704f), new Vector2(0.77f, 0.2633015006821282f), "Mordor"));
            map.Add(new CountryLineSegment(new Vector2(0.66f, 0.5361527967257844f), new Vector2(0.68f, 0.4679399727148704f), "Mordor"));
            map.Add(new CountryLineSegment(new Vector2(0.66f, 0.5361527967257844f), new Vector2(0.84f, 0.5497953615279673f), "Rhovanion"));
            map.Add(new CountryLineSegment(new Vector2(0.77f, 0.2633015006821282f), new Vector2(0.83f, 0.24965893587994548f), "Mordor"));
            map.Add(new CountryLineSegment(new Vector2(0.83f, 0.24965893587994548f), new Vector2(0.97f, 0.3587994542974079f), "Mordor"));
            map.Add(new CountryLineSegment(new Vector2(0.97f, 0.3587994542974079f), new Vector2(0.98f, 0.5088676671214188f), "Mordor"));
            map.Add(new CountryLineSegment(new Vector2(0.38f, 0.017735334242837686f), new Vector2(0.51f, 0.20873124147339694f), "null"));
            map.Add(new CountryLineSegment(new Vector2(0.38f, 0.017735334242837686f), new Vector2(0.69f, 0.03137789904502042f), "NearHarad"));
            map.Add(new CountryLineSegment(new Vector2(0.89f, 0.07230559345156895f), new Vector2(0.97f, 0.3587994542974079f), "NearHarad"));
            map.Add(new CountryLineSegment(new Vector2(0.69f, 0.03137789904502042f), new Vector2(0.89f, 0.07230559345156895f), "NearHarad"));
            map.Add(new CountryLineSegment(new Vector2(0.55f, 0.6862210095497954f), new Vector2(0.66f, 0.5361527967257844f), "Rhovanion"));
            map.Add(new CountryLineSegment(new Vector2(0.47f, 0.5634379263301501f), new Vector2(0.55f, 0.6862210095497954f), "Eriador"));
            map.Add(new CountryLineSegment(new Vector2(0.55f, 0.6862210095497954f), new Vector2(0.56f, 0.9045020463847203f), "Eriador"));
            map.Add(new CountryLineSegment(new Vector2(0.56f, 0.9045020463847203f), new Vector2(0.81f, 0.8362892223738063f), "Forodwaith"));
            map.Add(new CountryLineSegment(new Vector2(0.81f, 0.8362892223738063f), new Vector2(0.84f, 0.5497953615279673f), "Rhun"));
            map.Add(new CountryLineSegment(new Vector2(0.84f, 0.5497953615279673f), new Vector2(0.98f, 0.5088676671214188f), "Rhun"));
            map.Add(new CountryLineSegment(new Vector2(0.81f, 0.8362892223738063f), new Vector2(0.92f, 0.9454297407912687f), "Forodwaith"));
            map.Add(new CountryLineSegment(new Vector2(0.92f, 0.9454297407912687f), new Vector2(0.98f, 0.5088676671214188f), "null"));
            map.Add(new CountryLineSegment(new Vector2(0.31f, 0.9181446111869032f), new Vector2(0.56f, 0.9045020463847203f), "Forodwaith"));
            map.Add(new CountryLineSegment(new Vector2(0.09f, 0.7407912687585265f), new Vector2(0.31f, 0.9181446111869032f), "null"));
            map.Add(new CountryLineSegment(new Vector2(0.09f, 0.7407912687585265f), new Vector2(0.3f, 0.48158253751705316f), "Eriador"));
            map.Add(new CountryLineSegment(new Vector2(0.28f, 0.9727148703956344f), new Vector2(0.31f, 0.9181446111869032f), "Forodwaith"));
            map.Add(new CountryLineSegment(new Vector2(0.28f, 0.9727148703956344f), new Vector2(0.73f, 0.9863574351978172f), "null"));
            map.Add(new CountryLineSegment(new Vector2(0.73f, 0.9863574351978172f), new Vector2(0.92f, 0.9454297407912687f), "null"));

            decomp = new TrapezoidalDecomposition(map);

			// get unity objects
			instantObjects = new List<GameObject>();
            instantTrapezoidObjects = new List<GameObject>();

			// Display segments on screen.
			foreach (CountryLineSegment seg in map)
			{
				AddSegment(seg);
			}

			AdvanceLevel();
		}

		public void AdvanceLevel()
		{
			m_levelCounter++;

			// clear old level
			m_tryagainContainer.gameObject.SetActive(false);
			m_tryagainText.gameObject.SetActive(false);
			m_advanceButton.Disable();

			if (selectedCountry != null)
			{
				Destroy(selectedCountry.gameObject);
			}

			if (m_levelCounter < m_levels.Count)
			{
				InitLevel();
			}
			else
			{
				SceneManager.LoadScene(m_victoryScene);
			}
		}

		public void InitLevel()
		{
			// create new level
			var level = m_levels[m_levelCounter];
			levelCountry = level.Country;

			// update text box
			m_levelText.text = "Conquer " + levelCountry + "!";
		}

        // Displays new segment on the map
		public void AddSegment(CountryLineSegment segment, List<GameObject> objects = null)
		{
			// instantiate new road mesh
			var roadmesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
			roadmesh.transform.parent = this.transform;
            if (objects != null)
            {
				objects.Add(roadmesh);
			}
			
			Vector3 p1 = Camera.main.ViewportToWorldPoint(segment.Point1);
			Vector3 p2 = Camera.main.ViewportToWorldPoint(segment.Point2);

			var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
			roadmeshScript.CreateNewMesh(p1, p2);
		}

		// Update is called once per frame
		void Update()
		{

			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				// remove previous selected point
				if (selectedCountry != null)
				{
					Destroy(selectedCountry.gameObject);
				}

				// instantiate new selected point and display point prefab
				var worldlocationDisplay = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
				worldlocationDisplay.z = -2f;
				var go = Instantiate(m_pointPrefab, worldlocationDisplay, Quaternion.identity) as GameObject;
				selectedCountry = FindObjectsOfType<CountryPoint>()[0];
				selectedCountry.Pos = worldlocationDisplay;
				Debug.Log(selectedCountry.Pos);

				var worldlocation = Input.mousePosition;
				worldlocation.x /= Screen.width;
				worldlocation.y /= Screen.height;
				m_tryagainText.gameObject.SetActive(false);
				m_tryagainContainer.gameObject.SetActive(false);
				Debug.Log(worldlocation);
				CheckSolution(worldlocation);
			}

			if (Input.GetKeyDown(KeyCode.T))
			{
				if (!displayDecomposition)
				{
					DisplayTrapezoidalDecomposition();
					displayDecomposition = true;
				}
				else
				{
					RemoveTrapezoidalDecomposition();
					displayDecomposition = false;
				}

			}
		}

		public void CheckSolution(Vector3 worldlocation)
		{
			// retrieve which country has been clicked by the user from trapezoidal decomp
			Trapezoid res = (Trapezoid)decomp.SearchGraph.Search(worldlocation).Value;
			searchResult = res.Bottom.Country;
			if (searchResult == levelCountry)
			{
				if (levelCountry == "Mordor")
				{
					m_boromirExceptionContainer.gameObject.SetActive(true);
					m_boromirExceptionText.gameObject.SetActive(true);
				}
				m_advanceButton.Enable();
			}
			else
			{
				m_tryagainContainer.gameObject.SetActive(true);
				m_tryagainText.gameObject.SetActive(true);
				m_advanceButton.Disable();
				if (searchResult == "null")
				{
					m_tryagainText.text = "Try again! You clicked outside the map!";
				}
				else
				{
					m_tryagainText.text = "Try again! You selected " + searchResult.ToString() + "!";
				}
			}
		}

		public void RemoveTrapezoidalDecomposition()
		{
			// destroy game objects created in level
			foreach (var obj in instantTrapezoidObjects)
			{
				// destroy immediate
				// since controller will search for existing objects afterwards
				DestroyImmediate(obj);
			}
		}

		public void DisplayTrapezoidalDecomposition()
		{
			foreach (Trapezoid t in decomp.TrapezoidalMap.Trapezoids)
			{
				AddSegment(t.Top);
				AddSegment(t.Bottom);
				if (t.Top.isAbove(t.RightPoint) == -1)
				{
					LineSegment up = new LineSegment(t.RightPoint, new Vector2(t.RightPoint.x, decomp.BoundingBox.Top.Point1.y + 0.1f));
					if (t.Top.Segment.Intersect(up).HasValue)
					{
						LineSegment intersect = new LineSegment(t.RightPoint, (Vector2)t.Top.Segment.Intersect(up));

						AddSegment(new CountryLineSegment(intersect), instantTrapezoidObjects);
					}
				}

				if (t.Bottom.isAbove(t.RightPoint) == 1)
				{
					LineSegment down = new LineSegment(t.RightPoint, new Vector2(t.RightPoint.x, decomp.BoundingBox.Bottom.Point1.y - 0.1f));
					if (t.Bottom.Segment.Intersect(down).HasValue)
					{
						LineSegment intersect = new LineSegment(t.RightPoint, (Vector2)t.Bottom.Segment.Intersect(down));

						AddSegment(new CountryLineSegment(intersect), instantTrapezoidObjects);
					}
				}

				if (t.Top.isAbove(t.LeftPoint) == -1)
				{
					LineSegment up = new LineSegment(t.LeftPoint, new Vector2(t.LeftPoint.x, decomp.BoundingBox.Top.Point1.y + 0.1f));
					if (t.Top.Segment.Intersect(up).HasValue)
					{
						LineSegment intersect = new LineSegment(t.LeftPoint, (Vector2)t.Top.Segment.Intersect(up));

						AddSegment(new CountryLineSegment(intersect), instantTrapezoidObjects);
					}
				}

				if (t.Bottom.isAbove(t.LeftPoint) == 1)
				{
					LineSegment down = new LineSegment(t.LeftPoint, new Vector2(t.LeftPoint.x, decomp.BoundingBox.Bottom.Point1.y - 0.1f));
					if (t.Bottom.Segment.Intersect(down).HasValue)
					{
						LineSegment intersect = new LineSegment(t.LeftPoint, (Vector2)t.Bottom.Segment.Intersect(down));

						AddSegment(new CountryLineSegment(intersect), instantTrapezoidObjects);
					}
				}
			}
		}
	}
}
