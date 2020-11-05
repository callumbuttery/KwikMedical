using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KwikMedical
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //connection to firebase server
        public static FirebaseClient firebaseClient = new FirebaseClient("https://kwikmedical-96ff6.firebaseio.com/");
        public Window1()
        {
            InitializeComponent();
        }

        //called from button click
        async void Button_Click(object sender, RoutedEventArgs e)
        {
            //ensure user has entered a value
            if (nhsNoInput.Text != null)
            {
                //search and wait to see if user exists
                var retrieveRecords = await GetRecords();

                //check returned object value
                if(retrieveRecords != null)
                {
                    //assign retrieved values
                    string patientID = retrieveRecords.NHSid;
                    string userName = retrieveRecords.name;
                    string address = retrieveRecords.address;
                    string condition = retrieveRecords.medicalCondition;

                    patientDetailView.Items.Clear();
                    patientDetailView.Items.Add("NHS ID: " + patientID);
                    patientDetailView.Items.Add("Name: " + userName);
                    patientDetailView.Items.Add("Address" + address);
                    patientDetailView.Items.Add("Medical Condition " + condition);
                }
            }
            else
            {
                MessageBox.Show("Please input an NHS number to search");
            }
        }


        //method used to check if user exists with a certain NHS number
        private async Task<UserRecords> GetRecords()
        {
            //store value
            string nhsinput = nhsNoInput_Copy.Text;
   

            //search for patient details
            var getPatient = (await firebaseClient
                .Child("UserRecords")
                .OnceAsync<UserRecords>()).Where(a => a.Object.NHSid == nhsinput).FirstOrDefault(); 

            if(getPatient == null)
            {
                //no patient found, display error
                MessageBox.Show("Sorry, No users with this NHS ID found");
                return null;
            }
            else
            {
                //return the found user as an object
                var userToReturn = getPatient.Object as UserRecords;
                return userToReturn;
            }
        }

        //register user
        async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //check if user with that nhsID exists 
            var retrieveRecords = await GetRecords();

            if(retrieveRecords != null)
            {
                MessageBox.Show("user already exists with this ID");

            }
            else
            {
                string id = NHSIDCreator.Text;

                var postUser = await firebaseClient
                    .Child("UserRecords")
                    .PostAsync(new UserRecords() { NHSid = id, name = patientNameBox.Text, address = PatientAddressBox.Text, medicalCondition = Condition.Text });
            }
        }
    }
}
