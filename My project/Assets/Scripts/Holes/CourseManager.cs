using UnityEngine;

public class CourseManager : MonoBehaviour
{
    [SerializeField] private Hole[] holes;

    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private SwingController playerController;

    public Hole CurrentHole { get; private set; }
    public int CurrentHoleIndex { get; private set; } = 0;
    public int CurrentStrokes { get; private set; } = 0;
    public bool IsComplete { get; private set; } = false;

    private void Start()
    {
        StartCourse();
    }

    private void Update()
    {
        if (IsComplete || CurrentHole == null) return;
        if (ballLauncher == null) return;

        // Vérifie si la balle est dans le trou seulement quand elle est arrêtée
        //if (ballLauncher.CurrentState == BallState.AtRest)
        //    CurrentHole.CheckBallSunk(ballLauncher.transform.position);
    }

    private void StartCourse()
    {
        CurrentHoleIndex = 0;
        IsComplete = false;
        //onCourseStarted?.Invoke();
        LoadHole(0);
    }

    private void LoadHole(int index)
    {
        if (index >= holes.Length)
        {
            //CompleteCourse();
            return;
        }

        CurrentHole = holes[index];
        CurrentStrokes = 0;

        // Place la balle au tee
        //ballLauncher.PlaceBall(CurrentHole.GetTeeTransform().position);

        // Met à jour le drapeau dans SwingController
        //swingController.SetFlag(CurrentHole.GetFlagTransform());

        CurrentHole.StartHole();
        //onHoleChanged?.Invoke(index + 1);
    }

    public void RegisterStroke() => CurrentStrokes++;
    public int GetScoreRelativeToPar()
    {
        if (CurrentHole == null) return 0;
        return CurrentStrokes - CurrentHole.GetHoleData().par;
    }

}
