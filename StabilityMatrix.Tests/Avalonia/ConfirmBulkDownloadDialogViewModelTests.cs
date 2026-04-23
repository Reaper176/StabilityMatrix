using NSubstitute;
using StabilityMatrix.Avalonia.Services;
using StabilityMatrix.Avalonia.ViewModels.Base;
using StabilityMatrix.Avalonia.ViewModels.Dialogs;
using StabilityMatrix.Core.Models.Api;
using StabilityMatrix.Core.Models.Database;
using StabilityMatrix.Core.Models;
using StabilityMatrix.Core.Services;

namespace StabilityMatrix.Tests.Avalonia;

[TestClass]
public class ConfirmBulkDownloadDialogViewModelTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
    }

    [TestMethod]
    public async Task OnLoadedAsync_KeepsInstalledFilesVisibleInBulkList()
    {
        var modelIndexService = Substitute.For<IModelIndexService>();
        modelIndexService.ModelIndexBlake3Hashes.Returns(new HashSet<string> { "installed-hash" });
        modelIndexService.ModelIndex.Returns(new Dictionary<SharedFolderType, List<LocalModelFile>>());

        var viewModel = new ConfirmBulkDownloadDialogViewModel(
            modelIndexService,
            Substitute.For<ISettingsManager>(),
            Substitute.For<IServiceManager<ViewModelBase>>()
        )
        {
            Model = new CivitModel
            {
                ModelVersions =
                [
                    new CivitModelVersion
                    {
                        Files =
                        [
                            CreateFile(1, "already-there.safetensors", "installed-hash"),
                            CreateFile(2, "new-file.safetensors", "new-hash"),
                        ],
                    },
                ],
            },
        };

        await viewModel.OnLoadedAsync();

        Assert.AreEqual(2, viewModel.FilesToDownload.Count);
        Assert.IsTrue(viewModel.FilesToDownload.Single(x => x.FileViewModel.CivitFile.Id == 1).FileViewModel.IsInstalled);
        Assert.IsFalse(viewModel.FilesToDownload.Single(x => x.FileViewModel.CivitFile.Id == 2).FileViewModel.IsInstalled);
    }

    [TestMethod]
    public async Task OnLoadedAsync_MarksFileInstalled_WhenFilenameAlreadyExistsInIndex()
    {
        var modelIndexService = Substitute.For<IModelIndexService>();
        modelIndexService.ModelIndexBlake3Hashes.Returns(new HashSet<string>());
        modelIndexService.ModelIndex.Returns(
            new Dictionary<SharedFolderType, List<LocalModelFile>>
            {
                [SharedFolderType.StableDiffusion] =
                [
                    new LocalModelFile
                    {
                        RelativePath = "StableDiffusion/already-there.safetensors",
                        SharedFolderType = SharedFolderType.StableDiffusion,
                    },
                ],
            }
        );

        var viewModel = new ConfirmBulkDownloadDialogViewModel(
            modelIndexService,
            Substitute.For<ISettingsManager>(),
            Substitute.For<IServiceManager<ViewModelBase>>()
        )
        {
            Model = new CivitModel
            {
                ModelVersions =
                [
                    new CivitModelVersion
                    {
                        Files =
                        [
                            CreateFile(1, "already-there.safetensors", null),
                        ],
                    },
                ],
            },
        };

        await viewModel.OnLoadedAsync();

        Assert.IsTrue(viewModel.FilesToDownload.Single().FileViewModel.IsInstalled);
    }

    private static CivitFile CreateFile(int id, string name, string? blake3Hash)
    {
        return new CivitFile
        {
            Id = id,
            Name = name,
            SizeKb = 1024,
            Type = CivitFileType.Model,
            Hashes = new CivitFileHashes { BLAKE3 = blake3Hash },
            Metadata = new CivitFileMetadata(),
            PickleScanResult = string.Empty,
            VirusScanResult = string.Empty,
            DownloadUrl = string.Empty,
        };
    }
}
