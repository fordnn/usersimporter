using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;

namespace forDNN.Modules.UsersImporter
{
	partial class UsersImporter : PortalModuleBase, IActionable
	{

		#region "Event Handlers"

		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// Page_Load runs when the control is loaded 
		/// </summary> 
		/// ----------------------------------------------------------------------------- 
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				lblIcon.Visible = true;
				lblIcon.Style["display"] = "block";
				lblIcon.Text = "<a href=\"http://forDNN.com\" target=\"_blank\"><img src=\"http://forDNN.com/forDNNTeam.gif\" border=\"0\"/></a>";

				lnkExample.NavigateUrl = ResolveUrl("UsersImporterExample.xls");
			}

			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		private void DoImport()
		{
			string FileName = string.Format("{0}ImportUsers_{1}.xls",
				PortalSettings.HomeDirectoryMapPath,
				DateTime.Now.Ticks);
			objFile.PostedFile.SaveAs(FileName);

			string ConnectionString =
							string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\";", FileName);
			System.Data.OleDb.OleDbConnection objConnection = new System.Data.OleDb.OleDbConnection(ConnectionString);

			objConnection.Open();

			System.Data.OleDb.OleDbCommand objCommand = new System.Data.OleDb.OleDbCommand();
			objCommand.Connection = objConnection;
			objCommand.CommandType = System.Data.CommandType.Text;
			objCommand.CommandText = "SELECT [AffiliateId], [Email], [FirstName], [IsSuperUser], [LastName], [Username], [Password], [Country], [Street], [City], [Region], [PostalCode], [Unit], [Telephone] FROM [Users$];";

			DataTable dt = DotNetNuke.Common.Globals.ConvertDataReaderToDataTable(objCommand.ExecuteReader());
			objConnection.Close();

			int UsersCount = 0;
			int SuccessUsersCount = 0;
			string FailedUsers = "";

			foreach (DataRow dr in dt.Rows)
			{
				UsersCount++;
				try
				{
					DotNetNuke.Entities.Users.UserInfo objUser = new DotNetNuke.Entities.Users.UserInfo();
					objUser.AffiliateID = Convert.ToInt32(dr["AffiliateId"].ToString());
					objUser.Email = dr["Email"].ToString();
					objUser.FirstName = dr["FirstName"].ToString();
					objUser.IsSuperUser = (dr["IsSuperUser"].ToString() == "1");
					objUser.LastName = dr["LastName"].ToString();
					objUser.PortalID = this.PortalId;
					objUser.Username = dr["Username"].ToString();
					objUser.DisplayName = string.Format("{0} {1}", objUser.FirstName, objUser.LastName);
					objUser.Membership.Password = dr["Password"].ToString();

					//Please uncomment next lines in case you need to generate valid length password
					//if (objUser.Membership.Password.Length < 7)
					//{
					//    objUser.Membership.Password = "C0mpan1" + objUser.Membership.Password;
					//}

					objUser.Membership.Approved = true;
					objUser.Membership.Email = objUser.Email;
					objUser.Membership.Username = objUser.Username;
					objUser.Membership.PasswordQuestion = objUser.Membership.Password;
					objUser.Membership.UpdatePassword = true;

					objUser.Profile.Country = dr["Country"].ToString();
					objUser.Profile.Street = dr["Street"].ToString();
					objUser.Profile.City = dr["City"].ToString();
					objUser.Profile.Region = dr["Region"].ToString();
					objUser.Profile.PostalCode = dr["PostalCode"].ToString();
					objUser.Profile.Unit = dr["Unit"].ToString();
					objUser.Profile.Telephone = dr["Telephone"].ToString();
					objUser.Profile.FirstName = objUser.FirstName;
					objUser.Profile.LastName = objUser.LastName;

					DotNetNuke.Security.Membership.UserCreateStatus objCreateStatus =
						DotNetNuke.Entities.Users.UserController.CreateUser(ref objUser);
					if (objCreateStatus == DotNetNuke.Security.Membership.UserCreateStatus.Success)
					{
						SuccessUsersCount++;
					}
					else
					{
						FailedUsers += string.Format(Localization.GetString("RowMembershipError", this.LocalResourceFile),
							UsersCount,
							objUser.Username,
							objCreateStatus.ToString());
					}
				}
				catch (Exception Exc)
				{
					FailedUsers += string.Format(Localization.GetString("RowException", this.LocalResourceFile),
						UsersCount,
						Exc.Message);
				}
			}
			lblResult.Text = string.Format(Localization.GetString("Result", this.LocalResourceFile),
				UsersCount,
				SuccessUsersCount,
				FailedUsers);
		}


		protected void btnImport_Click(object sender, EventArgs e)
		{
			DoImport();
		}

		#endregion

		#region "Optional Interfaces"

		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// Registers the module actions required for interfacing with the portal framework 
		/// </summary> 
		/// <value></value> 
		/// <returns></returns> 
		/// <remarks></remarks> 
		/// <history> 
		/// </history> 
		/// ----------------------------------------------------------------------------- 
		public ModuleActionCollection ModuleActions
		{
			get
			{
				ModuleActionCollection Actions = new ModuleActionCollection();
				//Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile),
				//   ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit,
				//    true, false);
				return Actions;
			}
		}

		#endregion

	}

}