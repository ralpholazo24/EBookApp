
using PCLStorage;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
//using LeoJHarris.FilePicker.Abstractions;
//using LeoJHarris.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Import : ContentPage
    {
        private IXamHelper xamHelper;

        private FileData fileData;

        public Import()
        {
            InitializeComponent();
            // Intialization of voice recognition.
            try
            {
                xamHelper = DependencyService.Get<IXamHelper>();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        private async void Choose_Clicked(object sender, EventArgs e)
        {
            try
            {                   
                fileData = await CrossFilePicker.Current.PickFile();

                if (fileData == null) 
                    return;                                 
                 
                await DisplayAlert("File Selected", "Location: " + fileData.FilePath, "OK");
                lblFilePath.Text = "File Path: " + fileData.FilePath;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        private async void Upload_Clicked(object sender, EventArgs e)
        {
            try
            {                
                if (fileData == null)
                {
                    await DisplayAlert("", "Please select file to upload.", "OK");
                    return;
                }
                else if (Genre.SelectedIndex == -1)
                {
                    await DisplayAlert("", "Please select genre.", "OK");
                    return;
                }
                else
                {
                    String folderName = Genre.Items[Genre.SelectedIndex];

                    IFolder folder = FileSystem.Current.LocalStorage;
                    bool isExistFolder = await PCLHelper.IsFolderExistAsync(folderName, folder);

                    if (!isExistFolder)
                    {
                        folder = await PCLHelper.CreateFolder(folderName, folder);
                    }
                    else
                    {
                        folder = await folder.GetFolderAsync(folderName);
                    }

                    bool isExistFile = await PCLHelper.IsFileExistAsync(fileData.FileName, folder);                    
                    bool copyFile = false;
                    
                    if (isExistFile)
                    {
                        
                        // Validation if existing file
                        await PCLHelper.DeleteFile(fileData.FileName, folder);                        
                        try
                        {
                            IFile file = await folder.GetFileAsync(fileData.FilePath);
                            copyFile = await PCLHelper.CopyFileTo(file, folder);

                            using (var fs = new FileStream(folder.Path, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                fs.Write(fileData.DataArray, 0, fileData.DataArray.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught in process: {0}", ex);
                        }
                    }
                    else
                    {
                        try
                        {
 
                            var filepath = fileData.FilePath;// file.Path;
                            IFile file = await folder.GetFileAsync(fileData.FilePath);                            

                            string word = ConvertText.TextFile(filepath); // By default is text.

                            if (filepath.Contains(".pdf")) // if the file type is pdf.
                            {
                                word = xamHelper.PDTtoText(filepath);
                            }

                            if (filepath.Contains(".docx") || filepath.Contains(".doc")) // if the file type is word document.
                            {
                                word = xamHelper.WordToText(filepath);
                            }

                            copyFile = await PCLHelper.CopyFileTo(file, folder);

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error:", "Please try again to upload a valid file. The file must not contain images and any special symbols.", "OK");
                            ex.ToString();
                            fileData = null;
                            lblFilePath.Text = string.Empty;
                        }
                    }

                    if (copyFile)
                    {
                        await DisplayAlert("Success", "Import file successfully!", "OK");
                        fileData = null;
                        Genre.SelectedIndex = -1;
                        lblFilePath.Text = string.Empty;
                    }

                }


            }
            catch (Exception ex)
            {
                lblFilePath.Text = string.Empty;
                await DisplayAlert("Error", ex.ToString(), "OK");
                return;
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            fileData = new FileData();
            Genre.SelectedIndex = -1;
            lblFilePath.Text = string.Empty;
        }
    }
}