using System.Collections.Generic;
using System.Linq;
using UI.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public UIDocument UIMainScreenDoc;
        public UIDocument popupUIDoc;
        public UIDocument loseGameUIDoc;
        public UIDocument startUIDoc;
        public Popup pausePopup;
        public Popup startPopup;
        public Popup losePopup;
        public ShipManager ship;
        private HealthBar healthBar;
        private HealthBar insanityBar;
        private Label coinLabel;
        private int numberOfCoins = 0;
        private bool paused = false;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Pause(true);
            if (UIMainScreenDoc != null && UIMainScreenDoc.rootVisualElement != null)
            {
                // getting health label
                healthBar = new HealthBar("healthbar_mask", UIMainScreenDoc, () => ship.CurrentHealth, () => ship.MaxHealth);
                //insanityBar = new HealthBar("insanity_mask", UIMainScreenDoc, () => ship.CurrentMadness, () => ship.maxMadnessValue); // TODO: generalize class
                coinLabel = UIMainScreenDoc.rootVisualElement.Q<Label>("coins_num");
                //getting bottoms and assigning functions to them
                // AssignActionToButton("pause", GetPauseMenu);
                //AssignActionToButton("DEBUG_LWR_H", ship.CurrentHealth);
                //getting popup
                FindPopups();
                pausePopup.AssignActionToButton("resume", GetPauseMenu);
                pausePopup.AssignActionToButton("exit", Exit);
                startPopup.AssignActionToButton("play", StartGame);
                startPopup.Open();
                losePopup.AssignActionToButton("exit", Exit);
            }
            else
            {
                Debug.LogError("UIDocument not found. Make sure there's a UIDocument in the scene.");
            }
        }

        void AssignActionToButton(string buttonName, System.Action action)
        {
            var button = UIMainScreenDoc.rootVisualElement.Q<Button>(buttonName);
            if (button != null)
            {
                button.clicked += action;
            }
            else
            {
                Debug.LogError($"Could not find '{buttonName}' in UI Document");
            }
        }

        // void GetTaskBars(string buttonName, System.Action action)
        // {
        //     List<ShipTask> tasks = new();
        //     foreach (var ZoneManager in ship.shipTaskZones)
        //     {
        //         foreach (var task in ZoneManager.TaskList)
        //         {
        //             tasks.Add(task);
        //         }
        //     }
        //     foreach (var task in tasks) 
        //     {
        //         int percent = (int)((task.CurrentProgress / task.timeToComplete) * 100);
        //         bool isFailed = task.taskIsFailed();
        //     }
        // }

        void FindPopups()
        {
            if (popupUIDoc != null)
            {
                pausePopup = new Popup("pause_popup", popupUIDoc);
                startPopup = new Popup("start_popup", startUIDoc);
                losePopup = new Popup("lose_popup", loseGameUIDoc);
            }
            else
            {
                Debug.LogError($"Could not find popups because UI Document does not exist.");
            }
        }

        void Update()
        {
            healthBar?.Update(false);
            insanityBar?.Update(true);
            if (!paused && Input.GetKeyDown(KeyCode.Space))
            {
                GetPauseMenu();
            }

            coinLabel.text = ship.CurrentCoins.ToString(); // # SHOULD BE MONEY VAR

            // TOGGLE FOR EASY GAME OVER
            if (!paused && ship.CurrentHealth <= 0)
            {
                Lose();
            }
        }

        public void StartGame()
        {
            if (startUIDoc != null)
            {
                startPopup.Close();
                Pause(false);
            }
            else
            {
                Debug.LogWarning("Could not toggle popup. It was not loaded.");
            }
        }

        public void GetPauseMenu()
        {
            if (pausePopup != null)
            {
                bool isOpen = pausePopup.Toggle();
                Pause(isOpen);
            }
            else
            {
                Debug.LogWarning("Could not toggle popup. It was not loaded.");
            }
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Pause(bool shouldPause)
        {
            if (shouldPause)
            {
                paused = true;
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                Time.timeScale = 1;
            }
        }

        public void Lose()
        {
            Pause(true);
            Label losePopupLabel = loseGameUIDoc.rootVisualElement.Q<Label>("coin_label");
            Label timeLabel = loseGameUIDoc.rootVisualElement.Q<Label>("time_label");
            losePopupLabel.text = ship.CurrentCoins.ToString(); // # SHOULD BE MONEY
            timeLabel.text = ((int)ship.TimeFromStart).ToString(); // # SHOULD BE TIME
            losePopup.Open();
        }
    }

}
