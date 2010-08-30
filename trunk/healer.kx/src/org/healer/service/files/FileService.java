package org.healer.service.files;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.config.Log;
import org.healer.service.RESTService;

public class FileService implements RESTService {

	
	private static void uploadFile(String fileName) {

		// Servlet
	}
	
	
	private static void getFile(String fileName) {
		
		
		getFileFromDataBase();
	}
	
	private static void getFileFromDataBase() {
		
	}

	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		uploadFile("");
		
	}

	@Override
	public void onPost(HttpServletRequest request, HttpServletResponse response) {
		String method = request.getMethod();
		if (method.equalsIgnoreCase("GET")) {
			Log.log("GET");
			uploadFile("");
		} else if (method.equalsIgnoreCase("POST")) {
			Log.log("POST");
		} else if (method.equalsIgnoreCase("PUT")) {
			Log.log("PUT");
		} else if (method.equalsIgnoreCase("DELETE")) {
			Log.log("DELETE");
		}

		try {
			String fileName = "";
			getFile(fileName);
			
			response.setContentType("text/plain");
			response.getWriter().write("FileService invoked");
			response.getWriter().flush();
			response.getWriter().close();
		} catch (IOException e) {
			e.printStackTrace();
		}

		
	}

	@Override
	public void onPut(HttpServletRequest request, HttpServletResponse response) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onDelete(HttpServletRequest request,
			HttpServletResponse response) {
		// TODO Auto-generated method stub
		
	}

}
