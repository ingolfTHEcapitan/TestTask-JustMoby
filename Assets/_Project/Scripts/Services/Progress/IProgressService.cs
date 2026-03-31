using _Project.Scripts.Data;

namespace _Project.Scripts.Services.Progress
{
    public interface IProgressService
    {
        PlayerProgress PlayerProgress { get; set; }
    }
}