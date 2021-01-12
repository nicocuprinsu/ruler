namespace FindACountry
{
    using UnityEngine;

    public class CountryPoint : MonoBehaviour
    {

        private CountryController m_controller;

        void Awake()
        {
            m_controller = FindObjectOfType<CountryController>();
        }

        public Vector3 Pos
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                gameObject.transform.position = value;
            }
        }

        //void OnMouseDown()
        //{
        //    m_controller.m_searchedPoint = this;
        //    m_controller.m_line.SetPosition(0, Pos);
        //}

        //void OnMouseEnter()
        //{
        //    if (m_controller.m_firstPoint == null) return;

        //    m_controller.m_locked = true;
        //    m_controller.m_secondPoint = this;
        //    m_controller.m_line.SetPosition(1, Pos);
        //}

        //void OnMouseExit()
        //{
        //    if (this != m_controller.m_secondPoint) return;

        //    m_controller.m_locked = false;
        //    m_controller.m_secondPoint = null;
        //    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + 10 * Vector3.forward);
        //    m_controller.m_line.SetPosition(1, pos);
        //}
    }
}
