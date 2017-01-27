using System;
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
        private readonly ILocalFilesService _localFilesService;
        private readonly ITargetRepository _targetRepository;

        public TargetService(ILocalFilesService localFilesService, ITargetRepository targetRepository)
        {
            _localFilesService = localFilesService;
            _targetRepository = targetRepository;
        }

        public async Task<IEnumerable<Target>> GetAll()
        {
            return await _targetRepository.GetAll();
        }

        public async Task SetTarget(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new EmptyTargetNameException();
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new EmptyTargetPathException();
            }

            if (!_localFilesService.Exists(path))
            {
                throw new InvalidTargetPathException(path);
            }

            await _targetRepository.AddOrUpdate(new Target { Name = name, LocalPath = path });
        }

        // TODO: remove target
    }
}