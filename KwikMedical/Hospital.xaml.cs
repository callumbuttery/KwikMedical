﻿using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KwikMedical
{
    /// <summary>
    /// Interaction logic for Hospital.xaml
    /// </summary>
    public partial class Hospital : Window
    {

        //initialise firebase server connection
        public static FirebaseClient firebaseClient = new FirebaseClient("https://kwikmedical-96ff6.firebaseio.com/");

        //create list to store jobs
        List<Dispatches> listOfCases = new List<Dispatches>();

        //create patient history list
        List<UserRecords> history = new List<UserRecords>();

        //hospital number entered by user from combobox
        string hospitalNumber = "";

        public Hospital()
        {
            InitializeComponent();
            hospitalBox.Items.Add("Hospital 1");
            hospitalBox.Items.Add("Hospital 2");
            hospitalBox.Items.Add("Hospital 3");
            hospitalBox.Items.Add("Hospital 4");

            getCases();
        }


        //refresh cases on screen
        private void refreshClicked(object sender, RoutedEventArgs e)
        {
            
            getCases();
        }


        async void getCases()
        {
            //checks if user has selected a hospital
            if (!string.IsNullOrEmpty(hospitalBox.Text))
            {
                //get jobs
                listOfCases = (await firebaseClient
                    .Child("Dispatches")
                    .OnceAsync<Dispatches>()).Where(a => a.Object.onWayToHospital == true).Where(b => b.Object.hosptial == hospitalNumber).Select(item => new Dispatches
                    {
                        nhsID = item.Object.nhsID,
                        caseNumber = item.Object.caseNumber,
                        patientAddress = item.Object.patientAddress,
                        ambulance = item.Object.ambulance,
                        condition = item.Object.condition,
                        helpGiven = item.Object.helpGiven,
                        active = item.Object.active,
                        medicName = item.Object.medicName,
                        timeOfArrival = item.Object.timeOfArrival,
                        timeOfFinish = item.Object.timeOfFinish,
                        onWayToHospital = item.Object.onWayToHospital
                    }).ToList();

                //clear list box
                casesBox.Items.Clear();
                //display job details to screen
                foreach(var item in listOfCases)
                {
                    casesBox.Items.Add("Ambulance: " + item.ambulance);
                    casesBox.Items.Add("Case No: " + item.caseNumber);
                    casesBox.Items.Add("Condition: " + item.condition);
                    casesBox.Items.Add("Help Given: " + item.helpGiven);
                    casesBox.Items.Add("NHS ID : " + item.nhsID);
                    casesBox.Items.Add("Patient Address: " + item.patientAddress);
                    casesBox.Items.Add("time of arrival: " + item.timeOfArrival);
                    casesBox.Items.Add("time of finish: " + item.timeOfFinish);
                    casesBox.Items.Add("On way to hospital: " + item.onWayToHospital);
                    casesBox.Items.Add("\n\n\n\n");
                }   
            
                
            }
        }

        //user selects hospital
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            casesBox.Items.Clear();
            if (!string.IsNullOrEmpty(hospitalBox.SelectedItem.ToString()))
            {
                hospitalNumber = hospitalBox.SelectedItem.ToString();
                getCases();
            }

        }

        //used to search for patient records
        async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(IDEntry.Text))
            {
                //get id thats been entered
                string id = IDEntry.Text;

                //get history
                history = (await firebaseClient
                    .Child("UserRecords")
                    .OnceAsync<UserRecords>()).Where(a => a.Object.NHSid == id).Select(item => new UserRecords
                    {
                        NHSid = item.Object.NHSid,
                        name = item.Object.name,
                        medicalCondition = item.Object.medicalCondition,
                        dateTime = item.Object.dateTime,
                        address = item.Object.address



                    }).ToList();

                //order list
                var orderedHistory = history.OrderByDescending(x => x.dateTime).ToList();

                //clear list box
                historyBox.Items.Clear();

                //display each item
                foreach(var item in orderedHistory)
                {
                    historyBox.Items.Add("NHS ID: " + item.NHSid);
                    historyBox.Items.Add("Name: " + item.name);
                    historyBox.Items.Add("Medical Condition: " + item.medicalCondition);
                    historyBox.Items.Add("Date of event: " + item.dateTime);
                    historyBox.Items.Add("Address: " + item.address);
                    historyBox.Items.Add("\n");
                }

            }
        }
    }
}
