package org.healer.service.task;

import java.io.IOException;
import java.io.Writer;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.service.RESTService;

public class TaskService implements RESTService {

	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		
		
		try {
			response.setContentType("text/html");
			Writer writer = response.getWriter();
			
			writer.write("<div style='color:red'>Hello World</div>");
			writer.flush();
			writer.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	@Override
	public void onPost(HttpServletRequest request, HttpServletResponse response) {

	}

	@Override
	public void onPut(HttpServletRequest request, HttpServletResponse response) {

	}

	@Override
	public void onDelete(HttpServletRequest request,
			HttpServletResponse response) {

	}

}
