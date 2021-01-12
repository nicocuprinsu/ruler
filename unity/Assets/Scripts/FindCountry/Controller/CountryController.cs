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

		// Use this for initialization
		void Start()
		{
			m_advanceButton.Disable();

			// get unity objects
			m_segments = new List<LineSegment>();
			m_points = new List<CountryPoint>();
			instantObjects = new List<GameObject>();


            for (int i = 0; i < m_levels[m_levelCounter].Points.Count; i += 2)
            {
				Vector3 p1 = Camera.main.ViewportToWorldPoint(m_levels[m_levelCounter].Points[i]);
				Vector3 p2 = Camera.main.ViewportToWorldPoint(m_levels[m_levelCounter].Points[i + 1]);
				AddSegment(p1, p2);
			}
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
