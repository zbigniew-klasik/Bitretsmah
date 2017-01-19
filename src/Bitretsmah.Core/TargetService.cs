using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await _targetRepository.AddOrUpdate(null);
        }

        // TODO: remove target
    }
}