using StabilityMatrix.Core.Models.Api;
using StabilityMatrix.Core.Models;

namespace StabilityMatrix.Avalonia.ViewModels.Dialogs;

public class CivitFileDisplayViewModel
{
    public required CivitModelVersion ModelVersion { get; init; }
    public required CivitFileViewModel FileViewModel { get; init; }

    public bool IsInstalled => FileViewModel.IsInstalled;
    public string FileName => FileViewModel.CivitFile.Name;
    public FileSizeType FileSize => FileViewModel.CivitFile.FullFilesSize;
}
