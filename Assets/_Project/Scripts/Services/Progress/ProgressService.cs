using _Project.Scripts.Data.Player;

namespace _Project.Scripts.Services.Progress
{
    public class ProgressService : IProgressService
    {
        public PlayerProgress PlayerProgress { get; set; }
    }
}