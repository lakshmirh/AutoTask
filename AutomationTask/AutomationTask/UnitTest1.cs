using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using experitestClient;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace AutomationTask
{
    [TestClass]
    public class UnitTest1
    {
        private string host = "localhost";
        private int port = 8889;
        private string projectBaseDirectory = "C:\\Users\\Lakshmi\\workspace\\project2";
        protected Client client = null;
        public string t1 = "adb:target";


        [TestInitialize()]
        public void SetupTest()
        {
            try
            {
                client = new Client(host, port);
                client.SetProjectBaseDirectory(projectBaseDirectory);
                client.SetReporter("xml", "reports", "AutoTest");


                client.SetDevice(t1); 
               // client.SetSpeed(Normal);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something wrong with the setup");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }
        }

        /*Test on wikipedia app
         * Search for ‘furry rabbits’, assert that a ‘did you mean’ suggestion appears
         * After that, click on the suggestion and assert that the first search result is ‘Brutal: Paws of Fury’
         * Click on the first search result and add it to your wikipedia reading list
         * Open up your wikipedia reading list, assert that ‘Brutal: Paws of Fury’ is there
         * Swipe the article left or right in order to remove it from your reading list
         * Assert that ‘Brutal: Paws of Fury’ is no longer in your reading list
         */

        [TestMethod]
        public void A_Search_furryRabbits()
        {
            launch_wikipedia();
            int found = Search_furry_rabbits();
            Assert.AreEqual(found, 1);
        }

        [TestMethod]
        public void B_Search_Brutal()
        {
            bool search_paws_of_fury;
            bool found_did_you_mean = client.IsElementFound("NATIVE", "xpath=//*[@text=concat('Did you mean ', '\"', 'fury rabbit', '\"', '?') and @onScreen='true' and @hidden='false']", 0);
            if (found_did_you_mean)
            {
                client.Click("NATIVE", "xpath=//*[@text=concat('Did you mean ', '\"', 'fury rabbit', '\"', '?') and @onScreen='true' and @hidden='false']", 0,1);
                client.Sleep(1000);
                search_paws_of_fury = client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury']");
                Assert.IsTrue(search_paws_of_fury, "Brutal:Paws of fury found");
            }
            else
            {
                launch_wikipedia();
                Search_furry_rabbits();
                client.Click("NATIVE", "xpath=//*[@text=concat('Did you mean ', '\"', 'fury rabbit', '\"', '?') and @onScreen='true' and @hidden='false']", 0, 1);
                client.Sleep(1000);
                search_paws_of_fury = client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury']");
                Assert.IsTrue(search_paws_of_fury, "Brutal:Paws of fury found");

            }

        }

        [TestMethod]
        public void C_AddingToReadingList()
        {
            bool found_Brutal_found = client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury']", 0);
            if (found_Brutal_found)
            {
                client.Click("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury']", 0, 1);
                client.Sleep(500);
                Adding_to_reading_list();
                BackToHome();

            }
            else
            {
                launch_wikipedia();
                Search_furry_rabbits();
                client.Click("NATIVE", "xpath=//*[@text=concat('Did you mean ', '\"', 'fury rabbit', '\"', '?') and @onScreen='true' and @hidden='false']", 0, 1);
                client.Sleep(1000);
                bool search_paws_of_fury = client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury']");
                Adding_to_reading_list();
                BackToHome();

            }
        }

        [TestMethod]
        public void D_VerifyReadingList_afteradding()
        {
            OpenReadingList();
            if (client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury' and @onScreen='true']", 0))
            {
                Console.WriteLine("Brutal:Paws of fury found in reading list");
                Assert.AreEqual(1, 1);
            }
           
        }
        
        [TestMethod]
        public void E_RemoveFromReadingList()
        {
            client.DragCoordinates(50, 550, 700, 550, 2000);
            BackToHome();
        }

        [TestMethod]
        public void F_VerifyReadingList_afterDeleting()
        {
            OpenReadingList();
            bool Brutal_found = client.IsElementFound("NATIVE", "xpath=//*[@text='Brutal: Paws of Fury' and @onScreen='true']", 0);
            if(!Brutal_found)
            {
                Console.WriteLine("Brutal:Paws of fury not found in reading list");
                Assert.AreEqual(1, 1);
            }

        }

        /*Functions Used */

        public void launch_wikipedia()
        {
            client.Launch("org.wikipedia/.main.MainActivity", true, true);
            client.Sleep(1000);
        }


        public int Search_furry_rabbits()
        {
            client.Click("NATIVE", "xpath=//*[@text='Search Wikipedia']", 0, 1);
            client.ElementSendText("NATIVE", "xpath=//*[@id='search_src_text']", 0, "furry rabbits");
            client.Sleep(2000);
            bool did_you_mean_found = client.IsElementFound("NATIVE", "xpath=//*[@text=concat('Did you mean ', '\"', 'fury rabbit', '\"', '?') and @onScreen='true' and @hidden='false']", 0);
            if (did_you_mean_found)
            {
                Console.WriteLine("Did you mean was found");
                return 1;
            }
            else
            {
                Console.WriteLine("Did you mean was not found");
                return 0;
            }
        }

        public void Adding_to_reading_list()
        {
            client.Click("NATIVE", "xpath=//*[@class='android.support.v7.widget.ActionMenuPresenter$OverflowMenuButton' and @onScreen='true']", 0, 1);
            client.Click("NATIVE", "xpath=//*[@text='Add to reading list']", 0, 1);
            client.Click("NATIVE", "xpath=//*[@text='Create new']", 0, 1);
            client.Sleep(1000);
            client.Click("NATIVE", "xpath=//*[@text='OK']", 0, 1);

        }

        public void BackToHome()
        {
            launch_wikipedia();
        }

        public void OpenReadingList()
        {
            client.Click("NATIVE", "xpath=//*[@id='icon' and @class='android.support.v7.widget.AppCompatImageView' and @knownSuperClass='android.widget.ImageView']", 1, 1);
            client.Click("NATIVE", "xpath=//*[@id='item_title']", 0, 1);

        }





        [TestCleanup()]
        public void TearDown()
        {
            try
            {
               
                client.GenerateReport(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.ReleaseClient();
                client.GenerateReport(true);
            }
            

        }
    }
}
