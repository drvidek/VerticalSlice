using UnityEngine;

public class AlarmRunner : MonoBehaviour
{
    #region Singleton + Awake
    private static AlarmRunner _singleton = null;
    public static AlarmRunner Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("AlarmManager instance already exists, destroy duplicate!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    void Update()
    {
        Tick();
    }
    
    /// <summary>
    /// Mandatory to run all alarms
    /// </summary>
    private void Tick()
    {
        for (int i = 0; i < Alarm.AlarmsInUse.Count; i++)
        {
            Alarm alarm = Alarm.AlarmsInUse[i];
            alarm.Run();
            if (!Alarm.AlarmsInUse.Contains(alarm))
            {
                i--;
            }
        }
    }

    /// <summary>
    /// Creates a game object with a AlarmRunner in the scene. One AlarmRunner in a scene is mandatory to make alarms work.
    /// </summary>
    public static void CreateAlarmRunner()
    {
        if (_singleton == null)
        Instantiate(new GameObject()).AddComponent<AlarmRunner>().name = "AlarmRunner";
    }
}
