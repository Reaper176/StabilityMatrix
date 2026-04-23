using StabilityMatrix.Core.Models.Database;
using StabilityMatrix.Core.Models.Api;
using StabilityMatrix.Core.Services;

namespace StabilityMatrix.Avalonia.ViewModels.Dialogs;

internal static class CivitFileInstallDetector
{
    public static bool IsInstalled(IModelIndexService modelIndexService, CivitFile civitFile)
    {
        if (civitFile.Type != CivitFileType.Model)
        {
            return false;
        }

        var installedHashes = modelIndexService.ModelIndexBlake3Hashes ?? new HashSet<string>();

        if (civitFile.Hashes?.BLAKE3 is { Length: > 0 } blake3Hash
            && installedHashes.Contains(blake3Hash))
        {
            return true;
        }

        var indexedFiles = modelIndexService.ModelIndex?.Values ?? Enumerable.Empty<List<LocalModelFile>>();

        return indexedFiles
            .Where(static files => files is not null)
            .SelectMany(static files => files)
            .Any(localModel => FileNameMatches(localModel, civitFile.Name));
    }

    private static bool FileNameMatches(LocalModelFile localModel, string civitFileName)
    {
        return string.Equals(localModel.FileName, civitFileName, StringComparison.OrdinalIgnoreCase);
    }
}
