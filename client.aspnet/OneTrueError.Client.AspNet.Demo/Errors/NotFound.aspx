<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="OneTrueError.Client.AspNet.Demo.Errors.NotFound" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Failed to find <%= Exception.Message %></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Page is not found
        </div>
        <div>
            <%= ErrorContext.HttpStatusCode  %>
        </div>
    </form>
</body>
</html>
