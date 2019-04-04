﻿using EbookApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Question : ContentPage
    {
        public Question(QuestionItem questionItem)
        {
            InitializeComponent();
             
            QOne.Text = questionItem.QOne;
            QTwo.Text = questionItem.QTwo;
            QThree.Text = questionItem.QThree;
             
            BindingContext = new QuestionItem
            {
                ID = questionItem.ID,
                Genre = questionItem.Genre,
                Title = questionItem.Title,
                QOne = QOne.Text,
                QTwo = QTwo.Text,
                QThree = QThree.Text
            } as QuestionItem;


        }
         
        private async void Submit_Clicked(object sender, EventArgs e)
        { 
            var item = (QuestionItem)BindingContext;             
            await App.Database.SaveItemAsync(item);
            await Navigation.PopModalAsync();
        }

    }
}