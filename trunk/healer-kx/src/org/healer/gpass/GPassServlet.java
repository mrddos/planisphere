package org.healer.gpass;

import java.io.IOException;
import java.util.List;

import javax.jdo.PersistenceManager;
import javax.jdo.Query;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.PersistenceManagerDictionary;
import org.healer.gpass.beans.RecordEntry;
import org.mortbay.util.ajax.JSON;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class GPassServlet extends HttpServlet {
	private static final long serialVersionUID = 7982223539268242856L;
	
	

	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws IOException {


		String result = null;
		
		String command = request.getParameter("command");
		if ("add".equalsIgnoreCase(command)) {
			addNewRecordEntry(request, response);
			result = "add success";
		} else if ("remove".equalsIgnoreCase(command)) {
			removeRecordEntry();
		} else if ("modify".equalsIgnoreCase(command)) {
			modifyRecordEntry();
		} else if ("get".equalsIgnoreCase(command)) {
			getRecordEntry();
		} else if ("list".equalsIgnoreCase(command)) {
			List entries = listRecordEntries(request, response);
			
			GsonBuilder builder = new GsonBuilder();
			Gson gson = builder.create();  
			entries.size();
			
			result = gson.toJson(entries);
		} else {
			result = "command string required.";
		}

		response.setContentType("text/plain");
		response.getWriter().write(result);
	}
	
	private List listRecordEntries(HttpServletRequest request, HttpServletResponse response) {
		PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
		Query query = pm.newQuery(RecordEntry.class);
	    

	    List<RecordEntry> results = (List<RecordEntry>) query.execute();
		return results;
		
	}
	
	private void getRecordEntry() {
		
	}
	
	private void addNewRecordEntry(HttpServletRequest request, HttpServletResponse response) {
		String desc = request.getParameter("desc");
		String secr = request.getParameter("secr");
		RecordEntry re = new RecordEntry(desc, secr);
		
		PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
		pm.makePersistent(re);

		pm.flush();
		pm.close();
	}
	
	private void removeRecordEntry() {

	}
	
	private void modifyRecordEntry() {
		
	}
}
