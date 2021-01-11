namespace FindACountry
{
    using UnityEngine;
    using Util.Geometry;

    public class CountrySegment : MonoBehaviour
    {
        public Vector2 Pos1 { get; private set; }
        public Vector2 Pos2 { get; private set; }
        public LineSegment Segment { get; set; }

        private CountryController m_controller;

        void Awake()
        {
            Pos1 = new Vector2(transform.position.x, transform.position.y);
            m_controller = FindObjectOfType<CountryController>();
        }
    }
}