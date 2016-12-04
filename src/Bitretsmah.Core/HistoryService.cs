using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IHistoryService
    {
        Task<Node> GetLastStructure(string target);
    }

    public class HistoryService : IHistoryService
    {
        private readonly IBackupRepository _backupRepository;
        private readonly INodeChangesApplier _nodeChangesApplier;

        public HistoryService(IBackupRepository backupRepository, INodeChangesApplier nodeChangesApplier)
        {
            _backupRepository = backupRepository;
            _nodeChangesApplier = nodeChangesApplier;
        }

        public async Task<Node> GetLastStructure(string target)
        {
            Ensure.That(target).IsNotNullOrWhiteSpace();
            var backups = await _backupRepository.GetAllForTarget(target);
            if (backups == null || !backups.Any()) return null;
            var changes = backups.OrderBy(x => x.CreationTime).Select(x => x.StructureChange).ToList();
            var lastStructure = _nodeChangesApplier.Apply(changes);
            return lastStructure;
        }
    }
}