namespace FindACountry
{
    using System.Collections.Generic;
    using Util.Geometry;
    using UnityEngine;

    /// <summary>
    /// Data container for convex hull level, containing point set.
    /// </summary>
    [CreateAssetMenu(fileName = "countryLevelNew", menuName = "Levels/Country Level")]
    public class CountryLevel : ScriptableObject
    {
        [Header("Map segments")]
        public List<Vector2> Points = new List<Vector2>();
    }
}