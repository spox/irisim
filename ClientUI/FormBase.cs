// FormBase.cs created with MonoDevelop
// User: sine at 8:53 AM 2/1/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IrisIM
{
	namespace UI
	{
		public class FormBase : System.Windows.Forms.Form{
			// menu
			private System.Windows.Forms.MainMenu mainMenu;
			// actions menu
			private System.Windows.Forms.MenuItem menuActions;
			private System.Windows.Forms.MenuItem menuActionsConnect;
			private System.Windows.Forms.MenuItem menuActionsDisconnect;
			private System.Windows.Forms.MenuItem menuActionsQuit;
			// settings menu
			private System.Windows.Forms.MenuItem menuSettings;
			// help menu
			private System.Windows.Forms.MenuItem menuHelp;
			private System.Windows.Forms.MenuItem menuHelpAbout;
			// database
			private System.Data.SQLite.SQLiteConnection dbconn;
			
			public FormBase(){
				this.Text = "Iris IM";
				this.DoInitializations();
			}
			
			~FormBase(){
				CleanupDatabase();
			}
			
			private void DoInitializations(){
				InitializeMenus();
				InitializeDatabase();
			}
			
			private void InitializeDatabase(){
				try{
					this.dbconn = new SQLiteConnection("Data Source=IrisIM.db;FailIfMissing=True;Version=3");
					this.dbconn.Open();
				}catch{
					this.dbconn = new SQLiteConnection("Data Source=IrisIM.db;Version=3");
					this.dbconn.Open();
					try{
						CreateDatabase();
					}catch(Exception e){
						throw new System.Exception("Failed to initialize storage: " + e.ToString());
					}
				}
			}
			
			private void CreateDatabase(){
				SQLiteCommand cmd = new SQLiteCommand();
				cmd.CommandText = "create table users( name varchar not null unique, password varchar not null, email varchar not null, id integer not null primary key)";
				cmd.Connection = this.dbconn;
				cmd.ExecuteNonQuery();
			}
			
			private void CleanupDatabase(){
				this.dbconn.Close();
			}
			
			private void InitializeMenus(){
				this.mainMenu = new System.Windows.Forms.MainMenu();
				this.Menu = this.mainMenu;
				BuildActionsMenu();
				BuildSettingsMenu();
				BuildHelpMenu();
			}
			
			private void BuildActionsMenu(){
				this.menuActions = new System.Windows.Forms.MenuItem();
				this.menuActionsConnect = new System.Windows.Forms.MenuItem();
				this.menuActionsDisconnect = new System.Windows.Forms.MenuItem();
				this.menuActionsQuit = new System.Windows.Forms.MenuItem();
				this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{this.menuActions});
				this.menuActions.Index = 0;
				this.menuActions.Text = "&Actions";
				this.menuActions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{
					this.menuActionsConnect, this.menuActionsDisconnect, this.menuActionsQuit});
				this.menuActionsConnect.Index = 0;
				this.menuActionsConnect.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
				this.menuActionsConnect.Text = "&Connect...";
				this.menuActionsDisconnect.Index = 1;
				this.menuActionsDisconnect.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
				this.menuActionsDisconnect.Text = "&Disconnect";
				this.menuActionsQuit.Index = 2;
				this.menuActionsQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
				this.menuActionsQuit.Text = "&Quit";
				this.menuActionsQuit.Click += new System.EventHandler(this.menuActionsQuit_Click);
			}
			
			private void BuildSettingsMenu(){
				this.menuSettings = new System.Windows.Forms.MenuItem();
				this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{this.menuSettings});
				this.menuSettings.Index = 0;
				this.menuSettings.Text = "&Settings";
			}
			
			private void BuildHelpMenu(){
				this.menuHelp = new System.Windows.Forms.MenuItem();
				this.menuHelpAbout = new System.Windows.Forms.MenuItem();
				this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{this.menuHelp});
				this.menuHelp.Index = 0;
				this.menuHelp.Text = "&Help";
				this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{this.menuHelpAbout});
				this.menuHelpAbout.Index = 0;
				this.menuHelpAbout.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
				this.menuHelpAbout.Text = "&About";
				this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
			}
			
			protected void menuHelpAbout_Click(object sender, System.EventArgs e){
				string msg = "IrisIM\nThe secure instant messenger\n\nAuthor: spox\nSite: http://github.com/spox/irisim/tree/master";
				System.Windows.Forms.MessageBox.Show(msg, "About IrisIM",
				                                     System.Windows.Forms.MessageBoxButtons.OK,
				                                     System.Windows.Forms.MessageBoxIcon.Information);
			}
			
			protected void menuActionsQuit_Click(object sender, System.EventArgs e){
				System.Environment.Exit(0);
			}
		}
	}
}