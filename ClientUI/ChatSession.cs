using System;
using System.Drawing;
using System.Windows.Forms;

namespace IrisIM
{
	namespace Client
	{
		namespace DefaultUI
		{
			public class ChatSession
			{
				private TextBox _chat_window;
				private TextBox _user_list;
				private TextBox _entry;
				private Splitter _splitter;
				private Button _send_button;
				private int _user_list_width;
				private IrisUI _application;
				
				public ChatSession(IrisUI application)
				{
					this._application = application;
					this.initalize();
				}
				
				public void focus()
				{
					this._application.container.Controls.Clear();
					this._application.container.Controls.Add(this._user_list);
					this._application.container.Controls.Add(this._splitter);
					this._application.container.Controls.Add(this._chat_window);
					this._application.container.Controls.Add(this._entry);
					this._application.container.Controls.Add(this._send_button);
					this._application.status_bar_inform.Text = "Focused.";
				}
				
				private void initalize()
				{
					this._application.status_bar_inform.Text = "Initializing...";
					this._chat_window = new TextBox();
					this._user_list = new TextBox();
					this._entry = new TextBox();
					this._splitter = new Splitter();
					this._send_button = new Button();
					
					// dummy text to be removed //
					this._chat_window.Text = "Hi there";
					this._user_list.Text = "Username";
					
					/* start chat window setup */
					this._chat_window.Multiline = true;
					this._chat_window.Height = this._application.container.Height - 25;
					this._chat_window.Width = this._application.container.Width - 100;
					this._chat_window.ReadOnly = true;
					this._chat_window.BackColor = Color.White;
					this._chat_window.Top = 5;
					this._chat_window.Left = 5;
					this._chat_window.BorderStyle = BorderStyle.FixedSingle;
					this._chat_window.Resize += new EventHandler(this.resize_fix_heights);
					this._chat_window.Dock = DockStyle.Left; 
					/* end chat window setup */
					/* start splitter setup */
					this._splitter.BorderStyle = BorderStyle.FixedSingle;
					this._splitter.Location = new Point(this._chat_window.Left + this._chat_window.Width, 0);
					this._splitter.Height = this._application.container.Height - 25;
					this._splitter.Width = 3;
					this._splitter.BackColor = Color.Blue;
					this._splitter.Dock = DockStyle.Left;
					this._splitter.SplitterMoved += new SplitterEventHandler(this.split_resize_user_list);
					/*end splitter setup */
					/* start user list setup */
					this._user_list.Multiline = true;
					this._user_list.ReadOnly = true;
					this._user_list.BorderStyle = BorderStyle.FixedSingle;
					this._user_list.BackColor = Color.White;
					this._user_list.Height = this._application.container.Height - 25;
					this._user_list.Width = this._user_list_width = 150;
					this._user_list.Dock = DockStyle.Left;
					this._user_list.Left = this._chat_window.Left + this._chat_window.Width;
					this._user_list.Resize += new EventHandler(this.resize_fix_heights);
					this._user_list.Top = 5;
					/* end user list setup */
					/* start entry setup */
					this._entry.BorderStyle = BorderStyle.FixedSingle;
					this._entry.BackColor = Color.White;
					this._entry.Width = this._application.container.Size.Width - 70;
					this._entry.Left = 0;
					this._entry.Top = this._application.container.Height - 20;
					/* end entry setup */
					/* start send button setup */
					this._send_button.Width = 40;
					this._send_button.Height = 15;
					this._send_button.Text = "Send";
					this._send_button.BackColor = Color.White;
					this._send_button.Top = this._application.container.Height - 25;
					this._send_button.Left = this._application.container.Width - 50;
					this._application.status_bar_inform.Text = "Focusing...";
					this.focus();
					/* set resizing hooks and we are done */
					this._application.container.SizeChanged += new EventHandler(this.resize_session);
				}
				
				private void resize_session(object sender, EventArgs args)
				{
					this.hold_user_list(this, EventArgs.Empty);
					this.resize_chat_window(this, EventArgs.Empty);
					this._splitter.Height = this._application.container.Height - 25;
					this._entry.Width = this._application.container.Size.Width - 70;
					this._entry.Left = 0;
					this._entry.Top = this._application.container.Height - 20;
					this._send_button.Top = this._application.container.Height - 20;
					this._send_button.Left = this._application.container.Width - 50;
				}
				
				private void resize_chat_window(object sender, EventArgs args)
				{
					this._chat_window.Width = this._application.container.Size.Width - this._user_list_width - 3;
					this._chat_window.Height = this._application.container.Size.Height - 25;
				}
				
				private void hold_user_list(object sender, EventArgs args)
				{
					this._user_list.Left = this._application.container.Width - this._user_list_width;
					this._user_list.Height = this._application.container.Size.Height - 25;
				}
				
				private void resize_user_list(object sender, EventArgs args)
				{
					this._user_list_width = this._application.container.Size.Width - this._chat_window.Size.Width;
					this._user_list.Size = new Size(this._user_list_width, this._application.container.Size.Height - 25);
				}
				
				private void split_resize_user_list(object sender, SplitterEventArgs args)
				{
					this.resize_user_list(this, EventArgs.Empty);
				}
				
				private void resize_fix_heights(object sender, EventArgs args)
				{
					this._user_list.Height = this._application.container.Size.Height - 25;
					this._chat_window.Height = this._application.container.Size.Height - 25;
					this._splitter.Height = this._application.container.Size.Height - 25;
				}
			}
		}
	}
}