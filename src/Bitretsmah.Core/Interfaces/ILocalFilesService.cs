﻿using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILocalFilesService
    {
        Task<Node> GetNodeStructure(string nodePath);
    }
}