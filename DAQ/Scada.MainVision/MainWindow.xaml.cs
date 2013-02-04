﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scada.MainVision
{
    public class Item
    {
        public string Name { get; set; }


        public string Date { get; set; }
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            List<Item> a = new List<Item>();
            a.Add(new Item() { Name = "AA", Date = "fasadfsda" });
            a.Add(new Item() { Name = "VV", Date = "sdafdsaf" });
            a.Add(new Item() { Name = "FF", Date = "fasdf" });
            this.list.ItemsSource = a;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<Item> a = new List<Item>();
            a.Add(new Item() { Name = "AA", Date = "fasadfsda" });
            a.Add(new Item() { Name = "VV", Date = "sdafdsaf" });
            a.Add(new Item() { Name = "AA", Date = "fasadfsda" });
            a.Add(new Item() { Name = "VV", Date = "sdafdsaf" });
            a.Add(new Item() { Name = "FF", Date = "fasdf" });
            a.Add(new Item() { Name = "FF", Date = "fasdf" });
            this.list.ItemsSource = a;
        }


    }
}
