using UnityEngine;
using UnityEngine.UI;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        public Image ImgHealthBar;

        public Text TxtHealth;

        public int Min;

        public int Max;

        private float mCurrentPercent;

        private int mCurrentValue;

        public float CurrentPercent
        {
            get { return mCurrentPercent; }
        }

        public int CurrentValue
        {
            get { return mCurrentValue; }
        }

        // Use this for initialization
        void Start()
        {
        }

        public void SetValue(int health)
        {
            if (health != mCurrentValue)
            {
                if (Max - Min == 0)
                {
                    mCurrentValue = 0;
                    mCurrentPercent = 0;
                }
                else
                {
                    mCurrentValue = health;
                    mCurrentPercent = (float) mCurrentValue / (float) (Max - Min);
                }

                TxtHealth.text = string.Format("{0} %", Mathf.RoundToInt(mCurrentPercent * 100));

                ImgHealthBar.fillAmount = mCurrentPercent;
            }
        }
    }
}