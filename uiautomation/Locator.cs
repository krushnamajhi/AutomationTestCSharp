using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.uiautomation
{
    public class Locator
    {
        public By By { get; internal set; }
        public String Xpath { get; internal set; }

        internal Locator()
        {

        }

        public static Locator Id(String locator)
        {
            return new Locator() { By = By.Id(locator), Xpath = $"//*[@id=\"{locator}\"]" };
        } 
        public static Locator Name(String locator)
        {
            return new Locator() { By = By.Name(locator), Xpath = $"//*[@name=\"{locator}\"]" };
        } 
        
        public static Locator XPath(String locator)
        {
            return new Locator() { By = By.XPath(locator), Xpath = locator };
        }
        
        public static Locator CSSSelector(String locator)
        {
            return new Locator() { By = By.CssSelector(locator), Xpath = $"//*[@css=\"{locator}\"]" };
        }
        public static Locator LinkText(String locator)
        {
            return new Locator() { By = By.LinkText(locator), Xpath = $"//*[text()=\"{locator}\"]" };
        } 
        public static Locator PartialLinkText(String locator)
        {
            return new Locator() { By = By.PartialLinkText(locator), Xpath = $"//*[contains(text(),\"{locator}\")]" };
        } 
        public static Locator ClassName(String locator)
        {
            return new Locator() { By = By.ClassName(locator), Xpath = $"//*[@class=\"{locator}\"]" };
        }
        
        public static Locator Text(String locator)
        {
            return new Locator() { By = By.XPath($"//*[text()=\"{locator}\"]"), Xpath = $"//*[text()=\"{locator}\"]" };
        } 
        public static Locator PartialText(String locator)
        {
            return new Locator() { By = By.XPath($"//*[contains(text(),\"{locator}\")]"), Xpath = $"//*[contains(text(),\"{locator}\")]" };
        }
    }
}
