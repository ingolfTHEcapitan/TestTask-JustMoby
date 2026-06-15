using _Project.Scripts.Data.Player;

namespace _Project.Scripts.Services.Progress
{
    public interface IProgressService
    {
        PlayerProgress PlayerProgress { get; set; }
    }
}