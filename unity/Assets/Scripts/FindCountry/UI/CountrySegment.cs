namespace FindACountry
{
    using UnityEngine;
    using Util.Geometry;

    public class CountrySegment : MonoBehaviour
    {
        public LineSegment Segment { get; set; }

        private CountryController m_controller;

        void Awake()
        {
            m_controller = FindObjectOfType<CountryController>();
        }
    }
}