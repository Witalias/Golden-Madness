using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private bool forcedTutorial = false;

    public bool ForcedTutorial { get => forcedTutorial; }

    //private void Start()
    //{
    //    Time.timeScale = 1f;
    //}
}
