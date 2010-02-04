<%@ page language="java" contentType="text/html; charset=UTF-8"%>
<%@ page import="com.google.appengine.api.users.User" %>
<%@ page import="com.google.appengine.api.users.UserService" %>
<%@ page import="com.google.appengine.api.users.UserServiceFactory" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html">
<title>Login GPass</title>
<style>
	.datalist {
		border: 1px solid;
	}
	.datalist td {
		border: 1px solid;
	}
</style>
<script src="http://www.google.com/jsapi"></script>  
<script type="text/javascript">  
	google.load("jquery", "1.3.1");  
</script> 

</head>
<body>



		<%
		    UserService userService = UserServiceFactory.getUserService();
		    User user = userService.getCurrentUser();
		    if (user != null) {
		%>
				<p>Hello, <%= user.getNickname() %>! (You can
				<a href="<%= userService.createLogoutURL("/login.jsp") %>">sign out</a>.)</p>
		<%
		    } else {
		%>
				<p>Hello!
				<a href="<%= userService.createLoginURL("/index.jsp") %>">Sign in</a>
				to include your name with greetings you post.</p>
		<%
		    }
		%>
</body>
</html>