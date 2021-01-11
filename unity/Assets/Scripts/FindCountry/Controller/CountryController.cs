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

		private List<LineSegment> m_segments;

		private List<GameObject> instantObjects;

		protected int m_levelCounter = 0;

		// Use this for initialization
		void Start()
		{
			// get unity objects
			m_segments = new List<LineSegment>();
			instantObjects = new List<GameObject>();

			InitLevel();
		}

		public void InitLevel()
		{
			if (m_levelCounter >= m_levels.Count)
			{
				SceneManager.LoadScene(m_victoryScene);
				return;
			}

			// clear old level
			Clear();

			// initialize country borders/segments
            for (int i = 0; i < m_levels[m_levelCounter].Points.Count; i += 2)
            {
				AddSegment(m_levels[m_levelCounter].Points[i], m_levels[m_levelCounter].Points[i + 1]);
            }

			AddSegment(new Vector2(-1, 0), new Vector2(0, 1));
			var obj = Instantiate(m_borderMeshPrefab, new Vector2(-1, 0), Quaternion.identity) as GameObject;
			obj.transform.parent = this.transform;
			instantObjects.Add(obj);

			m_advanceButton.Disable();
		}

		// Update is called once per frame
		void Update()
		{
		}

		public void AddSegment(Vector2 p1, Vector2 p2)
		{
			LineSegment segment = new LineSegment(p1, p2);
			m_segments.Add(segment);

			// instantiate new road mesh
			var borderMesh = Instantiate(m_borderMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
			borderMesh.transform.parent = this.transform;
			instantObjects.Add(borderMesh);

			borderMesh.GetComponent<CountrySegment>().Segment = segment;

			var borderMeshScript = borderMesh.GetComponent<ReshapingMesh>();
			borderMeshScript.CreateNewMesh(p1, p2);
		}

		/// <summary>
		/// Clears hull and relevant game objects
		/// </summary>
		private void Clear()
		{
			m_segments.Clear();

			// destroy game objects created in level
			foreach (var obj in instantObjects)
			{
				// destroy immediate
				// since controller will search for existing objects afterwards
				DestroyImmediate(obj);
			}
		}
	}
}
