<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="OneTrueError.Client.AspNet.Demo.Errors.NotFound" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Failed to find <%= Exception.Message %></title>
</head>
<body>
	<form method="post" action="$URL$">
		<input type="hidden" value="$reportId$" name="reportId" />
        <div>
            Page is not found
        </div>
        <div>
           <%= ErrorContext.HttpStatusCode  %>
        </div>
		<div>
			<p>Could you please let us know how to reproduce it? Any information you	 give us will help us solve it faster.</p>
			<textarea rows="10" cols="40" name="Description"></textarea>
		</div>
    </form>
</body>
</html>
