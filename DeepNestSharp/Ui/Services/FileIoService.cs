﻿namespace DeepNestSharp.Ui.Services
{
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using DeepNestLib;
  using DeepNestSharp.Domain.Services;
  using Microsoft.Win32;

  public class FileIoService : IFileIoService
  {
    private readonly IDispatcherService dispatcherService;

    public FileIoService(IDispatcherService dispatcherService)
    {
      this.dispatcherService = dispatcherService;
    }

    public bool Exists(string filePath)
    {
      FileInfo fileInfo = new FileInfo(filePath);
      return fileInfo.Exists;
    }

    public async Task<string> GetOpenFilePathAsync(string filter, string initialDirectory)
    {
      var filePaths = await this.GetOpenFilePathsAsync(filter, initialDirectory, false).ConfigureAwait(false);
      return filePaths.First();
    }

    public async Task<string[]> GetOpenFilePathsAsync(string filter, string initialDirectory, bool allowMultiSelect = true)
    {
      try
      {
        OpenFileDialog openFileDialog = new OpenFileDialog()
        {
          Filter = filter,
          Multiselect = allowMultiSelect,
        };

        if (!string.IsNullOrWhiteSpace(initialDirectory))
        {
          openFileDialog.InitialDirectory = initialDirectory;
        }

        bool dialogResponse = false;
        await this.dispatcherService.InvokeAsync(() => dialogResponse = openFileDialog.ShowDialog() == true).ConfigureAwait(false);
        if (dialogResponse)
        {
          return openFileDialog.FileNames;
        }

        return new string[] { string.Empty };
      }
      catch (System.Exception ex)
      {
        throw;
      }
    }

    public string GetSaveFilePath(string fileDialogFilter, string? fileName = null, string? initialDirectory = null)
    {
      SaveFileDialog dlg = new SaveFileDialog()
      {
        Filter = fileDialogFilter,
      };

      if (!string.IsNullOrWhiteSpace(fileName))
      {
        dlg.FileName = fileName;
      }

      if (!string.IsNullOrWhiteSpace(initialDirectory))
      {
        dlg.InitialDirectory = initialDirectory;
      }

      var response = dlg.ShowDialog();
      if (response.HasValue && response.Value)
      {
        return dlg.FileName;
      }

      return string.Empty;
    }
  }
}