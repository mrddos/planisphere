package org.healer.gpass;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.jdo.PersistenceManager;
import javax.jdo.Query;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.PersistenceManagerDictionary;
import org.healer.gpass.beans.Entry;
import org.healer.gpass.beans.RecordEntry;


import com.google.appengine.api.datastore.Key;
import com.google.appengine.api.datastore.KeyFactory;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;




public class GPassServlet extends HttpServlet {
	private static final long serialVersionUID = 7982223539268242856L;
	
	

	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws IOException {


		String result = null;
		
		String command = request.getParameter("command");
		if ("add".equalsIgnoreCase(command)) {
			long id = addNewRecordEntry(request, response);
			result = String.valueOf(id);
			
		} else if ("remove".equalsIgnoreCase(command)) {
			result = removeRecordEntry(request, response);
			
		} else if ("modify".equalsIgnoreCase(command)) {
			modifyRecordEntry(request, response);
			
		} else if ("get".equalsIgnoreCase(command)) {
			getRecordEntry();
			
		} else if ("list".equalsIgnoreCase(command)) {
			List<Entry> entries = listRecordEntries(request, response);

			GsonBuilder builder = new GsonBuilder();
			//
			Gson gson = builder.create();

			
			result = gson.toJson(entries);
			
		} else if ("bank".equalsIgnoreCase(command)) {
			
		} else {
			result = "command string required.";
		}

		response.setContentType("text/plain");
		response.setCharacterEncoding("UTF-8");
		response.getWriter().write(result);
	}
	
	@SuppressWarnings("unchecked")
	private List<Entry> listRecordEntries(HttpServletRequest request, HttpServletResponse response) {
		PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
		Query query = pm.newQuery(RecordEntry.class);

	    List<RecordEntry> results = (List<RecordEntry>) query.execute();
	    
	    List<Entry> ret = new ArrayList<Entry>();
	    for (RecordEntry re: results) {
	    	ret.add(new Entry(re));
	    }
	    pm.close();
		return ret;
		
	}
	
	@Deprecated
	private void getRecordEntry() {
		
	}
	
	private long addNewRecordEntry(HttpServletRequest request, HttpServletResponse response) {
		String name = request.getParameter(Entry.NAME);
		String desc = request.getParameter(Entry.DESC);
		String secr = request.getParameter(Entry.SECR);
		if (name != null) {
			if (desc == null) {
				desc = "";
			}
			if (secr == null) {
				secr = "";
			}
			RecordEntry re = new RecordEntry(name, desc, secr);
			PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
			re = pm.makePersistent(re);

			pm.flush();
			pm.close();
			
			return re.getId();
		}
		
		return -1;
	}
	
	private String removeRecordEntry(HttpServletRequest request, HttpServletResponse response) {
		String Id = request.getParameter("id");
		if (Id != null && Id.length() > 0) {
			long id = Long.parseLong(Id);
			
			PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
			
			Key key = KeyFactory.createKey(RecordEntry.class.getSimpleName(), id);
			RecordEntry re = pm.getObjectById(RecordEntry.class, key);
			if (re != null) {
				pm.deletePersistent(re);
			} else {
				return "Error_NoThisObject";
			}
			pm.close();
			return "Success";
		}
		return "Error_WrongParameter";
	}
	
	private void modifyRecordEntry(HttpServletRequest request, HttpServletResponse response) {
		String Id = request.getParameter("id");
		if (Id != null && Id.length() > 0) {
			long id = Long.parseLong(Id);
			
			PersistenceManager pm = PersistenceManagerDictionary.get("secret-optional");
			
			Key key = KeyFactory.createKey(RecordEntry.class.getSimpleName(), id);
			RecordEntry re = pm.getObjectById(RecordEntry.class, key);
			
			// Add code for modify the RecordEntry;
			pm.close();
		}
	}
}
