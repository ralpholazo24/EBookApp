
using PCLStorage;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
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
        }

        private async void Choose_Clicked(object sender, EventArgs e)
        {
            try
            {
                fileData = new FileData();
                fileData = await CrossFilePicker.Current.PickFile(); // Initialization for choosing or selecting the files you want to upload.
                string filePath = fileData.FilePath; // Getting the file path of the file that you will upload.

                if (fileData == null) // Validation if there is no file selected.
                    return; // user cancelled file picking

                string fileName = fileData.FileName; // Getting the file name of the file you will upload.                
                
                await DisplayAlert("File Selected", "Location: " + filePath, "OK");
                lblFilePath.Text = "File Path: " + filePath;

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

                    IFile file = await folder.GetFileAsync(fileData.FilePath);

                    bool copyFile = false;
                    if (isExistFile)
                    {
                        // Validation if existing file
                        await PCLHelper.DeleteFile(fileData.FileName, folder);
                        copyFile = await PCLHelper.CopyFileTo(file, folder);
                    }
                    else
                    {
                        try
                        {
                            // Getting text content from pdf, word and text file.                    
                            var filepath = file.Path;
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
                            lblFilePath.Text = string.Empty;

                        }
                    }
                    
                    if (copyFile)
                    {
                        await DisplayAlert("Success", "Import file successfully!", "OK");
                        fileData = new FileData();
                        Genre.SelectedIndex = -1;
                    }

                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
                return;
            }
        }
         
    }
}