using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QSOCollector;
using QSOCollector.Models;
using QSOCollector.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace QSOCollector.Tests
{
    [TestClass]
    public class QsoCollectorFormTests
    {
        private QsoCollectorForm form;
        private Mock<DbRepository> dbRepositoryMock;
        private StartupParams startupParams;

        [TestInitialize]
        public void Setup()
        {
            dbRepositoryMock = new Mock<DbRepository>("Data Source=:memory:");
            startupParams = new StartupParams();
            form = (QsoCollectorForm)Activator.CreateInstance(
                typeof(QsoCollectorForm),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new object[] { "Data Source=:memory:", startupParams },
                null
            );
            // Optionally inject the mock repository if you refactor for testability
        }

        [TestMethod]
        public void RestoreSavedFormValuesFromDB_SetsControlsCorrectly()
        {
            var settings = new Dictionary<string, string?>()
            {
                { "ServerEnabled", "True" },
                { "ServerPort", "1234" },
                { "ClientEnabled", "False" },
                { "ClientServerNameIp", "127.0.0.1" },
                { "ClientServerPort", "5678" },
                { "AutoStart", "True" }
            };
            dbRepositoryMock.Setup(r => r.LoadSettings()).Returns(settings);

            // Use reflection to call private method
            var method = typeof(QsoCollectorForm).GetMethod("RestoreSavedFormValuesFromDB", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(form, null);

            Assert.AreEqual(true, form.Controls["enableServerCheckBox"].Enabled);
            Assert.AreEqual("1234", form.Controls["serverPortTextBox"].Text);
            Assert.AreEqual(false, form.Controls["enableClientCheckBox"].Enabled);
            Assert.AreEqual("127.0.0.1", form.Controls["clientServerNameIpTextBox"].Text);
            Assert.AreEqual("5678", form.Controls["clientServerPortTextBox"].Text);
            Assert.AreEqual(true, form.Controls["autoStartCheckbox"].Enabled);
        }

        [TestMethod]
        public void SaveFormValuesToDB_CallsRepositoryWithCorrectValues()
        {
            // Set up controls with test values
            form.Controls["enableServerCheckBox"].Enabled = true;
            form.Controls["serverPortTextBox"].Text = "4321";
            form.Controls["enableClientCheckBox"].Enabled = false;
            form.Controls["clientServerNameIpTextBox"].Text = "localhost";
            form.Controls["clientServerPortTextBox"].Text = "8765";
            form.Controls["autoStartCheckbox"].Enabled = false;

            // Use reflection to call private method
            var method = typeof(QsoCollectorForm).GetMethod("SaveFormValuesToDB", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(form, null);

            dbRepositoryMock.Verify(r => r.SaveSetting("ServerEnabled", "True"), Times.Once);
            dbRepositoryMock.Verify(r => r.SaveSetting("ServerPort", "4321"), Times.Once);
            dbRepositoryMock.Verify(r => r.SaveSetting("ClientEnabled", "False"), Times.Once);
            dbRepositoryMock.Verify(r => r.SaveSetting("ClientServerNameIp", "localhost"), Times.Once);
            dbRepositoryMock.Verify(r => r.SaveSetting("ClientServerPort", "8765"), Times.Once);
            dbRepositoryMock.Verify(r => r.SaveSetting("AutoStart", "False"), Times.Once);
        }

        [TestMethod]
        public void HandleStartupParams_StartServerAndClient_SetsWindowStateMinimized()
        {
            var testParams = new StartupParams { StartServer = true, StartClient = true, IsQuiet = true };
            var method = typeof(QsoCollectorForm).GetMethod("HandleStartupParams", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { testParams });
            Assert.AreEqual(FormWindowState.Minimized, form.WindowState);
        }

        [TestMethod]
        public void HandleCheckBoxChanged_EnablesChildControls()
        {
            var parent = new Panel();
            var checkBox = new CheckBox { Checked = true, Parent = parent };
            var button = new Button();
            parent.Controls.Add(checkBox);
            parent.Controls.Add(button);

            var method = typeof(QsoCollectorForm).GetMethod("HandleCheckBoxChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { checkBox });

            Assert.IsTrue(button.Enabled);
        }

        [TestMethod]
        public void HandleClientServerChanged_DisablesStartButtonIfFieldsEmpty()
        {
            var clientServerNameIpTextBox = new TextBox { Text = "" };
            var clientServerPortTextBox = new TextBox { Text = "" };
            var startClientButton = new Button();

            typeof(QsoCollectorForm).GetField("clientServerNameIpTextBox", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, clientServerNameIpTextBox);
            typeof(QsoCollectorForm).GetField("clientServerPortTextBox", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, clientServerPortTextBox);
            typeof(QsoCollectorForm).GetField("startClientButton", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, startClientButton);

            var method = typeof(QsoCollectorForm).GetMethod("HandleClientServerChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, null);

            Assert.IsFalse(startClientButton.Enabled);
        }

        [TestMethod]
        public void GetAppStartupCmd_ReturnsExpectedCommand()
        {
            var enableServerCheckBox = new CheckBox { Enabled = true };
            var enableClientCheckBox = new CheckBox { Enabled = false };
            typeof(QsoCollectorForm).GetField("enableServerCheckBox", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, enableServerCheckBox);
            typeof(QsoCollectorForm).GetField("enableClientCheckBox", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, enableClientCheckBox);

            var method = typeof(QsoCollectorForm).GetMethod("GetAppStartupCmd", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(form, null) as string;

            Assert.IsTrue(result.Contains("--start-server"));
            Assert.IsTrue(result.Contains("--quiet"));
        }

        [TestMethod]
        public void HandleExportEnabled_EnablesExportButtonIfRowsPresent()
        {
            var serverQsoAmountsDataGridView = new DataGridView();
            var qsoExportButton = new Button();
            serverQsoAmountsDataGridView.Rows.Add();
            serverQsoAmountsDataGridView.Rows.Add();

            typeof(QsoCollectorForm).GetField("serverQsoAmountsDataGridView", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, serverQsoAmountsDataGridView);
            typeof(QsoCollectorForm).GetField("qsoExportButton", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(form, qsoExportButton);

            var method = typeof(QsoCollectorForm).GetMethod("HandleExportEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, null);

            Assert.IsTrue(qsoExportButton.Enabled);
        }
    }
}