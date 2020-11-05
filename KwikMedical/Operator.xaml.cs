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

        //used to store patient data
        string globalNHSID = "";
        string globalAddress = "";
        string globalCondition = "";
        public Window1()
        {
            InitializeComponent();
            
            //prepare combo boxes
            ambulanceBox.Items.Add("Ambulance 1");
            ambulanceBox.Items.Add("Ambulance 2");
            ambulanceBox.Items.Add("Ambulance 3");
            ambulanceBox.Items.Add("Ambulance 4");

            hostpitalBox.Items.Add("Hospital 1");
            hostpitalBox.Items.Add("Hospital 2");
            hostpitalBox.Items.Add("Hospital 3");
            hostpitalBox.Items.Add("Hospital 4");
        }

        //method used to check if user exists with a certain NHS number
        private async Task<UserRecords> GetRecords()
        {
            //store value
            string nhsinput = NHSIDCreator.Text;
   

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

                //gets NHS ID
                string id = NHSIDCreator.Text;

                //post new emergency to database
                var postUser = await firebaseClient
                    .Child("UserRecords")
                    .PostAsync(new UserRecords() { NHSid = id, name = patientNameBox.Text, address = PatientAddressBox.Text, medicalCondition = Condition.Text });

            //display to screen
            pushedDetails.Items.Clear();
            globalNHSID = id;
            globalAddress = PatientAddressBox.Text;
            pushedDetails.Items.Add("NHS ID: " + id);
            pushedDetails.Items.Add("Patient Name: " + patientNameBox.Text);
            pushedDetails.Items.Add("Patient Address: " + PatientAddressBox.Text);
            pushedDetails.Items.Add("Medical Condition: " + Condition.Text);
            MessageBox.Show("Added emergency to database");

        }

        //used to dispatch ambulance to patient and assign a hospital
        async void dispatch_Click(object sender, RoutedEventArgs e)
        {
            //if pushDetails box contains more than 0 items then an emergency has been logged which can be assigned to an ambulance
            if (pushedDetails.Items.Count > 0)
            {
                //user must select a hospital and ambulance to dispatch the case too
                if (!string.IsNullOrEmpty(ambulanceBox.Text) && !string.IsNullOrEmpty(hostpitalBox.Text))
                {
                    //generate new caseNo to uniquely identify each emergency call
                    Guid caseNo = new Guid();
                    //convert to string for server storage
                    string ConvertedCase = caseNo.ToString();

                    //post new emergency to server
                    var postAmbulance = await firebaseClient
                        .Child("Dispatches")
                        .PostAsync(new Dispatches() { nhsID = globalNHSID, caseNumber = ConvertedCase, patientAddress = globalAddress, ambulance = ambulanceBox.Text, hosptial = hostpitalBox.Text, condition = globalCondition, active = true });
                    MessageBox.Show("Dispatched");
                }
                else
                {
                    MessageBox.Show("Please select an ambulance and hosptial to dispatch too");
                }
            }
            else
            {
                MessageBox.Show("No emergency entered");
            }
        }
    }
}
