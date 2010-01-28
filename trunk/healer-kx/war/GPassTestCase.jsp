<%@ page language="java" contentType="text/html; charset=UTF-8"%>
<%@ page import="com.google.appengine.api.users.User" %>
<%@ page import="com.google.appengine.api.users.UserService" %>
<%@ page import="com.google.appengine.api.users.UserServiceFactory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html">
<title>GPass Test Cases</title>
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
<script text='javascript'>
function addOnClick() {
	
	var name = document.getElementById("name1").value;
	var desc = document.getElementById("desc1").value;
	var secr = document.getElementById("secr1").value;

	var url = "/gpass?" + "command=add" + "&name=" + name + "&desc=" + desc + "&secr=" + secr;
	console.debug(url);
	//ajax
	$.get( url, {},  function(data) { console.debug(data); }  
	);  
	
}
		
function updateRecordEntry(id) {
	alert(id);
}

function deleteRecordEntry(id) {
	var url = "/gpass?" + "command=remove" + "&id=" + id;
	$.get(  
			url,  
	         {},  
	         function(data) { console.debug(data); }  
	); 
}

function createCell(row, text) {
	var td = document.createElement("td");
	var span = document.createElement("span");
	span.innerHTML = text;
	td.appendChild(span);
	row.appendChild(td);
}

function createButtonCell(row, text, func) {
	var td = document.createElement("td");
	var btn = document.createElement("input");
	btn.type = "button";
	btn.value = text;
	btn.onclick = func;
	td.appendChild(btn);
	row.appendChild(td);
}

function createRow(table, data) {
	var tr = document.createElement("tr");
	var id = data["id"];
	createCell(tr, id);
	createCell(tr, data["name"]);
	createCell(tr, data["description"]);
	createCell(tr, data["secret"]);
	createButtonCell(tr, "update", function(){ updateRecordEntry(id);});
	createButtonCell(tr, "dalete", function(){ deleteRecordEntry(id);});
	table.appendChild(tr);
	
}

function fillDataList(data) {
	var list = eval(data);
	// a div
	var dataList = document.getElementById("dataList");
	
	var table = document.createElement("table");
	table.cellSpacing = "0";
	
	for (var d in list) {
		createRow(table, list[d]);
	}
	dataList.appendChild(table);
}

function listOnClick() {

	var url = "/gpass?" + "command=list";
	$.ajax( {type:"GET",  
			url: url,
			dataType: 'json',  
	        success: function(data) { fillDataList(data); }  
	});
}



</script>
</head>  
<body>
	<div>
		<%
		    UserService userService = UserServiceFactory.getUserService();
		    User user = userService.getCurrentUser();
		    if (user != null) {
		%>
				<p>Hello, <%= user.getNickname() %>! (You can
				<a href="<%= userService.createLogoutURL(request.getRequestURI()) %>">sign out</a>.)</p>
		<%
		    } else {
		%>
				<p>Hello!
				<a href="<%= userService.createLoginURL(request.getRequestURI()) %>">Sign in</a>
				to include your name with greetings you post.</p>
		<%
		    }
		%>
	</div>
	<table style="border:1px solid">
		<tr>
			<td>

			</td>
		<tr>
			<td>
				<input id='name1' type='text' value='<name>'/><br><br>
				<input id='desc1' type='text' value='<description>'/><br><br>
				<input id='secr1' type='text' value='<secret>'/>
			</td>
			<td>
				<input type='button' value='Add' onclick="addOnClick()">
			</td>
			<td>
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td>
				<input type='button' value='List All' onclick="listOnClick()">
			</td>
			<td>
			</td>
		</tr>
		<tr>
			<div id='dataList' class="datalist" style="width:800px">
				
			</div>
		</tr>
	</table>
</body>
</html>