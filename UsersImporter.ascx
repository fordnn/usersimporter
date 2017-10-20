<%@ Control Language="C#" Inherits="forDNN.Modules.UsersImporter.UsersImporter" AutoEventWireup="true"
	CodeBehind="UsersImporter.ascx.cs" %>
<br />
<asp:HyperLink ID="lnkExample" runat="server" CssClass="CommandButton" resourcekey="Example"></asp:HyperLink><br /><br />
<asp:Label ID="lblImport" CssClass="NormalRed" runat="server" resourcekey="Import"></asp:Label>&nbsp;
<input id="objFile" type="file" name="objFile" runat="server"><br><br>
<asp:LinkButton ID="btnImport" runat="server" CssClass="CommandButton" OnClick="btnImport_Click"
	resourcekey="ImportUsers"></asp:LinkButton>
<br>
<asp:Label ID="lblResult" CssClass="NormalRed" runat="server"></asp:Label>
<br />
<br />
<br />
<br />
<asp:Label ID="lblIcon" runat="server"></asp:Label>