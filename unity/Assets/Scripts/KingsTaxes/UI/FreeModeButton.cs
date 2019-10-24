﻿namespace KingsTaxes
{
    using UnityEngine;
    using UnityEngine.UI;

    class FreeModeButton : MonoBehaviour
    {
        public string m_beatkey = "taxes_beat";
        public string m_numberOfLevelsKey = "taxes_score";

        void Start()
        {
            if (0 == PlayerPrefs.GetInt(m_beatkey, 0))
            {
                gameObject.SetActive(false);
            }
            else
            {
                var score = PlayerPrefs.GetInt(m_numberOfLevelsKey, 0);
                this.transform.Find("Panel").Find("Text").GetComponent<Text>().text = score.ToString();
            }
        }
    }
}