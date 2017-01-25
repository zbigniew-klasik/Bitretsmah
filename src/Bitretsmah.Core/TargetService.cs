using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitretsmah.Core.Exceptions;

namespace Bitretsmah.Core
{
    public interface ITargetService
    {
        Task<IEnumerable<Target>> GetAll();

        Task SetTarget(string name, string path);
    }

    public class TargetService : ITargetService
    {
        private readonly ITargetRepository _targetRepository;

        public TargetService(ITargetRepository targetRepository)
        {
            _targetRepository = targetRepository;
        }

        public async Task<IEnumerable<Target>> GetAll()
        {
            return await _targetRepository.GetAll();
        }

        public async Task SetTarget(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(name)) // TODO: unit tests for that
            {
                throw new InvalidTargetName(name);
            }

            if (string.IsNullOrWhiteSpace(path)) // TODO: also check if path exists
            {
                throw new InvalidTargetPath(path);
            }

            await _targetRepository.AddOrUpdate(new Target { Name = name, LocalPath = path });
        }

        // TODO: remove target
    }
}