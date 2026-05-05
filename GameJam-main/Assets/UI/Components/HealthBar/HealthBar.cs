using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    namespace Components
    {
        public class HealthBar
        {
            private readonly VisualElement m_HealthBarMask;

            // ON BEST - INFO FROM PLAYER CONTROL

            private const int DEBUG_MAX_HEALTH = 100;
            private int debug_health = 100;
            private bool DEBUG_healthChanged = true;
            private readonly System.Func<float> getter = null;
            private readonly System.Func<float> maxGetter = null;
            public void DebugSimulateHealthChange(int addValue = 0)
            {
                if (debug_health > 0 + addValue)
                {
                    debug_health -= 10;
                }
                else
                {
                    debug_health = DEBUG_MAX_HEALTH;
                }
                DEBUG_healthChanged = true;
            }

            public HealthBar(string barName, UIDocument doc, System.Func<float> g, System.Func<float> mg)
            {
                m_HealthBarMask = doc.rootVisualElement.Q<VisualElement>(barName);
                if (m_HealthBarMask == null)
                {
                    Debug.LogWarning($"Could not find '{barName}' in UI Document");
                }
                getter = g;
                maxGetter = mg;
            }
            public void LowerHealth()
            {
                DebugSimulateHealthChange();
                Debug.Log("Performing action!");
            }
            
            public void Update(bool isWidth, int min = 0, int max = 100)
            {
                if (DEBUG_healthChanged) // TODO: ADD IN SHIP MANAGER THAT HEALTH CHANGED
                {
                    float healthRatio = (float)getter.Invoke() / maxGetter.Invoke();
                    float healthPercent = Mathf.Lerp(min, max, healthRatio);
                    var lengthToCrop = Length.Percent(healthPercent);
                    if (isWidth)
                    {
                        m_HealthBarMask.style.width = lengthToCrop;
                    } else
                    {
                        m_HealthBarMask.style.height = lengthToCrop;
                    }
                }
            }
        };

    }
}
