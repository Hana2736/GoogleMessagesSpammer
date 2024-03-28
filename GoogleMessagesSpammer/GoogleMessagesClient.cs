using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace GoogleMessagesSpammer
{
    public abstract class GoogleMessagesClient
    {
        private static Dictionary<string, bool> isRcs;
        private static IWebDriver driver;
        private static string selectedUser;
        private static IWebElement startChatBtn, msgBar;
        private static bool setup, loggedIn;

        internal static void Setup()
        {
            Console.WriteLine("Setting up. This can take a while");
            if (setup) Console.WriteLine("Already set up!!");
            isRcs = new Dictionary<string, bool>();
            selectedUser = "nobody";
            var options = new FirefoxOptions();
            options.SetLoggingPreference(LogType.Profiler, LogLevel.Off);
            options.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
            options.SetLoggingPreference(LogType.Performance, LogLevel.Off);
            options.SetLoggingPreference(LogType.Client, LogLevel.Off);
            options.SetLoggingPreference(LogType.Server, LogLevel.Off);
            options.LogLevel = FirefoxDriverLogLevel.Fatal;
            options.AddArgument("--log");
            options.AddArgument("error");
            options.AddArgument("-profile");
            var firefoxStore = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.googleMessages";
            try
            {
                Directory.CreateDirectory(firefoxStore);
            }
            catch
            {
                //Means we got it
            }

            options.AddArgument(firefoxStore);
            options.AddArgument("-headless");
            options.AddArgument("--width=900");
            options.AddArgument("--height=450");
            driver = new FirefoxDriver(options);
            driver.Url = "https://messages.google.com/web/conversations";
            Console.WriteLine("Messages for Web found");
            Thread.Sleep(4500);
            setup = true;
        }

        internal static void CheckLogin()
        {
            if (!setup || loggedIn) Console.WriteLine("Not set up / already logged in!!!");
            ;
            try
            {
                startChatBtn = driver.FindElement(By.CssSelector("[class=\"start-chat\"]"));
                Console.WriteLine("Already logged in.");
            }
            catch
            {
                while (true)
                    try
                    {
                        driver.FindElement(By.XPath("//*[@id=\"mat-mdc-slide-toggle-1-label\"]")).Click();
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }

                var imageElement = driver.FindElement(By.CssSelector("[alt=\"Scan me!\"]"));
                var tempPath = Environment.GetEnvironmentVariable("temp") + "/pairQR.html";
                File.WriteAllText(tempPath,
                    "<html><body><img src=\"" + imageElement.GetAttribute("src") + "\"/></body></html>");
                Process.Start(tempPath);
                Console.WriteLine(
                    "Login to GMessages with the QR in your browser....\nIt will expire soon, if it doesn't work, restart.");
                while (true)
                    try
                    {
                        startChatBtn = driver.FindElement(By.CssSelector("[class=\"start-chat\"]"));
                        Console.WriteLine("Logged in.");
                        File.Delete(tempPath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }
            }
        }

        internal static void ChangeUser(string phone)
        {
            //Console.WriteLine("Enter contact phone number:");
            if (phone != selectedUser)
            {
                startChatBtn.FindElement(By.CssSelector("[href=\"/web/conversations/new\"")).Click();
                Thread.Sleep(500);
                var typeBar = driver.FindElement(By.CssSelector("[data-e2e-contact-input=\"\"]"));
                var rawNum = Regex.Replace(phone, "[^.0-9]", "");
                typeBar.SendKeys(rawNum);
                Thread.Sleep(25);
                while (true)
                    try
                    {
                        Thread.Sleep(10);
                        typeBar.SendKeys(Keys.Enter);
                    }
                    catch
                    {
                        break;
                    }

                while (true)
                    try
                    {
                        msgBar = driver.FindElement(By.CssSelector("[data-e2e-message-input-box=\"\"]"));
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(50);
                    }

                Console.WriteLine("Loaded conversation");
                if (!isRcs.ContainsKey(phone))
                {
                    Thread.Sleep(2500);
                    bool isRCS;
                    try
                    {
                        driver.FindElement(By.CssSelector("[placeholder=\"RCS message\"]"));
                        isRCS = true;
                    }
                    catch
                    {
                        isRCS = false;
                    }

                    isRcs[phone] = isRCS;
                }

                selectedUser = phone;
                Console.WriteLine("Conversation is " + (isRcs[phone] ? "RCS" : "SMS"));
            }
        }

        internal static void SendMessage(string msg)
        {
            msgBar.SendKeys(msg.Trim().Replace("\n", Keys.Enter) + Keys.LeftControl + Keys.Enter);
            Console.WriteLine("Sending....");
            Thread.Sleep(150);
            var foundSent = false;
            while (true)
            {
                try
                {
                    driver.FindElement(
                        By.CssSelector(
                            "[class=\"mat-mdc-tooltip-trigger status-icon sending-icon ng-star-inserted\"]"));
                    foundSent = true;
                }
                catch
                {
                    if (foundSent)
                        break;
                }

                Thread.Sleep(100);
            }

            Console.WriteLine("Sent.");
        }
    }
}