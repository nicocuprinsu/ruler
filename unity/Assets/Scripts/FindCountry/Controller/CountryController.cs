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

		private List<LineSegment> m_segments;

		private List<GameObject> instantObjects;

		protected int m_levelCounter = 0;

		private List<Vector2> PointsTest;

		private List<CountryPoint> m_points;

		private List<CountryLineSegment> map = new List<CountryLineSegment>();

		// Use this for initialization
		void Start()
		{


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
			map.Add(new CountryLineSegment(new Vector2(0.34f, 0.017735334242837686f), new Vector2(0.51f, 0.20873124147339694f), "null"));
			map.Add(new CountryLineSegment(new Vector2(0.34f, 0.017735334242837686f), new Vector2(0.77f, 0.03137789904502042f), "NearHarad"));
			map.Add(new CountryLineSegment(new Vector2(0.88f, 0.09959072305593453f), new Vector2(0.97f, 0.3587994542974079f), "NearHarad"));
			map.Add(new CountryLineSegment(new Vector2(0.77f, 0.03137789904502042f), new Vector2(0.88f, 0.09959072305593453f), "NearHarad"));
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




			m_advanceButton.Disable();

			// get unity objects
			m_segments = new List<LineSegment>();
			m_points = new List<CountryPoint>();
			instantObjects = new List<GameObject>();

			foreach (CountryLineSegment seg in map)
			{
				AddSegment(seg);
			}
		}

		public void AddSegment(CountryLineSegment segment)
		{
			m_segments.Add(segment.Segment);

			// instantiate new road mesh
			var roadmesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
			roadmesh.transform.parent = this.transform;
			instantObjects.Add(roadmesh);

			Vector3 p1 = Camera.main.ViewportToWorldPoint(segment.Point1);
			Vector3 p2 = Camera.main.ViewportToWorldPoint(segment.Point2);

			var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
			roadmeshScript.CreateNewMesh(p1, p2);
		}

		public void AddSegment(Vector3 a_point1, Vector3 a_point2)
		{
			var segment = new LineSegment(a_point1, a_point2);

			// dont add double segments
			if (m_segments.Contains(segment) || m_segments.Contains(new LineSegment(a_point2, a_point1)))
				// also check reverse
				return;

			m_segments.Add(segment);

			// instantiate new road mesh
			var roadmesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
			roadmesh.transform.parent = this.transform;
			instantObjects.Add(roadmesh);

			roadmesh.GetComponent<CountrySegment>().Segment = segment;

			var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
			roadmeshScript.CreateNewMesh(a_point1, a_point2);
		}

		public void InitLevel()
		{
			//if (m_levelCounter >= m_levels.Count)
			//{
			//	//SceneManager.LoadScene(m_victoryScene);
			//	return;
			//}

			//// clear old level
			//Clear();

			//// initialize country borders/segments
			//for (int i = 0; i < m_levels[m_levelCounter].Segments.Count; i += 1)
			//{
			//    AddSegment(m_levels[m_levelCounter].Segments[i]);
			//}




		}

		// Update is called once per frame
		void Update()
		{
		}

		//public void AddSegment(Vector2 p1, Vector2 p2)
		//{
		//	LineSegment segment = new LineSegment(p1, p2);
		//	m_segments.Add(segment);

		//	// instantiate new road mesh
		//	var borderMesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
		//	borderMesh.transform.parent = this.transform;
		//	instantObjects.Add(borderMesh);

		//	borderMesh.GetComponent<CountrySegment>().Segment = segment;

		//	var borderMeshScript = borderMesh.GetComponent<ReshapingMesh>();
		//	borderMeshScript.CreateNewMesh(p1, p2);
		//}

		//     public void AddSegment(LineSegment segment)
		//      {
		//	m_segments.Add(segment);
		//	var borderMesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
		//	borderMesh.transform.parent = this.transform;
		//	instantObjects.Add(borderMesh);

		//	borderMesh.GetComponent<CountrySegment>().Segment = segment;

		//	var borderMeshScript = borderMesh.GetComponent<ReshapingMesh>();
		//	borderMeshScript.CreateNewMesh(segment.Point1, segment.Point2);
		//}



		/// <summary>
		/// Clears hull and relevant game objects
		/// </summary>
		private void Clear()
		{
			//m_segments.Clear();

			//// destroy game objects created in level
			//foreach (var obj in instantObjects)
			//{
			//	// destroy immediate
			//	// since controller will search for existing objects afterwards
			//	DestroyImmediate(obj);
			//}
		}
	}
}
